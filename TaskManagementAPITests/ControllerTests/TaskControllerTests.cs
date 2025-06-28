using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Models.Dtos;
using TaskManagementAPI.Models.Entities;
using TaskManagementAPI.Services;

namespace TaskManagementAPITests.ControllerTests
{
    public class TasksControllerTests
    {
        private TasksController CreateController(string role = "User", string userId = "user1", Mock<ITaskService>? serviceMock = null)
        {
            serviceMock ??= new Mock<ITaskService>();
            var loggerMock = new Mock<ILogger<TasksController>>();

            var controller = new TasksController(serviceMock.Object, loggerMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }

        [Fact]
        public void Create_ShouldCallServiceAndReturnOk()
        {
            var serviceMock = new Mock<ITaskService>();
            var controller = CreateController("Admin", serviceMock: serviceMock);

            var task = new TaskItem
            {
                Id = 1,
                TaskName = "Test Task",
                Description = "Test Desc",
                userId = "user1"
            };

            var result = controller.Create(task);

            serviceMock.Verify(s => s.Create(task), Times.Once);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void Get_ShouldReturnTask_WhenFound()
        {
            var task = new TaskItem { Id = 1, TaskName = "Task", userId = "user1" };
            var serviceMock = new Mock<ITaskService>();
            serviceMock.Setup(s => s.Get(1)).Returns(task);

            var controller = CreateController("User", serviceMock: serviceMock);

            var result = controller.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(task, okResult.Value);
        }

        [Fact]
        public void Get_ShouldReturnNotFound_WhenTaskNotFound()
        {
            var serviceMock = new Mock<ITaskService>();
            serviceMock.Setup(s => s.Get(1)).Returns((TaskItem?)null);

            var controller = CreateController("User", serviceMock: serviceMock);

            var result = controller.Get(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Id not found!", notFound.Value);
        }

        [Fact]
        public void UpdateTask_AsAdmin_ShouldCallUpdateAsAdmin()
        {
            var serviceMock = new Mock<ITaskService>();
            var controller = CreateController("Admin", serviceMock: serviceMock);

            var dto = new TaskUpdateDto { Status = "Done" };

            var result = controller.UpdateTask(1, dto);

            serviceMock.Verify(s => s.UpdateAsAdmin(1, dto), Times.Once);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Task Updated!", ok.Value);
        }

        [Fact]
        public void UpdateTask_AsUser_ShouldCallUpdateAsUser()
        {
            var serviceMock = new Mock<ITaskService>();
            var controller = CreateController("User", "user42", serviceMock: serviceMock);

            var dto = new TaskUpdateDto { Status = "InProgress" };

            var result = controller.UpdateTask(1, dto);

            serviceMock.Verify(s => s.UpdateAsUser(1, dto, "user42"), Times.Once);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Task Updated!", ok.Value);
        }

        [Fact]
        public void UpdateTask_ShouldReturnBadRequest_WhenDtoIsInvalid()
        {
            var controller = CreateController("User");

            var result = controller.UpdateTask(1, null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid or missing status.", badRequest.Value);
        }

        [Fact]
        public void UpdateTask_ShouldReturnBadRequest_WhenServiceThrows()
        {
            var serviceMock = new Mock<ITaskService>();
            serviceMock.Setup(s => s.UpdateAsUser(It.IsAny<int>(), It.IsAny<TaskUpdateDto>(), It.IsAny<string>()))
                       .Throws(new Exception("Update failed"));

            var controller = CreateController("User", "user1", serviceMock);

            var dto = new TaskUpdateDto { Status = "InProgress" };

            var result = controller.UpdateTask(1, dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Update failed", badRequest.Value);
        }

        [Fact]
        public void Delete_ShouldCallServiceAndReturnOk()
        {
            var serviceMock = new Mock<ITaskService>();
            var controller = CreateController("Admin", serviceMock: serviceMock);

            var result = controller.Delete(1);

            serviceMock.Verify(s => s.Delete(1), Times.Once);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User deleted!", ok.Value);
        }

        [Fact]
        public void GetAll_ShouldReturnAllTasks()
        {
            var tasks = new List<TaskItem>
            {
                new TaskItem { Id = 1, TaskName = "Task 1", userId = "user1" },
                new TaskItem { Id = 2, TaskName = "Task 2", userId = "user2" }
            };

            var serviceMock = new Mock<ITaskService>();
            serviceMock.Setup(s => s.GetAll()).Returns(tasks);

            var controller = CreateController("Admin", serviceMock: serviceMock);

            var result = controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(tasks, ok.Value);
        }
    }
}
