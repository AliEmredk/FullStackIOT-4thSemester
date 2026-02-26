namespace dataaccess.Entities;

public class AppUser
{
    public int Id { get; set; }

    public string Username { get; set; } = "";

    // store hashed password only (never store plain text)
    public string PasswordHash { get; set; } = "";

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public List<TurbineCommand> Commands { get; set; } = new();
}