using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models.Dtos;
using TaskManagementAPI.Models.Entities;
using TaskManagementAPI.Services;
namespace TaskManagementAPITests.ServiceTests
{
    public class UserServiceTests
    {
        private ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // ensures isolation
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public void User_Cannot_Update_UserName_Or_Role()
        {
            using var context = CreateDbContext();

            var user = new User { Id = Guid.NewGuid(), UserName = "user", UserEmail = "userEmail", Password = "Password", Phone = "phone", Role = "User" };
            var target = new User { Id = Guid.NewGuid(), UserName = "target", Role = "User", UserEmail = "userEmail", Password = "Password", Phone = "phone" };

            context.Users.AddRange(user, target);
            context.SaveChanges();

            var service = new UserService(context);
            var dto = new UpdateUserDto { UserName = "hacker", Role = "Admin" };

            var ex = Assert.Throws<InvalidOperationException>(() =>
                service.UpdateUser(target, dto, user.Id.ToString()));

            Assert.Equal("You are not allowed to update UserName or Role.", ex.Message);
        }

        [Fact]
        public void Throws_Unauthorized_When_CurrentUser_NotFound()
        {
            using var context = CreateDbContext();

            var user = new User { Id = Guid.NewGuid(), UserName = "user", UserEmail = "userEmail", Password = "Password", Phone = "phone", Role = "User" };
            var target = new User { Id = Guid.NewGuid(), UserName = "target", Role = "User", UserEmail = "userEmail", Password = "Password", Phone = "phone" };
            context.Users.Add(target);
            context.SaveChanges();

            var service = new UserService(context);
            var dto = new UpdateUserDto { Password = "new" };

            var ex = Assert.Throws<UnauthorizedAccessException>(() =>
                service.UpdateUser(target, dto, Guid.NewGuid().ToString()));

            Assert.Equal("Current user not found.", ex.Message);
        }

        [Fact]
        public void User_Can_Update_Password_Email_And_Phone()
        {
            using var context = CreateDbContext();

            var user = new User { Id = Guid.NewGuid(), UserName = "user", UserEmail = "userEmail", Password = "Password", Phone = "phone", Role = "User" };
            var target = new User { Id = Guid.NewGuid(), UserName = "target", Role = "User", UserEmail = "userEmail", Password = "Password", Phone = "phone" };

            context.Users.AddRange(user, target);
            context.SaveChanges();

            var service = new UserService(context);
            var dto = new UpdateUserDto
            {
                Password = "newPass",
                UserEmail = "new@mail.com",
                Phone = "999"
            };

            service.UpdateUser(target, dto, user.Id.ToString());

            Assert.Equal("newPass", target.Password);
            Assert.Equal("new@mail.com", target.UserEmail);
            Assert.Equal("999", target.Phone);
        }
    }

}
