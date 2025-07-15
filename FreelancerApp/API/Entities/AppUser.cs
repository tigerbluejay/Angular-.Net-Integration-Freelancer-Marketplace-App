namespace API.Entities;

public class AppUser
{
    public int Id { get; set; } 

    // UserName should never be null, so we add the required modifier
    public required string UserName { get; set; }
}