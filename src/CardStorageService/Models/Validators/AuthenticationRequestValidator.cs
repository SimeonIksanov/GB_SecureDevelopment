using CardStorageService.Models.Requests;
using FluentValidation;
using FluentValidation.Results;

namespace CardStorageService.Models.Validators;

public class AuthenticationRequestValidator : AbstractValidator<AuthenticationRequest>
{
    public AuthenticationRequestValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .Length(5, 255)
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(5);
    }
}
