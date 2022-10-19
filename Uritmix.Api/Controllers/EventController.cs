using Command.Event;
using Command.Room;
using Helpers.Core;
using Helpers.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Model.Auth;
using Service.Security.UserAccessor;
using View.Event;
using View.Room;

namespace Uritmix.Api.Controllers;

[ApiController]
[Route("/api/v1/event")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ValidError))]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status200OK)]
public class EventController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    ///     Создает новое событие занятия
    /// </summary>
    [HttpPost]
    [AuthorizeByRole(AuthRole.Admin)]
    public Task<ResultResponse<EventView>> CreateEvent(
        [FromBody] CreateEvent.CreateEventForm model,
        CancellationToken ct)
    {
        return _mediator.Send(new CreateEvent.Command(model), ct);
    }
    
    /// <summary>
    ///     Возвращает список событий
    /// </summary>
    [HttpGet]
    [AuthorizeByRole(AuthRole.Manager, AuthRole.Admin)]
    public Task<ResultResponse<PaginatedListViewModel<EventView>>> GetEvents(
        [FromQuery] string filterExpression,
        CancellationToken ct)
    {
        return _mediator.Send(new GetEvents.Query(filterExpression), ct);
    }
}