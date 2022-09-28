using CardStorageService.Models.Requests;
using FluentValidation;

namespace CardStorageService.Models.Validators;

public class ClientCreateRequestValidator : AbstractValidator<ClientCreateRequest>
{
    public ClientCreateRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(255)
            .Must(n => n.All(char.IsLetter));

        RuleFor(x => x.Patronymic)
            .NotEmpty()
            .MaximumLength(255)
            .Must(n => n.All(char.IsLetter));

        RuleFor(x => x.Surname)
            .NotEmpty()
            .MaximumLength(255)
            .Must(n => n.All(char.IsLetter));
    }
}
