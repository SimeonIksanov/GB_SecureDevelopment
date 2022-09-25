using CardStorageService.Models;
using CardStorageService.Models.Requests;

namespace CardStorageService.Services;

public interface IAuthenticationService
{
    AuthenticationResponse Login(AuthenticationRequest authenticationRequest);

    SessionInfo? GetSessionInfo(string token);
}
