using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TodoLibrary.DataAccess;
using TodoLibrary.Models;

namespace TodoApi.Controllers.v2;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[ApiVersion("2.0")]
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

    // GET: api/Todos/5
    [HttpGet("{todoId}", Name = "Get")]
    public async Task<ActionResult<TodoModel>> Get(int todoId)
    {
        _logger.LogInformation("GET: api/todos/{TodoId}", todoId);

        try
        {
            var output = await _data.GetOneAssigned(GetUserId(), todoId);

            return Ok(output);
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "The GET call to {ApiPath} failed. The Id was {TodoId}",
                $"api/Todos/Id",
                todoId);
            return BadRequest();
        }
    }

    // POST: api/Todos
    [HttpPost]
    public async Task<ActionResult<TodoModel>> Post([FromBody] string task)
    {
        _logger.LogInformation("POST: api/todos BODY: {Task}", task);

        try
        {
            var output = await _data.Create(GetUserId(), task);

            return Ok(output);
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "The POST call to {ApiPath} failed. The Task was {Task}",
                $"api/Todos",
                task);
            return BadRequest();
        }
    }

    // PUT: api/Todos/5
    [HttpPut("{todoId}")]
    public async Task<IActionResult> Put(int todoId, [FromBody] string task)
    {
        _logger.LogInformation("PUT: api/todos/{TodoId} BODY: {Task}", todoId, task);

        try
        {
            await _data.UpdateTask(GetUserId(), todoId, task);

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "The PUT call to {ApiPath} failed. The TodoId was {TodoId} The Task was {Task}",
                $"api/Todos/Id",
                todoId,
                task);
            return BadRequest();
        }
    }

    // PUT: api/Todos/5/Complete
    [HttpPut("{todoId}/Complete")]
    public async Task<IActionResult> Complete(int todoId)
    {
        _logger.LogInformation("PUT: api/todos/{TodoId}", todoId);

        try
        {
            await _data.CompleteTodo(GetUserId(), todoId);

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "The PUT call to {ApiPath} failed. The TodoId was {TodoId}",
                $"api/Todos/Id/Complete",
                todoId);
            return BadRequest();
        }
    }

    // DELETE: api/Todos/5
    [HttpDelete("{todoId}")]
    public async Task<IActionResult> Delete(int todoId)
    {
        _logger.LogInformation("DELETE: api/todos/{TodoId}", todoId);

        try
        {
            await _data.Delete(GetUserId(), todoId);

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "The DELETE call to {ApiPath} failed. The TodoId was {TodoId}",
                $"api/Todos/Id",
                todoId);
            return BadRequest();
        }
    }

    private int GetUserId()
    {
        var userIdText = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdText);
    }
}