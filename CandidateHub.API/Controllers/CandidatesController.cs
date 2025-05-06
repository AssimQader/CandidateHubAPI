using CandidateHub.API.Models;
using CandidateHub.Application.DTOs;
using CandidateHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CandidateHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatesController : ControllerBase
    {
        private readonly ICandidateService _candidateService;
        public CandidatesController(ICandidateService candidateService)
        {
            _candidateService = candidateService;
        }


        /// <summary>
        /// Create or update a candidate based on email.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CandidateDto>>> CreateOrUpdate([FromBody] CandidateDto candidateDto)
        {
            if (!ModelState.IsValid)
            {
                List<ValidationError> errors = ModelState
                    .Where(kvp => kvp.Value?.Errors.Count > 0)
                    .SelectMany(kvp =>
                        kvp.Value!.Errors.Select(e => new ValidationError
                        {
                            Field = kvp.Key,
                            Message = e.ErrorMessage
                        }))
                    .ToList();

                return BadRequest(ApiResponse<CandidateDto>.Failed("Validation failed", errors));
            }

            try
            {
                CandidateDto? result = await _candidateService.CreateOrUpdateCandidateAsync(candidateDto);
                return Ok(ApiResponse<CandidateDto>.Success(result, "Candidate saved successfully"));
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ApiResponse<CandidateDto>.Failed(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CandidateDto>.Failed($"An unexpected error occurred: {ex.Message}"));
            }
        }

        /// <summary>
        /// Get a candidate by email.
        /// </summary>
        [HttpGet("email/{email}")]
        public async Task<ActionResult<ApiResponse<CandidateDto>>> GetByEmail([FromRoute] string email)
        {
            try
            {
                CandidateDto? candidate = await _candidateService.GetCandidateByEmailAsync(email);
                if (candidate == null)
                    return NotFound(ApiResponse<CandidateDto>.Success(default, "Candidate not found!"));

                return Ok(ApiResponse<CandidateDto>.Success(candidate, "Candidate retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<CandidateDto>.Failed($"An unexpected error occurred : {ex.Message}"));
            }
        }


        /// <summary>
        /// Get all candidates.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CandidateDto>>>> GetAll()
        {
            try
            {
                IEnumerable<CandidateDto> candidates = await _candidateService.GetAllCandidatesAsync();
                
                if (candidates == null || !candidates.Any())
                    return NotFound(ApiResponse<IEnumerable<CandidateDto>>.Success(default, "No candidates found!"));   

                return Ok(ApiResponse<IEnumerable<CandidateDto>>.Success(candidates, "Candidates retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<IEnumerable<CandidateDto>>.Failed($"An unexpected error occurred : {ex.Message}"));
            }
        }
    }
}
