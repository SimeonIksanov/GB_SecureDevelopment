using CardStorageService.Data;
using CardStorageService.Models;
using CardStorageService.Models.Requests;
using CardStorageService.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CardStorageService.Services.Implementation
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly int _tokenLifetimeInMinutes = 15;
        private readonly Dictionary<string, SessionInfo> _sessions = new();
        private readonly IServiceScopeFactory _serviceScopeFactory;

        internal static readonly string SecretKey = "1234567890123456";

        public AuthenticationService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        
        
        public AuthenticationResponse Login(AuthenticationRequest authenticationRequest)
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            CardStorageServiceDbContext dbContext = scope.ServiceProvider.GetRequiredService<CardStorageServiceDbContext>();

            Account? account = !string.IsNullOrWhiteSpace(authenticationRequest.Login)
                ? FindAccountByLogin(dbContext, authenticationRequest.Login)
                : null;

            if (account == null)
            {
                return new AuthenticationResponse { Status = AuthenticationStatus.UserNotFound };
            }

            if (!PasswordUtils.VerifyPassword(authenticationRequest.Password, account.PasswordSalt, account.PasswordHash))
            {
                return new AuthenticationResponse { Status = AuthenticationStatus.InvalidUsernameOrPassword };
            }

            AccountSession session = new AccountSession
            {
                AccountId = account.Id,
                SessionToken = CreateSessionToken(account),
                TimeCreated = DateTime.UtcNow,
                TimeLastRequest = DateTime.UtcNow,
                IsClosed = false,
            };
            dbContext.AccountSessions.Add(session);
            dbContext.SaveChanges();

            SessionInfo sessionInfo = GetSessionInfo(account, session);

            lock(_sessions)
            {
                _sessions[sessionInfo.SessionToken] = sessionInfo;
            }
            
            return new AuthenticationResponse
            {
                Status = AuthenticationStatus.Success,
                SessionInfo = sessionInfo
            };
        }

        public SessionInfo? GetSessionInfo(string token)
        {
            SessionInfo? sessionInfo;
            lock (_sessions)
            {
                _sessions.TryGetValue(token, out sessionInfo);
            }

            if (sessionInfo == null)
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();
                CardStorageServiceDbContext dbContext = scope.ServiceProvider.GetRequiredService<CardStorageServiceDbContext>();
                
                AccountSession? session = dbContext.AccountSessions.FirstOrDefault(session => session.SessionToken == token);

                if (session == null)
                    return null;

                Account? account = dbContext.Accounts.FirstOrDefault(account => account.Id == session.AccountId);

                if (account == null)
                    return null;

                sessionInfo = GetSessionInfo(account, session);
                if (sessionInfo != null)
                {
                    lock (_sessions)
                    {
                        _sessions[token] = sessionInfo;
                    }
                }
            }
            return sessionInfo;
        }

        private SessionInfo GetSessionInfo(Account account, AccountSession accountSession)
        {
            return new SessionInfo
            {
                SessionId = accountSession.Id,
                SessionToken = accountSession.SessionToken,
                Account = new AccountDto
                {
                    Id = account.Id,
                    Email = account.Email,
                    FirstName = account.FirstName,
                    SecondName = account.SecondName,
                    LastName = account.LastName,
                    Locked = account.Locked,
                }
            };
        }

        private Account? FindAccountByLogin(CardStorageServiceDbContext dbContext, string login)
        {
            return dbContext
                .Accounts
                .FirstOrDefault(account => account.Email == login);
        }

        private string CreateSessionToken(Account account)
        {
            byte[] secretCodeBytes = Encoding.UTF8.GetBytes(SecretKey);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
            {
                Expires = DateTime.UtcNow.AddMinutes(_tokenLifetimeInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretCodeBytes), SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                    new Claim(ClaimTypes.Email, account.Email),
                })
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);

            return jwtSecurityTokenHandler.WriteToken(securityToken);
        }
    }
}
