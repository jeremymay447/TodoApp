using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TodoApi.Data;
using TodoApi.DTOs;
using TodoApi.Enums;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TodosController : ControllerBase
    {
        private readonly TodoContext _context;
        private readonly ILogger<TodosController> _logger;

        public TodosController(TodoContext context, ILogger<TodosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/todos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoResponseDto>>> GetTodos()
        {
            var userId = GetCurrentUserId();

            var todos = await _context.Todos
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var response = todos.Select(t => MapToDto(t));
            return Ok(response);
        }

        // GET: api/todos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoResponseDto>> GetTodo(int id)
        {
            var userId = GetCurrentUserId();
            var todo = await _context.Todos.FindAsync(id);

            if (todo == null)
            {
                return NotFound(new { message = $"Todo with id {id} not found" });
            }

            if (todo.UserId != userId)
            {
                return Forbid(); // User doesn't own this todo
            }

            return Ok(MapToDto(todo));
        }

        // POST: api/todos
        [HttpPost]
        public async Task<ActionResult<TodoResponseDto>> CreateTodo(CreateTodoDto createDto)
        {
            var userId = GetCurrentUserId();

            var todo = new Todo
            {
                Title = createDto.Title,
                Description = createDto.Description,
                Completed = false,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                Priority = createDto.Priority                
            };

            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created todo with id {TodoId}", todo.Id);

            return CreatedAtAction(
                nameof(GetTodo),
                new { id = todo.Id },
                MapToDto(todo)
            );
        }

        // PUT: api/todos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, UpdateTodoDto updateDto)
        {
            var todo = await _context.Todos.FindAsync(id);
            var userId = GetCurrentUserId();


            if (todo == null)
            {
                return NotFound(new { message = $"Todo with id {id} not found" });
            }

            if (todo.UserId != userId)
            {
                return Forbid(); 
            }

            
            if (updateDto.Title != null)
                todo.Title = updateDto.Title;

            if (updateDto.Description != null)
                todo.Description = updateDto.Description;

            if (updateDto.Completed.HasValue)
                todo.Completed = updateDto.Completed.Value;

            if (updateDto.Priority.HasValue)
                todo.Priority = updateDto.Priority.Value;

            todo.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated todo with id {TodoId}", id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return Ok(MapToDto(todo));
        }

        // PATCH: api/todos/5/toggle
        [HttpPatch("{id}/toggle")]
        public async Task<IActionResult> ToggleTodo(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            var userId = GetCurrentUserId();

            if (todo == null)
            {
                return NotFound(new { message = $"Todo with id {id} not found" });
            }

            if (todo.UserId != userId)
            {
                return Forbid(); // User doesn't own this todo
            }

            todo.Completed = !todo.Completed;
            todo.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Toggled todo {TodoId} to {CompletedStatus}", id, todo.Completed);

            return Ok(MapToDto(todo));
        }

        // DELETE: api/todos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            var userId = GetCurrentUserId();

            if (todo == null)
            {
                return NotFound(new { message = $"Todo with id {id} not found" });
            }

            if (todo.UserId != userId)
            {
                return Forbid(); // User doesn't own this todo
            }

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted todo with id {TodoId}", id);

            return NoContent();
        }

        private bool TodoExists(int id)
        {
            return _context.Todos.Any(e => e.Id == id);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim!);
        }

        private static TodoResponseDto MapToDto(Todo todo)
        {
            return new TodoResponseDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                Completed = todo.Completed,
                CreatedAt = todo.CreatedAt,
                UpdatedAt = todo.UpdatedAt,
                Priority = todo.Priority
            };
        }
    }
}
