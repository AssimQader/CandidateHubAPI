using CandidateHub.Domain.Entities;
using CandidateHub.Domain.Interfaces.Repos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateHub.Infrastructure.Repos
{
    public class CandidateRepository : GenericRepository<Candidate>, ICandidateRepository
    {
        private new readonly DbContext _context;

        public CandidateRepository(DbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Candidate?> GetByEmailAsync(string email)
        {
            return await _context.Set<Candidate>()
                .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
        }
    }
}
