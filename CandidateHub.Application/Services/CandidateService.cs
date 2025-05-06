using CandidateHub.Application.DTOs;
using CandidateHub.Application.Interfaces.Services;
using CandidateHub.Application.Mappings;
using CandidateHub.Domain.Entities;
using CandidateHub.Domain.Interfaces.Repos;

namespace CandidateHub.Application.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CandidateService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CandidateDto> CreateOrUpdateCandidateAsync(CandidateDto dto)
        {
            try
            {
                string normalizedEmail = dto.Email.Trim().ToLowerInvariant();
                IGenericRepository<Candidate> repo = _unitOfWork.Repository<Candidate>();
                Candidate? modelDb = await repo.FindAsync(c => c.Email.ToLowerInvariant() == normalizedEmail);

                if (modelDb is null)
                {
                    var entity = dto.ToEntity();
                    entity.Email = normalizedEmail;
                    entity.CreatedAt = DateTime.UtcNow;

                    await repo.AddAsync(entity);
                    await _unitOfWork.CommitAsync();

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
                string normalizedEmail = email.Trim().ToLowerInvariant();
                Candidate? modelDb = await _unitOfWork.Repository<Candidate>()
                    .FindAsync(c => c.Email.ToLowerInvariant() == normalizedEmail);

                if (modelDb == null)
                {
                    return null;
                }

                return modelDb.ToDto();
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
                IEnumerable<Candidate>? candidates = await _unitOfWork.Repository<Candidate>().GetAllAsync();
                if (!candidates.Any())
                {
                    return null;
                }

                return candidates.Select(c => c.ToDto());
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to retrieve candidate list : {ex.Message}");
            }
        }
    }
}
