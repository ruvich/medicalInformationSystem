using medicalApp.Data;
using medicalApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Xml.Linq;

namespace medicalApp.Controllers
{
    [Route("api/inspection")]
    [ApiController]
    public class Inspection : ControllerBase
    {
        private readonly AppDataContext _context;

        public Inspection(AppDataContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InspectionModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInspectionDetails (string id)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid inspectionId))
                {
                    return BadRequest(new { status = "400", message = "Invalid ID format. Value must be a valid GUID." });
                }

                var inspection = await _context.InspectionDataBaseModels
                    .Include(i => i.patient)
                    .Include(i => i.doctor)
                    .Include(i => i.diagnoses)
                    .Include(i => i.consultations)
                        .ThenInclude(c => c.speciality)
                    .Include(i => i.consultations)
                        .ThenInclude(c => c.comments)
                    .FirstOrDefaultAsync(i => i.id == inspectionId);

                if (inspection == null)
                {
                    return NotFound(new { status = "404", message = "Inspection not found" });
                }

                var inspectionModel = new InspectionModel
                {
                    id = inspectionId,
                    createTime = inspection.createTime,
                    date = inspection.date,
                    anamnesis = inspection.anamnesis,
                    complaints = inspection.complaints,
                    treatment = inspection.treatment,
                    conclusion = inspection.conclusion,
                    nextVisitDate = inspection.nextVisitDate,
                    deathDate = inspection.deathDate,
                    baseInspectionId = inspection.baseInspectionId,
                    previousInspectionId = inspection.previousInspectionId,
                    hasChain = inspection.hasChain,
                    hasNested = inspection.hasNested,
                    patient = new PatientShowModel
                    {
                        id = inspection.patient.id,
                        createTime = inspection.patient.createTime,
                        name = inspection.patient.name,
                        birthday = inspection.patient.birthday,
                        gender = inspection.patient.gender
                    },
                    doctor = new DoctorProfileModel {
                        id = inspection.doctor.id,
                        createTime = inspection.doctor.createTime,
                        name = inspection.doctor.name,
                        email = inspection.doctor.email,
                        birthday = inspection.doctor.birthday,
                        gender = inspection.doctor.gender,
                        phone = inspection.doctor.phone
                    },
                    diagnosis = inspection.diagnoses.Select(d => new DiagnosisModel{
                        id = d.id,
                        createTime = d.createTime,
                        code = d.code,
                        name = d.name,
                        description = d.description,
                        type = d.type
                    }).ToList(),
                    consultations = inspection.consultations.Select(c => new InspectionConsultationModel{
                        id = c.id,
                        createTime = c.createTime,
                        inspectionId = c.inspectionId,
                        speciality = new SpecialityShowModel
                        {
                            id = c.speciality.id,
                            name = c.speciality.name
                        },
                        rootComment = c.comments
                            .Where(c => c.parentId == null)
                            .OrderBy(c => c.createTime)
                            .Select(c => new InspectionCommentModel
                            {
                                id = c.id,
                                content = c.content,
                                createTime = c.createTime,
                                modifyTime = c.modifyTime,
                                author = new DoctorProfileModel
                                {
                                    id = c.author.id,
                                    createTime = c.author.createTime,
                                    name = c.author.name,
                                    email = c.author.email,
                                    birthday = c.author.birthday,
                                    gender = c.author.gender,
                                    phone = c.author.phone
                                }
                            }).FirstOrDefault(),
                        commentsNumber = c.comments.Count
                    }).ToList()
                };
                return Ok(inspectionModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "500", message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EditInspection(Guid id, [FromBody] InspectionEditModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { status = "400", message = "Invalid arguments" });
                }

