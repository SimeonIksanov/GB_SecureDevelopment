namespace CardStorageService.Models;

public enum AuthenticationStatus
{
    Success = 0,
    UserNotFound,
    InvalidUsernameOrPassword
}
