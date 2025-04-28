using Domain.Models;
using MongoDB.Driver;
using Persistence.Repositories;

namespace Application.Services;
public class AuthService
{
    private readonly UserRepository _userRepo;

    public AuthService(UserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<User> Register(User user)
    {
        await _userRepo.CreateUser(user);
        return user;
    }

    public async Task<User> Login(string username, string password)
    {
        var user = await _userRepo.GetUserByUsername(username);
        if (user != null && user.Password == password) // Replace with proper password hashing in production
        {
            return user;
        }
        throw new Exception("Invalid credentials");
    }

    public async Task<User> GetUserById(string userId) =>
        // Assuming UserRepository could be extended or a direct MongoDB query is used
        await _userRepo.GetUserById(userId); // Add this method to UserRepository if needed
}