                var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(doctorId))
                {
                    return Unauthorized(new { status = "401", message = "Unauthorized access" });
                }

                var inspection = await _context.InspectionDataBaseModels
                    .Include(ins => ins.diagnoses)
                    .FirstOrDefaultAsync(ins => ins.id == id);

                if (inspection == null)
                {
                    return NotFound(new { status = "404", message = "Inspection not found" });
                }

                if (inspection.doctorID != Guid.Parse(doctorId))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new { status = "403", message = "Access denied. You can only edit your own inspections." });
                }

                if (model.diagnosis == null || model.diagnosis.Count(d => d.type == DiagnosisType.Main) != 1)
                {
                    return BadRequest(new { status = "400", message = "Inspection must necessarily have one diagnosis with the diagnosis type 'Main'" });
                }

                if (model.conclusion == Conclusion.Disease)
                {
                    if (model.nextVisitDay == null)
                    {
                        return BadRequest(new { status = "400", message = "You must indicate the date of the next inspection." });
                    }

                    if (model.deathDate != null)
                    {
                        return BadRequest(new { status = "400", message = "Death date should not be provided for a Disease conclusion." });
                    }
                }

                if (model.conclusion == Conclusion.Death)
                {
                    if (model.deathDate == null)
                    {
                        return BadRequest(new { status = "400", message = "You must provide the date of death." });
                    }

                    if (model.nextVisitDay != null)
                    {
                        return BadRequest(new { status = "400", message = "Next visit date should not be provided for a Death conclusion." });
                    }
                }

                if (model.conclusion == Conclusion.Recovery)
                {
                    if (model.nextVisitDay != null || model.deathDate != null)
                    {
                        return BadRequest(new { status = "400", message = "Neither next visit date nor death date should be provided for a Recovery conclusion." });
                    }
                }

                var deathInspectionExist = await _context.InspectionDataBaseModels.AnyAsync(ins => ins.patientID == id && ins.conclusion == Conclusion.Death);
                if (model.conclusion == Conclusion.Death && deathInspectionExist)
                {
                    return BadRequest(new { status = "400", message = "The patient already has a death certificate" });
                }

                inspection.anamnesis = model.anamnesis;
                inspection.complaints = model.complaints;
                inspection.treatment = model.treatment;
                inspection.conclusion = model.conclusion;
                inspection.nextVisitDate = model.nextVisitDay;
                inspection.deathDate = model.deathDate;

                _context.DiagnosisDataBaseModels.RemoveRange(inspection.diagnoses);

                foreach (var d in model.diagnosis)
                {
                    var icd10record = await _context.Icd10DataBaseModels.FirstOrDefaultAsync(i => i.id == d.icdDiagnosisId);
                    if (icd10record == null)
                    {
                        return BadRequest(new { status = "400", message = $"ICD-10 record with ID not found." });
                    }

                    var newDiagnosis = new DiagnosisDataBaseModel
                    {
                        id = Guid.NewGuid(),
                        createTime = DateTime.UtcNow,
                        code = icd10record.code,
                        name = icd10record.name,
                        description = d.description,
                        type = d.type,
                        inspectionId = inspection.id,
                        icd10Id = icd10record.id
                    };
                    await _context.DiagnosisDataBaseModels.AddAsync(newDiagnosis);
                }

                await _context.SaveChangesAsync();

                return Ok(new { status = "200", message = "Inspection updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "500", message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}/chain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InspectionChainModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ShowChainInspections(Guid id)
        {
            try
            {
                var authorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(authorId, out Guid authorGuid))
                {
                    return Unauthorized(new { status = "401", message = "Unauthorized access" });
                }

                var rootInspection = await _context.InspectionDataBaseModels
                    .Include(ins => ins.diagnoses)
                    .Include(ins => ins.patient)
                    .Include(ins => ins.doctor)
                    .FirstOrDefaultAsync(ins => ins.id == id && ins.hasChain);

                if (id == Guid.Empty)
                {
                    return BadRequest(new { status = "400", message = "Invalid inspection ID provided." });
                }

                if (rootInspection == null)
                {
                    return NotFound(new { status = "404", message = "Root inspection not found or is not a chain root." });
                }

                var inspectionChain = new List<InspectionDataBaseModel> { rootInspection };
                var currInspection = rootInspection;

                while (currInspection.hasNested)
                {
                    currInspection = await _context.InspectionDataBaseModels
                        .Include(ins => ins.diagnoses)
                        .Include(ins => ins.patient)
                        .Include(ins => ins.doctor)
                        .FirstOrDefaultAsync(ins => ins.previousInspectionId == currInspection.id);

                    if (currInspection != null)
                    {
                        inspectionChain.Add(currInspection);
                    }
                    else
                    {
                        break;
                    }
                }
                var inspectionModel = inspectionChain.Select(ins => new InspectionChainModel
                {
                    id = ins.id,
                    createTime = ins.createTime,
                    date = ins.date,
                    conclusion = ins.conclusion,
                    previousInspectionId = ins?.previousInspectionId,
                    doctorId = ins.doctor.id,
                    doctor = ins.doctor.name,
                    patientId = ins.patient.id,
                    patient = ins.patient.name,
                    hasChain = ins.hasChain,
                    hasNested = ins.hasNested,
                    diagnosis = ins.diagnoses.Select(d => new DiagnosisModel
                    {
                        id = d.id,
                        createTime = d.createTime,
                        code = d.code,
                        name = d.name,
                        description = d.description,
                        type = d.type
                    }).ToList()
                }).ToList();

                return Ok(new { status = "200", data = inspectionModel });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "500", message = ex.Message });
            }
        }
    }
}
