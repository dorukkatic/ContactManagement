using Contacts.Contracts.Person;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.WebApi.Controllers;

[ApiController]
[ApiExplorerSettings(GroupName = "People")]
[Route("people")]
public class PeopleController : ControllerBase
{
    private readonly IPeopleService peopleService;

    public PeopleController(IPeopleService peopleService)
    {
        this.peopleService = peopleService;
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

    [HttpGet]
    [Route("{id:guid}")]
    public IActionResult GetPersonById(Guid id)
    {
        // Placeholder for fetching a person by ID (future feature)
        return Ok($"Person with ID {id} (Stub Response)");
    }
}