using Contacts.Contracts.Common;
using Contacts.Contracts.ContactInfos;
using Contacts.Contracts.People;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.WebApi.Controllers;

[ApiController]
[ApiExplorerSettings(GroupName = "People")]
[Route("people")]
public class PeopleController : ControllerBase
{
    private readonly IPeopleService peopleService;
    private readonly IContactInfosService contactInfosService;

    public PeopleController(
        IPeopleService peopleService, 
        IContactInfosService contactInfosService)
    {
        this.peopleService = peopleService;
        this.contactInfosService = contactInfosService;
    }

    /// <summary>
    /// Gets a paginated list of people.
    /// </summary>
    /// <param name="pagination">The pagination values</param>
    /// <param name="orderBy">The ordering values</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of people.</returns>
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetPeople(
        [FromQuery] PaginationQuery pagination,
        [FromQuery] PeopleOrderByQuery orderBy,
        CancellationToken cancellationToken = default)
    {
        var people =
            await peopleService.GetPeople(
                pagination.PageNumber,
                pagination.PageSize,
                orderBy.OrderBy,
                orderBy.IsDescending,
                cancellationToken);

        return Ok(people);
    }

    /// <summary>
    /// Adds a new person.
    /// </summary>
    /// <param name="person">The request containing person details.</param>
    /// <returns>A newly created person ID.</returns>
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> AddPerson([FromBody] AddPersonRequest person)
    {
        var personId = await peopleService.AddPerson(person);
        return CreatedAtAction(nameof(GetPersonById), new { id = personId }, null);
    }

    /// <summary>
    /// Gets a person by ID.
    /// </summary>
    /// <param name="id">The ID of the person to retrieve.</param>
    /// <returns>The person with the specified ID.</returns>
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetPersonById(Guid id)
    {
        var person = await peopleService.GetPersonById(id);
        if (person is null) return NotFound(new { Message = $"Person with ID {id} was not found." });

        return Ok(person);
    }

    [HttpPost]
    [Route("{id:guid}/contact-info")]
    public async Task<IActionResult> AddContactInfoToPerson(Guid id, [FromBody] AddContactInfoRequest request)
    {
        await contactInfosService.AddContactInfo(id, request);
        return CreatedAtAction(nameof(GetPersonById), new { id = id }, null);
    }
}