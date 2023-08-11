using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RunningWebApp.Controllers;
using RunningWebApp.Interfaces;
using RunningWebApp.Models;
using Xunit;

namespace RunningWebApp.Tests.Controller
{
    public class ClubControllerTests
    {
        private ClubController clubController;
        private IClubRepository clubRepository;
        private IPhotoService photoService;
        private IHttpContextAccessor httpContextAccessor;

        public ClubControllerTests()
        {
            //Dependencies
            clubRepository = A.Fake<IClubRepository>();
            photoService = A.Fake<IPhotoService>();
            httpContextAccessor = A.Fake<HttpContextAccessor>();

            //SUT (Sys under Test)
            clubController = new ClubController(clubRepository, photoService, httpContextAccessor);
        }

        [Fact]
        public void ClubController_Index_ReturnsSuccess()
        {
            //Arrange
            var clubs = A.Fake<IEnumerable<Club>>();
            A.CallTo(() => clubRepository.GetAll()).Returns(clubs);

            //Act
            var result = clubController.Index();

            //Assert - Object check actions
            result.Should().BeOfType<Task<IActionResult>>();
        }

        [Fact]
        public void ClubController_Detail_ReturnsSuccess()
        {
            //Arrange
            var id = 1;
            var club = A.Fake<Club>();
            A.CallTo(() => clubRepository.GetByIdAsync(id)).Returns(club);

            //Act
            var result = clubController.Detail(id);

            //Assert
            result.Should().BeOfType<Task<IActionResult>>();
        }
    }
}
