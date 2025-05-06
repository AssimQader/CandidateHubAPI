using CandidateHub.Application.DTOs;
using CandidateHub.Application.Services;
using CandidateHub.Domain.Entities;
using CandidateHub.Domain.Enums;
using CandidateHub.Domain.Interfaces.Repos;
using CandidateHub.Domain.Interfaces.Services;
using Moq;
using System.Linq.Expressions;

namespace CandidateHub.Tests.Unit_Tests.Application.Services
{
    public class CandidateServiceTests
    {
        private readonly Mock<IGenericRepository<Candidate>> _candidateRepoMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        
        private readonly CandidateService _candidateService;

        public CandidateServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _candidateRepoMock = new Mock<IGenericRepository<Candidate>>();
            _cacheServiceMock = new Mock<ICacheService>();

            _unitOfWorkMock.Setup(u => u.Repository<Candidate>())
                           .Returns(_candidateRepoMock.Object);

            _candidateService = new CandidateService(_unitOfWorkMock.Object, _cacheServiceMock.Object);
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
            Assert.Equal(dto.Email.ToLower(), result.Email.ToLower());
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
            Assert.Equal(dto.Email.ToLower(), result.Email.ToLower());
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
            Assert.Equal(candidate.Email.ToLower(), result.Email.ToLower());
        }


        [Fact]
        public async Task GetCandidateByEmailAsync_ShouldReturnEmptyCandidateDto_WhenNotFound()
        {
            _candidateRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Candidate, bool>>>()))
                               .ReturnsAsync((Candidate?)null);

            CandidateDto? result = await _candidateService.GetCandidateByEmailAsync("notfound@email.com");

            Assert.NotNull(result);
            Assert.True(string.IsNullOrWhiteSpace(result.FirstName));
            Assert.True(string.IsNullOrWhiteSpace(result.LastName));
            Assert.True(string.IsNullOrWhiteSpace(result.Email));
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
            //Assert.NotEmpty(result);
        }


        [Fact]
        public async Task GetAllCandidatesAsync_ShouldReturnEmptyList_WhenNoData()
        {
            _candidateRepoMock.Setup(r => r.GetAllAsync())
                               .ReturnsAsync(new List<Candidate>());

            IEnumerable<CandidateDto>? result = await _candidateService.GetAllCandidatesAsync();

            Assert.NotNull(result); // ensures method never returns null
            Assert.Empty(result);  // verifies the list is empty
        }



        [Fact]
        public async Task CreateOrUpdateCandidateAsync_ShouldInvalidateCache_AfterInsert()
        {

            CandidateDto dto = new ()
            {
                Email = "insert@test.com",
                FirstName = "Insert",
                LastName = "Test",
                PhoneNumber = "123",
            };

            _candidateRepoMock.Setup(grc => grc.FindAsync(It.IsAny<Expression<Func<Candidate, bool>>>()))
                              .ReturnsAsync((Candidate?)null);

            _candidateRepoMock.Setup(grc => grc.AddAsync(It.IsAny<Candidate>())).Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);


            await _candidateService.CreateOrUpdateCandidateAsync(dto);


             /*
                Verify() method here checks if that CacheService.Remove(...) was called with the key --> candidate:{email}
                due to the new insertion process above. If it wasn’t called (or was called with a different key), the test will fail.
            */
            _cacheServiceMock.Verify(c => c.Remove($"candidate:{dto.Email}"), Times.Once);


            /*
              Verify() method here Ensures that any previously cached candidate list (GetAllCandidatesAsync) is also invalidated,
              which keeps the cache in sync with the latest DB state
           */
            _cacheServiceMock.Verify(c => c.Remove("candidates:all"), Times.Once);
        }


        [Fact]
        public async Task GetCandidateByEmailAsync_ShouldReturnFromCache_WhenCached()
        {
            CandidateDto cachedDto = new()
            {
                Email = "cached@email.com",
                FirstName = "Asem",
                LastName = "Adel"
            };

            _cacheServiceMock.Setup(c => c.GetAsync<CandidateDto>($"candidate:{cachedDto.Email}"))
                             .ReturnsAsync(cachedDto);


            var result = await _candidateService.GetCandidateByEmailAsync($"{cachedDto.Email}");


            Assert.NotNull(result);
            Assert.Equal(cachedDto.FirstName, result.FirstName);
            _candidateRepoMock.Verify(grc => grc.FindAsync(It.IsAny<Expression<Func<Candidate, bool>>>()), Times.Never);
        }



        [Fact]
        public async Task GetCandidateByEmailAsync_ShouldFetchFromRepoAndSetCache_WhenNotInCache()
        {
            string email = "notincache@example.com";
            string normalizedEmail = email.ToLower();
           
            string cacheKey = $"candidate:{normalizedEmail}";

            Candidate candidate = new()
            {
                FirstName = "Ali",
                LastName = "Kamel",
                Email = email,
                PhoneNumber = "+201234567890"
            };

            _cacheServiceMock.Setup(c => c.GetAsync<CandidateDto>(cacheKey))
                             .ReturnsAsync((CandidateDto?)null); //simulate cache miss

            _candidateRepoMock.Setup(r =>
                r.FindAsync(It.IsAny<Expression<Func<Candidate, bool>>>()))
                .ReturnsAsync(candidate);

            _unitOfWorkMock.Setup(u => u.Repository<Candidate>())
                           .Returns(_candidateRepoMock.Object);

          

            CandidateDto? result = await _candidateService.GetCandidateByEmailAsync(email);

          
            Assert.NotNull(result);
            Assert.Equal(email.ToLower(), result.Email.ToLower());

            _cacheServiceMock.Verify(c => c.GetAsync<CandidateDto>(cacheKey), Times.Once);
            _cacheServiceMock.Verify(c => c.SetAsync(
                cacheKey,
                It.Is<CandidateDto>(d => d.Email.Equals(normalizedEmail, StringComparison.InvariantCultureIgnoreCase)),
                It.IsAny<TimeSpan>()), Times.Once);

            _candidateRepoMock.Verify(grc => grc.FindAsync(It.IsAny<Expression<Func<Candidate, bool>>>()), Times.Once);
        }
    }
}
