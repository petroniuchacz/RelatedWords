using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using RelatedWordsAPI.Controllers;
using RelatedWordsAPI.Models;
using RelatedWordsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using RelatedWordsAPI.Tests.Helpers;
using System.Data.Entity.Infrastructure;

namespace RelatedWordsAPI.Tests.UnitTests.Controller
{
    public class ProcessingController_Tests
    {

        public ProcessingController_Tests()
        {

        }

        // [Fact]
        public async Task Start_starts()
        {
            // Arrange
            int projectId = 2;
            var project = new Project { ProjectId = projectId };
            var data = new List<Project>
            {
                project
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Project>>();
            mockSet.As<IDbAsyncEnumerable<Project>>()
                .Setup(m => m.GetAsyncEnumerator())
                .Returns(new TestDbAsyncEnumerator<Project>(data.GetEnumerator()));

            mockSet.As<IQueryable<Project>>()
                .Setup(m => m.Provider)
                .Returns(new TestDbAsyncQueryProvider<Project>(data.Provider));

            mockSet.As<IQueryable<Project>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Project>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Project>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var context = new RelatedWordsContext { Projects = mockSet.Object };
            var mockRelatedWordsProcessorService = new Mock<IRelatedWordsProcessorService>();
            mockRelatedWordsProcessorService.Setup(m => m.TryStartProcessing(project))
                .Returns(true)
                .Verifiable();

            var controller = new ProcessingController(context, mockRelatedWordsProcessorService.Object);
            controller.ControllerContext = GetContext();

            // Act
            var result = await controller.Start(projectId);

            // Assert
            var okResult = Assert.IsType<NoContentResult>(result);
            mockRelatedWordsProcessorService.Verify();
        }

        private ControllerContext GetContext()
        {
            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, "username")
                    }, "someAuthTypeName"))
                }
            };
        }
    }
}
