using medicalApp.Data;
using medicalApp.Models;
using medicalApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace medicalApp.Controllers
{
    [Route("api/dictionary")]
    [ApiController]
    public class Dictionary : ControllerBase
    {
        private readonly AppDataContext _context;

        public Dictionary(AppDataContext context)
        {
            _context = context;
        }

        [HttpGet("speciality")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SpecialtiesPagedListModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(object))]
        public async Task<IActionResult> GetSpeciality([FromQuery] string? searchString = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest(new { status = "400", message = "Invalid arguments for filtration/pagination" });
                }

                var query = _context.SpecialityModels.AsQueryable();

                if (!string.IsNullOrEmpty(searchString))
                {
                    query = query.Where(currElement => currElement.name.Contains(searchString));
                }

                var totalSpecialties = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalSpecialties / (double)pageSize);

                if (page > totalPages && totalPages > 0)
                {
                    return BadRequest(new { status = "400", message = "Invalid arguments for filtration/pagination" });
                }

                var specialties = await query
                    .OrderBy(spec => spec.name)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var specialtyShowModels = specialties.Select(s => new SpecialityShowModel
                {
                    id = s.id,
                    name = s.name
                }).ToList();

                var paginationInfoList = new PageInfoModel
                {
                    size = pageSize,
                    count = totalSpecialties,
                    current = page
                };

                var result = new SpecialtiesPagedListModel
                {
                    specialities = specialtyShowModels,
                    pagination = paginationInfoList
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "500", message = ex.Message });
            }
        }

        [HttpGet("icd10/roots")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Icd10Model))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(object))]
        public async Task<IActionResult> GetRoots()
        {
            try
            {
                var roots = await _context.Icd10DataBaseModels
                .Where(record => record.parentId == null)
                .Select(record => new Icd10Model
                {
                    id = record.id,
                    createTime = record.createTime,
                    code = record.code,
                    name = record.name
                })
                .ToListAsync();

                return Ok(roots);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "500", message = ex.Message });
            }
        }

        [HttpGet("icd10")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Icd10Model))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(object))]
        public async Task<IActionResult> SearchIcd10([FromQuery] string? searchString, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                if (page <= 0 || pageSize <= 0)
                {
                    return BadRequest(new { status = "400", message = "Invalid arguments for filtration/pagination" });
                }

                var query = _context.Icd10DataBaseModels.AsQueryable();

                if (!string.IsNullOrEmpty(searchString))
                {
                    query = _context.Icd10DataBaseModels.Where(i => i.code.Contains(searchString) || i.name.Contains(searchString));
                }

                var totalIcd = await query.CountAsync();
                var totalPages = (int)Math.Ceiling(totalIcd / (double)pageSize);

                if (page > totalPages && totalPages > 0)
                {
                    return BadRequest(new { status = "400", message = "Invalid arguments for filtration/pagination" });
                }

                var records = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(i => new Icd10Model
                    {
                        id = i.id,
                        createTime = i.createTime,
                        code = i.code,
                        name = i.name
                    })
                    .ToListAsync();

                var paginationInfoList = new PageInfoModel
                {
                    size = pageSize,
                    count = totalIcd,
                    current = page
                };

                var result = new Icd10SearchModel
                {
                    records = records,
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
