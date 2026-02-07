using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.DTOs;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var todos = await _context.Todos
                .OrderByDescending(t => t.CreatedAt)
                .Where(t => t.CreatedAt.DayOfYear == DateTime.Now.DayOfYear)
                .ToListAsync();

            var response = todos.Select(t => MapToDto(t));
            return Ok(response);
        }

        // GET: api/todos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoResponseDto>> GetTodo(int id)
        {
            var todo = await _context.Todos.FindAsync(id);

            if (todo == null)
            {
                return NotFound(new { message = $"Todo with id {id} not found" });
            }

            return Ok(MapToDto(todo));
        }

        // POST: api/todos
        [HttpPost]
        public async Task<ActionResult<TodoResponseDto>> CreateTodo(CreateTodoDto createDto)
        {
            var todo = new Todo
            {
                Title = createDto.Title,
                Description = createDto.Description,
                Completed = false,
                CreatedAt = DateTime.UtcNow
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

            if (todo == null)
            {
                return NotFound(new { message = $"Todo with id {id} not found" });
            }

            // Update only provided fields
            if (updateDto.Title != null)
                todo.Title = updateDto.Title;

            if (updateDto.Description != null)
                todo.Description = updateDto.Description;

            if (updateDto.Completed.HasValue)
                todo.Completed = updateDto.Completed.Value;

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

            if (todo == null)
            {
                return NotFound(new { message = $"Todo with id {id} not found" });
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

            if (todo == null)
            {
                return NotFound(new { message = $"Todo with id {id} not found" });
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

        private static TodoResponseDto MapToDto(Todo todo)
        {
            return new TodoResponseDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                Completed = todo.Completed,
                CreatedAt = todo.CreatedAt,
                UpdatedAt = todo.UpdatedAt
            };
        }
    }
}
