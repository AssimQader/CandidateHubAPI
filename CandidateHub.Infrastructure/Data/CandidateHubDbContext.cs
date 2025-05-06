using CandidateHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CandidateHub.Infrastructure.Data
{
    public class CandidateHubDbContext : DbContext
    {
        public CandidateHubDbContext(DbContextOptions<CandidateHubDbContext> options) : base(options) { }


        public DbSet<Candidate> Candidates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Candidate>().HasIndex(c => c.Email).IsUnique(); // adds a non-clustered unique index


            modelBuilder.Entity<Candidate>().Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(150);

            base.OnModelCreating(modelBuilder);
        }
    }
}
