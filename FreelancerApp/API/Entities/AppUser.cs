namespace API.Entities;

public class AppUser
{
    public int Id { get; set; }

    // UserName should never be null, so we add the required modifier
    public required string UserName { get; set; }
    
    public required byte[] PasswordHash { get; set; }
    
    // the Salt scrambles the Hash for increased security
    public required byte[] PasswordSalt { get; set; }
}