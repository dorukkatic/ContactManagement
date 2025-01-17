using Contacts.Contracts.Common;
using Contacts.Contracts.ContactInfos;

namespace Contacts.Contracts.People;

public record PersonDetailResponse(
    Guid Id,
    string FirstName,
    string? LastName,
    string? Company,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    PagedResponse<ContactInfoResponse> ContactInfos);
