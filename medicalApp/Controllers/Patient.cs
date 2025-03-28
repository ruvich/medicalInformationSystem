using medicalApp.Data;
using medicalApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using medicalApp.Data;
using medicalApp.Models;
using medicalApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq;
using System.Numerics;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace medicalApp.Controllers
{
    [Route("api/patient")]
    [ApiController]
    public class Patient : ControllerBase
    {
        private readonly AppDataContext _context;

        public Patient(AppDataContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegistrationPatient([FromBody] PatientCreateModel model)
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

                var doctor = await _context.DoctorModels.FirstOrDefaultAsync(m => m.id.ToString() == doctorId);

                if (doctor == null)
                {
                    return NotFound(new { status = "404", message = "Doctor not found" });
                }

                if (model.birthday >= DateTime.UtcNow)
                {
                    return BadRequest(new { status = "400", message = "Birthday cannot be in the future." });
                }

                else
                {
                    var patient = new PatientModel {
                        id = Guid.NewGuid(),
                        createTime = DateTime.UtcNow,
                        name = model.name,
                        birthday = model.birthday,
                        gender = model.gender,
                    };
                    _context.PatientModels.Add(patient);
                    await _context.SaveChangesAsync();

                    return Ok(patient.id);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "500", message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatientPagedListModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPatients(
            [FromQuery] string? name = null,
            [FromQuery] Conclusion? conclusions = null,
            [FromQuery] PatientSorting sorting = PatientSorting.CreateAsc,
            [FromQuery] bool? scheduledVisits = false,
            [FromQuery] bool? onlyMine = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5)
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest(new { status = "400", message = "Invalid arguments for filtration/pagination" });
                }

                var query = _context.PatientModels.AsQueryable();

                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(currP => currP.name.Contains(name));
                }

                if (conclusions.HasValue)
                {
                    query = query.Where(currP => currP.inspections.Any(ins => ins.conclusion == conclusions));
                }

                if (onlyMine == true)
                {
                    var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (Guid.TryParse(doctorId, out var parseDoctorId))
                    {
                        query = query.Where(currP => currP.inspections.Any(ins => ins.doctorID == parseDoctorId));
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }

                if (scheduledVisits == true)
                {
                    query = query.Where(currP => currP.inspections.Any(ins => ins.nextVisitDate != null));
                }

                query = sorting switch
                {
                    PatientSorting.NameAsc => query.OrderBy(currP => currP.name),
                    PatientSorting.NameDesc => query.OrderByDescending(currP => currP.name),
                    PatientSorting.CreateAsc => query.OrderBy(currP => currP.createTime),
                    PatientSorting.CreateDesc => query.OrderByDescending(currP => currP.createTime),
                    PatientSorting.InspectionAsc => query.OrderBy(currP => currP.inspections.Max(ins => ins.date)),
                    PatientSorting.InspectionDesc => query.OrderByDescending(currP => currP.inspections.Max(ins => ins.date)),
                    _ => query
                };

                var totalPatients = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalPatients / (double)pageSize);

                if (page > totalPages && totalPages > 0)
                {
                    return BadRequest(new { status = "400", message = "Invalid arguments for filtration/pagination" });
                }

                var patients = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var patientShowModels = patients.Select(p => new PatientShowModel
                {
                    id = p.id,
                    createTime = p.createTime,
                    name = p.name,
                    birthday = p.birthday,
                    gender = p.gender

                }).ToList();

                var paginationInfoList = new PageInfoModel
                {
                    size = pageSize,
                    count = totalPatients,
                    current = page
                };

                var result = new PatientPagedListModel
                {
                    patients = patientShowModels,
                    pagination = paginationInfoList
                };

                return Ok(result);
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "500", message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("{id}/inspections")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePatientInspection(Guid id, [FromBody] InspectionCreateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { status = "400", message = "Invalid arguments" });
                }

                var deathInspectionExist = await _context.InspectionDataBaseModels
                    .AnyAsync(ins => ins.patientID == id && ins.conclusion == Conclusion.Death);

                if (deathInspectionExist)
                {
                    return BadRequest(new { status = "400", message = "Patient has already died." });
                }

                if (model.date > DateTime.UtcNow)
                {
                    return BadRequest(new { status = "400", message = "Inspection date cannot be in the future" });
                }

                if (model.nextVisitDay.HasValue && model.nextVisitDay.Value <= model.date)
                {
                    return BadRequest(new { status = "400", message = "The next inspection date must be later than the current inspection date." });
                }

                if (model.deathDate != null && model.deathDate > DateTime.UtcNow)
                {
                    return BadRequest(new { status = "400", message = "Death date cannot be in the future." });
                }

                var previousInspection = await _context.InspectionDataBaseModels
                    .Where(currIns => currIns.patientID == id && currIns.id == model.previousInspectionId)
                    .OrderByDescending(currIns => currIns.date)
                    .FirstOrDefaultAsync();

                if (previousInspection != null && model.date < previousInspection.date)
                {
                    return BadRequest(new { status = "400", message = "Inspection cannot be done before the previous inspection in the chain." });
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

                var specialities = new HashSet<Guid>();
                if (model.consultations != null)
                {
                    foreach (var consultation in model.consultations)
                    {
                        if (!specialities.Add(consultation.specialityId))
                        {
                            return BadRequest(new { status = "400", message = "Duplicate speciality in consultations" });
                        }

                        if (string.IsNullOrWhiteSpace(consultation.comment.content))
                        {
                            return BadRequest(new { status = "400", message = "Consultation must have a comment" });
                        }
                    }
                }

                var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(doctorId))
                {
                    return Unauthorized();
                }

                var doctor = await _context.DoctorModels.FirstOrDefaultAsync(d => d.id == Guid.Parse(doctorId));
                if (doctor == null)
                {
                    return NotFound("Doctor not found.");
                }

                var inspectionId = Guid.NewGuid();
                Guid? baseInspectionId = null;
                bool hasChain = false;
                bool hasNested = false;

                if (model.previousInspectionId.HasValue)
                {
                    var repeatParent = await _context.InspectionDataBaseModels
                        .FirstOrDefaultAsync(ins => ins.previousInspectionId == model.previousInspectionId);

                    if (repeatParent != null)
                    {
                        return BadRequest(new { status = "400", message = "This inspection already has a child element" });
                    }

                    var parentInspection = await _context.InspectionDataBaseModels
                        .Where(ins => ins.id == model.previousInspectionId.Value)
                        .FirstOrDefaultAsync();

                    if (parentInspection != null)
                    {
                        var currentInspection = parentInspection;
                        while (currentInspection.previousInspectionId.HasValue)
                        {
                            var potentialRoot = await _context.InspectionDataBaseModels
                                .FirstOrDefaultAsync(ins => ins.id == currentInspection.previousInspectionId.Value);
                            if (potentialRoot != null)
                            {
                                currentInspection = potentialRoot;
                            }
                            else
                            {
                                break;
                            }
                        }

                        baseInspectionId = currentInspection.id;

                        parentInspection.hasNested = true;
                        if (parentInspection.previousInspectionId == null)
                        {
                            parentInspection.hasChain = true;
                        }

                        _context.InspectionDataBaseModels.Update(parentInspection);
                    }
                    else
                    {
                        return NotFound("Parent inspection is not found.");
                    }

                }
                else
                { 
                    baseInspectionId = null;
                }
                

                var inspection = new InspectionDataBaseModel
                {
                    id = inspectionId,
                    createTime = DateTime.UtcNow,
                    date = model.date,
                    anamnesis = model.anamnesis,
                    complaints = model.complaints,
                    treatment = model.treatment,
                    conclusion = model.conclusion,
                    nextVisitDate = model.nextVisitDay,
                    deathDate = model.deathDate,
                    previousInspectionId = model.previousInspectionId,
                    baseInspectionId = baseInspectionId,
                    hasChain = hasChain,
                    hasNested = hasNested,
                    patientID = id,
                    doctorID = Guid.Parse(doctorId),
                    diagnoses = model.diagnosis.Select(d =>
                    {
                        var icd10record = _context.Icd10DataBaseModels.FirstOrDefault(i => i.id == d.icdDiagnosisId);

                        return new DiagnosisDataBaseModel
                        {
                            id = Guid.NewGuid(),
                            createTime = DateTime.UtcNow,
                            code = icd10record.code,
                            name = icd10record.name,
                            description = d.description,
                            type = d.type,
                            inspectionId = inspectionId,
                            icd10Id = icd10record.id
                        };
                    }).ToList(),
                    consultations = model.consultations != null && model.consultations.Any()
                        ? model.consultations.Select(consultation =>
                        {
                        var consultationId = Guid.NewGuid();
                        var newConsultation = new ConsultationDataBaseModel
                        {
                            id = consultationId,
                            createTime = DateTime.UtcNow,
                            specialityId = consultation.specialityId,
                            inspectionId = inspectionId,
                            comments = new List<CommentDataBaseModel>
                        {
                            new CommentDataBaseModel
                            {
                                id = Guid.NewGuid(),
                                createTime = DateTime.UtcNow,
                                content = consultation.comment.content,
                                modifyTime = DateTime.UtcNow,
                                authorId = Guid.Parse(doctorId),
                                authorName = doctor.name,
                                consultationId = consultationId,
                                parentId = null
                            }
                        }
                    };

                    return newConsultation;
                    }).ToList() : new List<ConsultationDataBaseModel>()
                };

                _context.InspectionDataBaseModels.Add(inspection);
                await _context.SaveChangesAsync();
                return Ok(inspection.id);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "500", message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PatientShowModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPatient(Guid id)
        {
            try
            {

                var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(doctorId))
                {
                    return Unauthorized(new { status = "401", message = "Unauthorized access" });
                }

                var patientResult = await _context.PatientModels.FirstOrDefaultAsync(p => p.id == id);
                if (patientResult == null)
                {
                    return NotFound(new { status = "404", message = "Patient not found" });

                }
                var showPatient = new PatientShowModel
                {
                    id = patientResult.id,
                    createTime = patientResult.createTime,
                    name = patientResult.name,
                    birthday = patientResult.birthday,
                    gender = patientResult.gender
                };
                return Ok(showPatient);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "500", message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}/inspections/search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<InspectionShortModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPatientInspections(Guid id, [FromQuery] string? searchString = null)
        {
            try
            {

                var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(doctorId))
                {
                    return Unauthorized(new { status = "401", message = "Unauthorized access" });
                }

                var currPatient = await _context.PatientModels.AnyAsync(p => p.id == id);
                if (currPatient == null)
                {
                    return NotFound(new { status = "404", message = "No patients found" });
                }

                var query = _context.InspectionDataBaseModels.AsQueryable();

                query = query.Where(ins => ins.patientID == id);
                if (!string.IsNullOrEmpty(searchString))
                {
                    query = query.Where(ins => ins.diagnoses
                        .Any(d => d.name.Contains(searchString) || d.code.Contains(searchString)));
                }

                var inspections = await query
                    .Select(ins => new InspectionShortModel
                    {
                        id = ins.id,
                        createTime = ins.createTime,
                        date = ins.date,
                        diagnosis = ins.diagnoses.Select(d => new DiagnosisModel
                        {
                            id = d.id,
                            createTime = d.createTime,
                            name = d.name,
                            code = d.code,
                            description = d.description,
                            type = d.type
                        }).ToList()
                    })
                    .ToListAsync();

                if (inspections.Count == 0)
                {
                    return NotFound(new { status = "404", message = "No inspections found" });
                }

                return Ok(inspections);
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "500", message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}/inspections")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<InspectionShortModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPatientInspections (
            Guid id,
            [FromQuery] bool grouped = false,
            [FromQuery] List<string> icdRoots = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5)
        {
            try
            {
                var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(doctorId))
                {
                    return Unauthorized(new { status = "401", message = "Unauthorized access" });
                }

                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest(new { status = "400", message = "Invalid arguments for filtration/pagination" });
                }

                if (id == Guid.Empty)
                {
                    return BadRequest(new { status = "400", message = "Invalid patient ID provided." });
                }

                var currPatient = await _context.PatientModels.AnyAsync(p => p.id == id);
                if (currPatient == null)
                {
                    return NotFound(new { status = "404", message = "No patients found" });
                }

                var inspectionsQuery = _context.InspectionDataBaseModels
                    .Include(ins => ins.diagnoses)
                    .Include(ins => ins.patient)
                    .Include(ins => ins.doctor)
                    .AsQueryable();

                inspectionsQuery = inspectionsQuery.Where(ins => ins.patientID == id);

                var allInspections = await inspectionsQuery.ToListAsync();
                List<InspectionChainModel> inspectionModels = new List<InspectionChainModel>();

                var rootInspections = allInspections.Where(ins => ins.hasChain).ToList();
                var nonChainInspections = allInspections.Where(ins => !ins.hasChain).ToList();
                if (grouped)
                {
                    foreach (var rootInspection in rootInspections)
                    {
                        var inspectionChain = new List<InspectionDataBaseModel> { rootInspection };
                        var currInspection = rootInspection;

                        while (currInspection.hasNested)
                        {
                            currInspection = await _context.InspectionDataBaseModels
                                .Include(ins => ins.diagnoses)
                                .Include(ins => ins.patient)
                                .Include(ins => ins.doctor)
                                .AsQueryable()
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

                        inspectionModels.AddRange(inspectionChain.Select(ins => new InspectionChainModel
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
                                type = d.type,
                                icd10Id = d.icd10Id
                            }).ToList()
                        }));
                    }

                    inspectionModels.AddRange(nonChainInspections.Select(ins => new InspectionChainModel
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
                            type = d.type,
                            icd10Id = d.icd10Id
                        }).ToList()
                    }));
                }
                else
                {
                    inspectionModels.AddRange(allInspections.Select(ins => new InspectionChainModel
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
                            type = d.type,
                            icd10Id = d.icd10Id
                        }).ToList()
                    }));
                }

                inspectionModels = inspectionModels.DistinctBy(ins => ins.id).ToList();

                if (icdRoots != null && icdRoots.Any())
                {
                    var icdRootIds = await _context.Icd10DataBaseModels
                        .Where(icd => icdRoots.Select(r => Guid.Parse(r)).Contains(icd.id) && icd.parentId == null)
                        .Select(icd => icd.id)
                        .ToListAsync();

                    if (!icdRootIds.Any())
                    {
                        return BadRequest(new { status = "400", message = "Invalid arguments" });
                    }

                    bool belongsToRoot(Guid? icd10Id)
                    {
                        while (icd10Id.HasValue)
                        {
                            if (icdRootIds.Contains(icd10Id.Value))
                            {
                                return true;
                            }

                            icd10Id = _context.Icd10DataBaseModels
                                .Where(icd => icd.id == icd10Id.Value)
                                .Select(icd => icd.parentId)
                                .FirstOrDefault();
                        }
                        return false;
                    }

                    inspectionModels = inspectionModels
                        .Where(ins => ins.diagnosis.Any(d => belongsToRoot(d.icd10Id) && d.type == DiagnosisType.Main))
                        .ToList();
                }


                var paginatedInspections = inspectionModels
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var allInspectionsCount = inspectionModels.Count();

                var paginationInfoList = new PageInfoModel
                {
                    size = pageSize,
                    count = allInspectionsCount,
                    current = page
                };

                var result = new InspectionPagedListModel
                {
                    inspections = paginatedInspections,
                    pagination = paginationInfoList
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "500", message = ex.Message });
            }
        }
    }
}
