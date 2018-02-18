using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace vue_core_okta.Controllers
{
    [Route("api/[controller]")]
    public class ApiDataController : Controller
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }

        public class WeatherForecast
        {
            public string DateFormatted { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }

            public int TemperatureF
            {
                get
                {
                    return 32 + (int)(TemperatureC / 0.5556);
                }
            }
        }
    }

    [Route("api/[controller]")]
    public class TodoController : Controller
    {
        private static ConcurrentBag<Todo> todos = new ConcurrentBag<Todo> {
            new Todo { Id = Guid.NewGuid(), Description = "Learn Vue" }
        };

        [HttpGet()]
        public IEnumerable<Todo> GetTodos()
        {
            return todos.Where(t => !t.Done);
        }

        [HttpPost()]
        public Todo AddTodo([FromBody]Todo todo)
        {
            todo.Id = Guid.NewGuid();
            todo.Done = false;
            todos.Add(todo);
            return todo;
        }

        [HttpDelete("{id}")]
        public ActionResult CompleteTodo(Guid id)
        {
            var todo = todos.SingleOrDefault(t => t.Id == id);
            if (todo == null) return NotFound();

            todo.Done = true;
            return StatusCode(204);
        }
        public class Todo
        {
            public Guid Id { get; set; }
            public string Description { get; set; }
            public bool Done { get; set; }
        }
    }
}