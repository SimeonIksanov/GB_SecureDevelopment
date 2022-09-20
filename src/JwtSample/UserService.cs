using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtSample;

internal class UserService
{
    private const string SECRET_CODE = "some_super_duper_long_and_complex_passphrase";
    private IDictionary<string, string> _users = new Dictionary<string, string>()
    {
        {"root1", "test" }, // 0
        {"root2", "test" }  //1
    };

    public string Authenticate(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username)
            || string.IsNullOrWhiteSpace(password))
        {
            return string.Empty;
        }

        int i = 0;
        foreach(KeyValuePair<string, string> pair in _users)
        {
            if (string.CompareOrdinal(pair.Key, username) ==0 
                && string.CompareOrdinal(pair.Value, password) ==0 )
            {
                return GenerateJwtToken(i);
            }
            i++;
        }
        return string.Empty;
    }

    private string GenerateJwtToken(int id)
    {
        byte[] secretCodeBytes = Encoding.UTF8.GetBytes(SECRET_CODE);

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
        {
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretCodeBytes), SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString())
            })
        };

        JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
        
        return jwtSecurityTokenHandler.WriteToken(securityToken);
    }
}