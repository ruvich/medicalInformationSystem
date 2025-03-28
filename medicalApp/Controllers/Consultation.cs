using medicalApp.Data;
using medicalApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace medicalApp.Controllers
{
    [Route("api/consultation")]
    [ApiController]
    public class Consultation : ControllerBase
    {
        private readonly AppDataContext _context;

        public Consultation(AppDataContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InspectionPagedListModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetConsultations(
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

                var doctor = await _context.DoctorModels
                    .Include(d => d.speciality)
                    .FirstOrDefaultAsync(d => d.id == Guid.Parse(doctorId));

                if (doctor == null)
                {
                    return NotFound(new { status = "404", message = "Doctor not found" });
                }

                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest(new { status = "400", message = "Invalid arguments for pagination" });
                }

                List<Guid> icdRootIds = new List<Guid>();
                if (icdRoots != null && icdRoots.Any())
                {
                    icdRootIds = await _context.Icd10DataBaseModels
                        .Where(icd => icdRoots.Select(r => Guid.Parse(r)).Contains(icd.id) && icd.parentId == null)
                        .Select(icd => icd.id)
                        .ToListAsync();

                    if (!icdRootIds.Any())
                    {
                        return BadRequest(new { status = "400", message = "Invalid ICD-10 roots" });
                    }
                }

                var inspectionQuery = _context.InspectionDataBaseModels
                    .Include(ins => ins.consultations)
                        .ThenInclude(c => c.speciality)
                    .Include(ins => ins.diagnoses)
                    .Include(ins => ins.doctor)
                    .Include(ins => ins.patient)
                    .Where(ins => ins.consultations.Any(c => c.specialityId == doctor.specialityId) || ins.doctorID == doctor.id);

                if (grouped)
                {
                    inspectionQuery = inspectionQuery.Where(ins => ins.previousInspectionId == null);
                }

                List<InspectionDataBaseModel> inspections;
                if (icdRootIds.Any())
                {
                    inspections = await inspectionQuery.ToListAsync();

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

                    inspections = inspections
                        .Where(ins => ins.diagnoses.Any(d => belongsToRoot(d.icd10Id) && d.type == DiagnosisType.Main))
                        .ToList();
                }
                else
                {
                    inspections = await inspectionQuery.ToListAsync();
                }

                var inspectionModels = inspections
                    .Select(ins => new InspectionChainModel
                    {
                        id = ins.id,
                        createTime = ins.createTime,
                        date = ins.date,
                        conclusion = ins.conclusion,
                        previousInspectionId = ins.previousInspectionId,
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
                    })
                    .ToList();

                var paginatedInspections = inspectionModels
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var allInspectionsCount = inspectionModels.Count;

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


        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ConsultationModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetConsultationDetails(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid consultationId))
                {
                    return BadRequest(new { status = "400", message = "Invalid ID format. Value must be a valid GUID." });
                }

                var consultation = await _context.ConsultationsDataBaseModels
                    .Include(c => c.comments)
                        .ThenInclude(comment => comment.author)
                            .ThenInclude(author => author.speciality)
                    .Include(c => c.speciality)
                    .Include(c => c.inspection)
                        .ThenInclude(i => i.doctor)
                    .FirstOrDefaultAsync(c => c.id == consultationId);

                if (consultation == null)
                {
                    return NotFound(new { status = "404", message = "Inspection not found" });
                }

                var consultationModel = new ConsultationModel
                {
                    id = consultationId,
                    createTime = consultation.createTime,
                    inspectionId = consultation.inspectionId,
                    speciality = new SpecialityShowModel
                    {
                        id = consultation.speciality.id,
                        name = consultation.speciality.name
                    },
                    comments = consultation.comments.Select(c => new CommentModel
                    {
                        id = c.id,
                        createTime = c.createTime,
                        modifyTime = c.modifyTime,
                        content = c.content,
                        authorId = c.authorId,
                        author = c.author.speciality.name,
                        parentId = c.parentId
                    }).ToList(),
                };
                return Ok(consultationModel);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "500", message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("{id}/comment")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CommentCreate(string id, [FromBody] CommentCreateModel model)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid consultationId))
                {
                    return BadRequest(new { status = "400", message = "Invalid ID format. Value must be a valid GUID." });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { status = "400", message = "Invalid arguments" });
                }

                var consultation = await _context.ConsultationsDataBaseModels
                    .Include(c => c.inspection)
                    .FirstOrDefaultAsync(c => c.id == consultationId);

                if (consultation == null)
                {
                    return NotFound(new { status = "404", message = "Consultation not found" });
                }


                var authorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(authorId, out Guid authorGuid))
                {
                    return Unauthorized(new { status = "401", message = "Unauthorized access" });
                }

                var author = await _context.DoctorModels
                    .Include(m => m.speciality)
                    .FirstOrDefaultAsync(m => m.id == authorGuid);

                if (author == null)
                {
                    return Unauthorized(new { status = "401", message = "Unauthorized access" });
                }

                if (author.specialityId != consultation.specialityId && consultation.inspection.doctorID != authorGuid)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new { status = "403", message = "User doesn't have add comment to consultation (unsuitable specialty and not the inspection author)" });
                }

                var comment = new CommentDataBaseModel
                {
                    id = Guid.NewGuid(),
                    createTime = DateTime.UtcNow,
                    modifyTime = DateTime.UtcNow,
                    content = model.content,
                    authorId = authorGuid,
                    author = author,
                    authorName = author.speciality.name,
                    parentId = model.parentId != Guid.Empty ? model.parentId : null,
                    consultationId = consultationId,
                };

                _context.CommentDataBaseModels.Add(comment);
                await _context.SaveChangesAsync();

                return Ok(comment.id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "500", message = ex.Message, details = ex.InnerException?.Message });
            }
        }

        [Authorize]
        [HttpPut("comment/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CommentEdit(string id, [FromBody] CommentEditModel model)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid commentId))
                {
                    return BadRequest(new { status = "400", message = "Invalid ID format. Value must be a valid GUID." });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new { status = "400", message = "Invalid arguments" });
                }

                var comment = await _context.CommentDataBaseModels
                    .FirstOrDefaultAsync(c => c.id == commentId);

                if (comment == null)
                {
                    return NotFound(new { status = "404", message = "Comment not found" });
                }


                var authorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(authorId, out Guid authorGuid))
                {
                    return Unauthorized(new { status = "401", message = "Unauthorized access" });
                }
                
                if (comment.authorId != authorGuid)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new { status = "403", message = "User is not the author of the comment" });
                }

                comment.content = model.content;
                comment.modifyTime = DateTime.UtcNow;

                _context.CommentDataBaseModels.Update(comment);
                await _context.SaveChangesAsync();

                return Ok(new {status = "200", message = "The comment was updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "500", message = ex.Message});
            }
        }
    }
}
