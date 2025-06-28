using TaskManagementAPI.Models.Dtos;
namespace TaskManagementAPITests.DtoTests
{
    public class TaskUpdateDtoTests
    {
        [Fact]
        public void Can_Create_TaskUpdateDto_With_ValidData()
        {
            // Arrange & Act
            var dto = new TaskUpdateDto
            {
                TaskName = "Update Name",
                Description = "Update Description",
                Status = "InProgress"
            };

            // Assert
            Assert.Equal("Update Name", dto.TaskName);
            Assert.Equal("Update Description", dto.Description);
            Assert.Equal("InProgress", dto.Status);
        }

        [Fact]
        public void TaskName_And_Description_Can_Be_Null()
        {
            var dto = new TaskUpdateDto
            {
                TaskName = null,
                Description = null,
                Status = "Pending"
            };

            Assert.Null(dto.TaskName);
            Assert.Null(dto.Description);
            Assert.Equal("Pending", dto.Status);
        }
    }

}
