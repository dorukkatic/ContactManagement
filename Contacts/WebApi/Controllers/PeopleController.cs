using Contacts.Contracts.Common;
using Contacts.Contracts.ContactInfos;
using Contacts.Contracts.People;
using FluentResults.Extensions.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.WebApi.Controllers;

/// <summary>
/// Controller for managing people and their contact information.
/// </summary>
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
    /// Gets a paginated list of people with sorting options.
    /// </summary>
    /// <param name="pagination">
    ///     Pagination parameters:
    ///     - PageNumber: The page number to retrieve (starting from 1)
    ///     - PageSize: Number of items per page
    /// </param>
    /// <param name="orderBy">
    ///     Ordering parameters:
    ///     - OrderBy: The field to sort by (0=FirstName, 1=LastName, 2=Company, 3=CreatedAt, 4=UpdatedAt)
    ///     - IsDescending: Set to true for descending order, false for ascending
    /// </param>
    /// <param name="cancellationToken">Used to propagate notifications that the operation should be canceled</param>
    /// <response code="200">Returns the paginated list of people</response>
    [HttpGet]
    [Route("")]
    [ProducesResponseType(typeof(PagedResponse<PersonResponse>), StatusCodes.Status200OK)]
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
    /// Adds a new person to the system.
    /// </summary>
    /// <param name="person">The person details containing FirstName, LastName, and Company information</param>
    /// <returns>201 Created with the location header pointing to the new resource</returns>
    [HttpPost]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddPerson([FromBody] AddPersonRequest person)
    {
        var personId = await peopleService.AddPerson(person);
        return CreatedAtAction(nameof(GetPersonById), new { id = personId }, null);
    }

    /// <summary>
    /// Retrieves detailed information about a specific person.
    /// </summary>
    /// <param name="id">The unique identifier of the person</param>
    /// <returns>200 OK with person details or 404 Not Found if the person doesn't exist</returns>
    [HttpGet]
    [Route("{id:guid}")]
    [ProducesResponseType(typeof(PersonDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPersonById(Guid id)
    {
        var personResult = await peopleService.GetPersonById(id);
        if (personResult.IsFailed) return NotFound();

        return Ok(personResult.Value);
    }

    /// <summary>
    /// Adds contact information to a person.
    /// </summary>
    /// <param name="id">The unique identifier of the person</param>
    /// <param name="request">The contact information details with Type (0 - Email, 1 - Phone) and Value</param>
    /// <returns>201 Created with the location header or 404 Not Found if the person doesn't exist</returns>
    [HttpPost]
    [Route("{id:guid}/contact-info")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddContactInfoToPerson(Guid id, [FromBody] AddContactInfoRequest request)
    {
        var result = await contactInfosService.AddContactInfo(id, request);

        return
            result.IsFailed
                ? result.ToActionResult()
                : CreatedAtAction(nameof(GetPersonById), new { id = id }, null);
    }

    /// <summary>
    /// Retrieves all contact information for a specific person.
    /// </summary>
    /// <param name="id">The unique identifier of the person</param>
    /// <param name="pagination">The pagination parameters (pageNumber and pageSize)</param>
    /// <returns>200 OK with a paginated list of contact information</returns>
    [HttpGet]
    [Route("{id:guid}/contact-info")]
    [ProducesResponseType(typeof(PagedResponse<ContactInfoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContactInfos(Guid id, [FromQuery] PaginationQuery pagination)
    {
        var contactInfos =
            await contactInfosService.GetContactInfos(id, pagination.PageNumber, pagination.PageSize);
        return Ok(contactInfos);
    }

    /// <summary>
    /// Adds a location to a person. A person can only have one location.
    /// </summary>
    /// <param name="id">The unique identifier of the person</param>
    /// <param name="request">The location details containing the city name</param>
    /// <returns>201 Created or 400 Bad Request if the person already has a location</returns>
    [HttpPost]
    [Route("{id:guid}/location")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddLocationToPerson(Guid id, [FromBody] AddLocationRequest request)
    {
        var result = await contactInfosService.AddLocation(id, request);
        return
            result.IsFailed
                ? result.ToActionResult()
                : CreatedAtAction(nameof(GetPersonById), new { id = id }, null);
    }

    /// <summary>
    /// Deletes a person and all their associated contact information.
    /// </summary>
    /// <param name="id">The unique identifier of the person to delete</param>
    /// <returns>204 No Content or 404 Not Found if the person doesn't exist</returns>
    [HttpDelete]
    [Route("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePerson(Guid id)
    {
        var result = await peopleService.DeletePerson(id);

        return result.IsFailed ? NotFound() : NoContent();
    }

    /// <summary>
    /// Deletes a specific contact information entry.
    /// </summary>
    /// <param name="contactInfoId">The unique identifier of the contact information to delete</param>
    /// <returns>204 No Content or 404 Not Found if the contact information doesn't exist</returns>
    [HttpDelete]
    [Route("contact-info/{contactInfoId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteContactInfo(Guid contactInfoId)
    {
        var result = await contactInfosService.DeleteContactInfo(contactInfoId);

        if(result.IsFailed) return NotFound();

        return NoContent();
    }
}