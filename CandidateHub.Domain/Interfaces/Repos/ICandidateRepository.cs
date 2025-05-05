using CandidateHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateHub.Domain.Interfaces.Repos
{
    public interface ICandidateRepository : IGenericRepository<Candidate>
    {
        Task<Candidate?> GetByEmailAsync(string email); // adds a candidate-specific query over the generic repository.
    }
}
