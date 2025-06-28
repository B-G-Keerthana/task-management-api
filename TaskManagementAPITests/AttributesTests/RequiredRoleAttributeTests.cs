using TaskManagementAPI.Attributes;
namespace TaskManagementAPITests.AttributesTests
{
    public class RequiredRolesAttributeTests
    {
        [Fact]
        public void Constructor_SetsRolesCorrectly()
        {
            // Arrange
            var attribute = new RequiredRolesAttribute("Admin", "User");

            // Act
            var roles = attribute.Roles;

            // Assert
            Assert.Contains("Admin", roles);
            Assert.Contains("User", roles);
            Assert.Equal(2, roles.Length);
        }
    }

}
