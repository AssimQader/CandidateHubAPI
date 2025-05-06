using CandidateHub.Application.DTOs;
using CandidateHub.Application.Interfaces.Services;
using CandidateHub.Application.Mappings;
using CandidateHub.Domain.Entities;
using CandidateHub.Domain.Interfaces.Repos;
using CandidateHub.Domain.Interfaces.Services;

namespace CandidateHub.Application.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public CandidateService(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<CandidateDto> CreateOrUpdateCandidateAsync(CandidateDto dto)
        {
            try
            {
                string normalizedEmail = dto.Email.Trim().ToLower();
                IGenericRepository<Candidate> repo = _unitOfWork.Repository<Candidate>();
                Candidate? modelDb = await repo.FindAsync(c => c.Email.ToLower() == normalizedEmail);

                if (modelDb is null)
                {
                    var entity = dto.ToEntity();
                    entity.Email = normalizedEmail;
                    entity.CreatedAt = DateTime.UtcNow;

                    await repo.AddAsync(entity);
                    await _unitOfWork.CommitAsync();

                    // invalidate cache as new candidate is added
                    _cacheService.Remove($"candidate:{normalizedEmail}");
                    _cacheService.Remove("candidates:all");

                    return entity.ToDto();
                }
                else
                {
                    modelDb.FirstName = dto.FirstName;
                    modelDb.LastName = dto.LastName;
                    modelDb.PhoneNumber = dto.PhoneNumber;
                    modelDb.CallTimePreference = dto.CallTimePreference;
                    modelDb.LinkedInUrl = dto.LinkedInUrl;
                    modelDb.GitHubUrl = dto.GitHubUrl;
                    modelDb.Comments = dto.Comments;
                    modelDb.UpdatedAt = DateTime.UtcNow;

                    repo.Update(modelDb);
                    await _unitOfWork.CommitAsync();

                    // invalidate cache as an old candidat is updated
                    _cacheService.Remove($"candidate:{normalizedEmail}");
                    _cacheService.Remove("candidates:all");

                    return modelDb.ToDto();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error occurred while processing the candidate record : {ex.Message}");
            }
        }


        public async Task<CandidateDto?> GetCandidateByEmailAsync(string email)
        {
            try
            {
                string normalizedEmail = email.Trim().ToLower();
                string cacheKey = $"candidate:{normalizedEmail}";

                var cached = await _cacheService.GetAsync<CandidateDto>(cacheKey);
                if (cached != null)
                    return cached;


                Candidate? modelDb = await _unitOfWork.Repository<Candidate>()
                    .FindAsync(c => c.Email.ToLower() == normalizedEmail);


                if (modelDb == null)
                {
                    CandidateDto emptyDto = new();
                    await _cacheService.SetAsync(cacheKey, emptyDto, TimeSpan.FromMinutes(5));
                    return emptyDto;
                }

                CandidateDto dto = modelDb.ToDto();
                await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));
                
                return dto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to fetch candidate information : {ex.Message}");
            }
        }


        public async Task<IEnumerable<CandidateDto>?> GetAllCandidatesAsync()
        {
            try
            {
                string cacheKey = "candidates:all";
                var cached = await _cacheService.GetAsync<IEnumerable<CandidateDto>>(cacheKey);
                if (cached != null)
                    return cached;

                IEnumerable<Candidate>? candidates = await _unitOfWork.Repository<Candidate>().GetAllAsync();

                if (!candidates.Any())
                {
                    // cache and return an empty list
                    List<CandidateDto> emptyList = new();
                    await _cacheService.SetAsync(cacheKey, emptyList, TimeSpan.FromMinutes(10));
                    return emptyList;
                }


                List<CandidateDto> dtoList = candidates.Select(c => c.ToDto()).ToList();
                await _cacheService.SetAsync(cacheKey, dtoList, TimeSpan.FromMinutes(10));
                
                return dtoList;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to retrieve candidate list : {ex.Message}");
            }
        }
    }
}
