using CandidateHub.Application.DTOs;
using CandidateHub.Application.Services;
using CandidateHub.Domain.Entities;
using CandidateHub.Domain.Enums;
using CandidateHub.Domain.Interfaces.Repos;
using Moq;

namespace CandidateHub.Tests.Unit_Tests.Application.Services
{
    public class CandidateServiceTests
    {
        private readonly Mock<IGenericRepository<Candidate>> _candidateRepoMock;
        
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CandidateService _candidateService;

        public CandidateServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _candidateRepoMock = new Mock<IGenericRepository<Candidate>>();

            _unitOfWorkMock.Setup(u => u.Repository<Candidate>())
                           .Returns(_candidateRepoMock.Object);

            _candidateService = new CandidateService(_unitOfWorkMock.Object);
        }


        [Fact]
        public async Task CreateOrUpdateCandidateAsync_ShouldCreateNewCandidate_WhenEmailDoesNotExist()
        {
            // Arrange
            CandidateDto dto = new()
            {
                FirstName = "Hadeer",
                LastName = "Adam",
                Email = "test@example.com",
                PhoneNumber = "123456789",
                Comments = "Test comment",
                CallTimePreference = TimeIntervalPreference.Morning,
                GitHubUrl = "Test GitHubUrl",
                LinkedInUrl = "Test LinkedInUrl"
            };

            _candidateRepoMock.Setup(grc => grc.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Candidate, bool>>>()))
                               .ReturnsAsync((Candidate?)null);

            _candidateRepoMock.Setup(grc => grc.AddAsync(It.IsAny<Candidate>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);


            // Act
            CandidateDto result = await _candidateService.CreateOrUpdateCandidateAsync(dto);


            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Email.ToLowerInvariant(), result.Email.ToLowerInvariant());
        }



        [Fact]
        public async Task CreateOrUpdateCandidateAsync_ShouldUpdateCandidate_WhenEmailExists()
        {
            // Arrange
            CandidateDto dto = new()
            {
                FirstName = "Hassan",
                LastName = "Kholy",
                Email = "exists@example.com",
                PhoneNumber = "987654321",
                Comments = "Test comment",
                CallTimePreference = TimeIntervalPreference.Evening,
                GitHubUrl = "Test GitHubUrl",
                LinkedInUrl = "Test LinkedInUrl"
            };

            Candidate existing = new ()
            {
                Email = "exists@example.com"
            };

            _candidateRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Candidate, bool>>>()))
                               .ReturnsAsync(existing);

            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);


            // Act
            CandidateDto result = await _candidateService.CreateOrUpdateCandidateAsync(dto);


            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Email.ToLowerInvariant(), result.Email.ToLowerInvariant());
        }


        [Fact]
        public async Task GetCandidateByEmailAsync_ShouldReturnCandidate_WhenExists()
        {
            Candidate candidate = new ()
            {
                Email = "asem.adel00@gmail.com"
            };

            _candidateRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Candidate, bool>>>()))
                               .ReturnsAsync(candidate);

            CandidateDto? result = await _candidateService.GetCandidateByEmailAsync("asem.adel00@gmail.com");

            Assert.NotNull(result);
            Assert.Equal(candidate.Email.ToLowerInvariant(), result.Email.ToLowerInvariant());
        }


        [Fact]
        public async Task GetCandidateByEmailAsync_ShouldReturnNull_WhenNotFound()
        {
            _candidateRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Candidate, bool>>>()))
                               .ReturnsAsync((Candidate?)null);

            CandidateDto? result = await _candidateService.GetCandidateByEmailAsync("notfound@email.com");

            Assert.Null(result);
        }


        [Fact]
        public async Task GetAllCandidatesAsync_ShouldReturnList_WhenDataExists()
        {
            List<Candidate> list =
            [
                new Candidate { FirstName = "Test1", Email = "test1@gmail.com" },
                new Candidate { FirstName = "Test2", Email = "test2@gmail.com" }
            ];

            _candidateRepoMock.Setup(grc => grc.GetAllAsync())
                               .ReturnsAsync(list);

            IEnumerable<CandidateDto>? result = await _candidateService.GetAllCandidatesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }


        [Fact]
        public async Task GetAllCandidatesAsync_ShouldReturnNull_WhenNoData()
        {
            _candidateRepoMock.Setup(r => r.GetAllAsync())
                               .ReturnsAsync(new List<Candidate>());

            IEnumerable<CandidateDto>? result = await _candidateService.GetAllCandidatesAsync();

            Assert.Null(result);
        }
    }
}
