using TaskBoard.Domain.Entities;

public class UserTests
{
    [Fact]
    public void CreateTaskItem_ShouldSetProperties()
    {
        var oneTaskItems = new User("user@test.com", "Test123");

        Assert.Equal("user@test.com", oneTaskItems.Email);
    }
}