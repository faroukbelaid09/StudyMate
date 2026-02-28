using StudyMate.API.Models;

namespace StudyMate.API.Interfaces;

public interface IJwtTokenService
{
    string CreateToken(User user);
}