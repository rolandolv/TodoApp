using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TodoLibrary.DataAccess;
using TodoLibrary.Models;

namespace TodoApi.Controllers.v1;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("1.0", Deprecated = true)]
public class TodosController : ControllerBase
{
    private readonly ITodoData _data;
    private readonly ILogger<TodosController> _logger;

    public TodosController(ITodoData data, ILogger<TodosController> logger)
    {
        _data = data;
        _logger = logger;
    }

    // GET: api/Todos
    /// <summary>
    /// Gets a list of all todos assigned to a user
    /// </summary>
    /// <remarks>
    /// Sample Request: GET /Todos
    /// Sample Response:
    /// [
    ///     {
    ///         "id": 2,
    ///         "task": "Mow the lawn",
    ///         "assignedTo": 1,
    ///         "isComplete": false
    ///     }
    /// ]
    /// </remarks>
    /// <returns>A list of todos</returns>
    [HttpGet]
    public async Task<ActionResult<List<TodoModel>>> Get()
    {
        _logger.LogInformation("GET: api/todos");
        try
        {
            var output = await _data.GetAllAssigned(GetUserId());

            return Ok(output);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "The GET call to api/Todos failed.");
            return BadRequest();
        }
    }
    private int GetUserId()
    {
        var userIdText = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdText);
    }
}