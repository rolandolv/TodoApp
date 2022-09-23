using TodoLibrary.Models;

namespace TodoLibrary.DataAccess;

public interface ITodoData
{
    Task<List<TodoModel>> GetAllAssigned(int assignedTo);
    Task<TodoModel?> GetOneAssigned(int assignedTo, int todoId);
    Task<TodoModel?> Create(int assignedTo, string task);
    Task UpdateTask(int assignedTo, int todoId, string task);
    Task CompleteTodo(int assignedTo, int todoId);
    Task Delete(int assignedTo, int todoId);
}