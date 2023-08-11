using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RunningWebApp.Data;
using RunningWebApp.Data.Enum;
using RunningWebApp.Models;
using RunningWebApp.Repository;
using Xunit;

namespace RunningWebApp.Tests.Repository
{
    public class ClubRepositoryTest
    {
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            if (await databaseContext.Clubs.CountAsync() <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    databaseContext.Clubs.Add(
                      new Club()
                      {
                          Title = "Running Club 1",
                          Image = "https://www.eatthis.com/wp-content/uploads/sites/4/2020/05/running.jpg?quality=82&strip=1&resize=640%2C360",
                          Description = "This is the description of the first cinema",
                          ClubCategory = ClubCategory.City,
                          Address = new Address()
                          {
                              Street = "123 Main St",
                              City = "Charlotte",
                              State = "NC"
                          }
                      });
                    await databaseContext.SaveChangesAsync();
                }
            }
            return databaseContext;
        }

        [Fact]
        public async void ClubRepository_Add_ReturnsBool()
        {
            //Arrange
            var club = new Club()
            {
                Title = "Running Club 1",
                Image = "https://www.eatthis.com/wp-content/uploads/sites/4/2020/05/running.jpg?quality=82&strip=1&resize=640%2C360",
                Description = "This is the description of the first cinema",
                ClubCategory = ClubCategory.City,
                Address = new Address()
                {
                    Street = "123 Main St",
                    City = "Charlotte",
                    State = "NC"
                }
            };
            var dbContext = await GetDbContext();
            var clubRepository = new ClubRepository(dbContext);

            //Act
            var result = clubRepository.Add(club);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void ClubRepository_GetByIdAsync_ReturnsClub()
        {
            //Arrange
            var id = 1;
            var dbContext = await GetDbContext(); //AsNoTracking incase of tracking issues
            var clubRepository = new ClubRepository(dbContext);

            //Act
            var result = clubRepository.GetByIdAsync(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<Club>>();
        }

        [Fact]
        public async void ClubRepository_GetAll_ReturnsList()
        {
            //Arrange
            var dbContext = await GetDbContext();
            var clubRepository = new ClubRepository(dbContext);

            //Act
            var result = await clubRepository.GetAll();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Club>>();
        }

        //[Fact] -- todo: add repo count method
        //public async void ClubRepository_SuccessfulDelete_ReturnsTrue()
        //{
        //    //Arrange
        //    var club = new Club()
        //    {
        //        Title = "Running Club 1",
        //        Image = "https://www.eatthis.com/wp-content/uploads/sites/4/2020/05/running.jpg?quality=82&strip=1&resize=640%2C360",
        //        Description = "This is the description of the first cinema",
        //        ClubCategory = ClubCategory.City,
        //        Address = new Address()
        //        {
        //            Street = "123 Main St",
        //            City = "Charlotte",
        //            State = "NC"
        //        }
        //    };
        //    var dbContext = await GetDbContext();
        //    var clubRepository = new ClubRepository(dbContext);

        //    //Act
        //    clubRepository.Add(club);
        //    var result = clubRepository.Delete(club);
        //    var count = await clubRepository.>>GetCountAsync()<<;

        //    //Assert
        //    result.Should().BeTrue();
        //    count.Should().Be(0);
        //}
    }
}
