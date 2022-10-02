using CardStorageService.Models.Requests;
using FluentValidation;

namespace CardStorageService.Models.Validators
{
    public class CardCreateRequestValidator : AbstractValidator<CardCreateRequest>
    {
        public CardCreateRequestValidator()
        {
            RuleFor(x => x.ExpireDate)
                .NotEmpty()
                .GreaterThan(DateTime.UtcNow);

            RuleFor(x => x.CardNo)
                .NotNull()
                .CreditCard();

            RuleFor(x => x.CVV2)
                .NotNull()
                .Length(3)
                .Must(cvv => cvv.All(c => char.IsDigit(c)));

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(50)
                .Must(n => n.All(char.IsLetter));

            RuleFor(x => x.ClientId)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}
