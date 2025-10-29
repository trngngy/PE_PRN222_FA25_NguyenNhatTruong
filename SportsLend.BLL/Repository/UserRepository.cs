using Microsoft.EntityFrameworkCore;
using SportsLend.BLL.Models;
using SportsLend.DAL.Models;
using System.Security.Cryptography;
using System.Text;

namespace SportsLend.BLL.Repository;

public class UserRepository : GenericRepository<User>
{
    public UserRepository()
    {

    }

    public UserRepository(SportsLendDBContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> ValidateUserAsync(string email, string password)
    {
        var user = await GetUserByEmailAsync(email);
        if (user == null || !user.IsActive.GetValueOrDefault())
        {
            return null;
        }

        byte[] inputPasswordHash = ComputeHash(password);

        Console.WriteLine("---------------------------------------------");
        Console.WriteLine($"[DEBUG] Mật khẩu nhập vào: {password}");
        Console.WriteLine($"[DEBUG] Input Hash (Hex): {GetHashAsHexString(inputPasswordHash)}");
        Console.WriteLine($"[DEBUG] DB Hash (Hex): {GetHashAsHexString(user.PasswordHash)}");
        Console.WriteLine("---------------------------------------------");

        if (user.PasswordHash != null && user.PasswordHash.SequenceEqual(inputPasswordHash))
        {
            return user;
        }

        return null;
    }

    private byte[] ComputeHash(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return Array.Empty<byte>();
        }

        using (var sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }

    private string GetHashAsHexString(byte[]? hashBytes)
    {
        if (hashBytes == null || hashBytes.Length == 0)
        {
            return "NULL/EMPTY";
        }
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}
