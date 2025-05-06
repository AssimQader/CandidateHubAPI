using CandidateHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateHub.Infrastructure.Data
{
    public class CandidateSeeder
    {
        public static void Seed(CandidateHubDbContext context)
        {
            if (!context.Candidates.Any())
            {
                List<Candidate> candidates = new()
                {
                    new ()
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "Asem",
                        LastName = "Adel",
                        Email = "asem.adel00@gmail.com",
                        PhoneNumber = "+201061103073",
                        Comments = "NA",
                        CreatedAt = DateTime.UtcNow
                    },
                    new ()
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "Hadeer",
                        LastName = "Adam",
                        Email = "hadeer.adam@gmail.com",
                        PhoneNumber = "+201000911876",
                        Comments = "NA",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                context.Candidates.AddRange(candidates);
                context.SaveChanges();
            }
        }
    }
}
