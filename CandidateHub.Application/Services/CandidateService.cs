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


        #region Helper Methods
        private static async Task<bool> EmailExistsAsync(string email, IGenericRepository<Candidate> repo)
        {
            Candidate? existing = await repo.FindAsync(c => c.Email == email);

            return existing != null;
        }

        private void InvalidateCache(string emailKey)
        {
            _cacheService.Remove($"candidate:{emailKey}");
            _cacheService.Remove("candidates:all");
        }

        private async Task<CandidateDto> CreateCandidateAsync(CandidateDto dto, string normalizedEmail, IGenericRepository<Candidate> repo)
        {
            Candidate candidateDb = dto.ToEntity();
            candidateDb.Email = normalizedEmail;
            candidateDb.CreatedAt = DateTime.UtcNow;

            await repo.AddAsync(candidateDb);
            await _unitOfWork.CommitAsync();

            InvalidateCache(normalizedEmail);

            return candidateDb.ToDto();
        }

        private async Task<CandidateDto> UpdateCandidateAsync(CandidateDto dto, string normalizedEmail, IGenericRepository<Candidate> repo)
        {
            Candidate? modelDb = await repo.FindAsync(c => c.Email == normalizedEmail);

            modelDb.FirstName = dto.FirstName;
            modelDb.LastName = dto.LastName;
            modelDb.PhoneNumber = dto.PhoneNumber;
            modelDb.CallTimePreference = dto.CallTimePreference;
            modelDb.LinkedInUrl = dto.LinkedInUrl;
            modelDb.GitHubUrl = dto.GitHubUrl;
            modelDb.Comments = dto.Comments;
            modelDb.UpdatedAt = DateTime.UtcNow;
            modelDb.Email = normalizedEmail;

            repo.Update(modelDb);
            await _unitOfWork.CommitAsync();

            InvalidateCache(normalizedEmail);

            return modelDb.ToDto();
        } 
        #endregion


        public async Task<CandidateDto> CreateOrUpdateCandidateAsync(CandidateDto dto)
        {
            string normalizedEmail = dto.Email.Trim().ToLower();
            
            var repo = _unitOfWork.Repository<Candidate>();

            bool exist =  await EmailExistsAsync(normalizedEmail, repo);

            if (!exist) // candidate not exists in db
            {
                return await CreateCandidateAsync(dto, normalizedEmail, repo);
            }
            else
            {
                return await UpdateCandidateAsync(dto, normalizedEmail, repo);
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
