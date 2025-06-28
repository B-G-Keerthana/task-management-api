using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models.Dtos;
using TaskManagementAPI.Models.Entities;
using TaskManagementAPI.Services;
namespace TaskManagementAPITests.ServiceTests
{

    public class TaskServiceTests
    {
        private ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase($"TaskDb_{Guid.NewGuid()}")
                .Options;

            return new ApplicationDbContext(options);
        }

        private ICurrentUserService MockUser(string userId, string role)
        {
            var mock = new Mock<ICurrentUserService>();
            mock.Setup(u => u.UserId).Returns(userId);
            mock.Setup(u => u.Role).Returns(role);
            return mock.Object;
        }

        [Fact]
        public void UpdateAsAdmin_UpdatesAllFields()
        {
            using var context = CreateDbContext(); // ✅ fresh DB

            var task = new TaskItem
            {
                TaskName = "Old Name",
                Description = "Old Desc",
                Status = "Pending",
                userId = "admin"
            };

            context.Tasks.Add(task);
            context.SaveChanges();

            var service = new TaskService(context, MockUser("admin", "Admin"), new NullLogger<TaskService>());

            var dto = new TaskUpdateDto
            {
                TaskName = "New Name",
                Description = "New Desc",
                Status = "Completed"
            };

            service.UpdateAsAdmin(task.Id, dto);

            var updated = context.Tasks.Find(task.Id);
            Assert.Equal("New Desc", updated.Description);
            Assert.Equal("Completed", updated.Status);
        }

    }

}