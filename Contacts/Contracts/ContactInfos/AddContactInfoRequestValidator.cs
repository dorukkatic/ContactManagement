using FluentValidation;

namespace Contacts.Contracts.ContactInfos;

public class AddContactInfoRequestValidator : AbstractValidator<AddContactInfoRequest>
{
    public AddContactInfoRequestValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid contact info type.");
        
        RuleFor(x => x.Type)
            .NotEqual(ContactInfoTypeDTO.Location)
            .WithMessage("Location type is not allowed for this endpoint. Use add location endpoint instead.");
        
        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("Value is required.");
    }
}