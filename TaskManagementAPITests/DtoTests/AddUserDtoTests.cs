using TaskManagementAPI.Models.Dtos;

namespace TaskManagementAPITests.DtoTests
{
    public class AddUserDtoTests
    {
        [Fact]
        public void Can_Create_AddUserDto_With_ValidData()
        {
            // Arrange & Act
            var dto = new AddUserDto
            {
                UserName = "Alice",
                Password = "securepw",
                UserEmail = "alice@example.com",
                Phone = "1234567890",
                Role = "User"
            };

            // Assert
            Assert.Equal("Alice", dto.UserName);
            Assert.Equal("securepw", dto.Password);
            Assert.Equal("alice@example.com", dto.UserEmail);
            Assert.Equal("1234567890", dto.Phone);
            Assert.Equal("User", dto.Role);
        }

        [Fact]
        public void Phone_Can_Be_Null()
        {
            var dto = new AddUserDto
            {
                UserName = "Bob",
                Password = "bobpw",
                UserEmail = "bob@example.com",
                Phone = null,
                Role = "Admin"
            };

            Assert.Null(dto.Phone);
        }
    }

}
