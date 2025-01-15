using FluentValidation;

namespace Contacts.Contracts.Person;

public class AddPersonRequestValidator : AbstractValidator<AddPersonRequest>
{
    public AddPersonRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.");
    }
    
}