using CandidateHub.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace CandidateHub.Tests.Unit_Tests.Domain.Entities
{
    public class CandidateTests
    {
        private static List<ValidationResult> ValidateEntity(object entity)
        {
            ValidationContext context = new (entity, null, null);
            List<ValidationResult> results = [];
            Validator.TryValidateObject(entity, context, results, true);
           
            return results;
        }


        [Fact]
        public void Candidate_Should_Require_Email_And_Be_Valid_Format()
        {
            var candidate = new Candidate
            {
                FirstName = "Asem",
                LastName = "Adel",
                PhoneNumber = "1234567890",
                Email = "invalid-email-format"
            };

            List<ValidationResult> results = ValidateEntity(candidate);

            Assert.Contains(results, vr => vr.MemberNames.Contains("Email"));
        }


        [Fact]
        public void Candidate_Should_Reject_Invalid_PhoneNumber()
        {
            Candidate candidate = new() 
            {
                FirstName = "Asem",
                LastName = "Adel",
                Email = "a@a.com",
                PhoneNumber = "InvalidPhone123"
            };

            List<ValidationResult> results = ValidateEntity(candidate);

            Assert.Contains(results, r => r.MemberNames.Contains("PhoneNumber"));
        }

        [Fact]
        public void Candidate_Should_Pass_Validation_With_Valid_Data()
        {
            Candidate candidate = new()
            {
                FirstName = "Asem",
                LastName = "Adel",
                Email = "asem@microsoft.com",
                PhoneNumber = "+201061103073"
            };

            List<ValidationResult> results = ValidateEntity(candidate);

            Assert.Empty(results);
        }
    }
}
