using CandidateHub.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateHub.Application.Interfaces.Services
{
    public interface ICandidateService
    {
        Task<CandidateDto> CreateOrUpdateCandidateAsync(CandidateDto dto);
        Task<CandidateDto?> GetCandidateByEmailAsync(string email);
        Task<IEnumerable<CandidateDto>> GetAllCandidatesAsync();
    }
}
