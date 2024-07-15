namespace notip_server.Interfaces
{
    public interface IPasswordService
    {
        string HashPassword(string password, string salt);

        string GenerateSalt();

        bool VerifyPassword(string hashedPassword, string salt, string password);
    }
}
