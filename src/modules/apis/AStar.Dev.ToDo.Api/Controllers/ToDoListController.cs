using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace AStar.Dev.ToDo.Api.Controllers;

[Authorize]
[AuthorizeForScopes(Scopes = ["access_as_user",])]
[Route("api/[controller]")]
public sealed class TodoListController : Controller
{
    private static readonly Dictionary<int, ToDo> TodoStore = new();

    // GET: api/values
    [HttpGet]
    public IEnumerable<ToDo> Get()
    {
        var owner = User?.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

        return TodoStore.Values.Where(x => x.Owner == owner);
    }

    // GET: api/values
    [HttpGet("{id:int}", Name = "GetById")]
    public ToDo Get(int id)
        => TodoStore.Values.FirstOrDefault(t => t.Id == id);

    [HttpDelete("{id:int}")]
    public void Delete(int id)
        => TodoStore.Remove(id);

    // POST api/values
    [HttpPost]
    public IActionResult Post([FromBody] ToDo todo)
    {
        var id = 1;

        if(TodoStore.Count > 0) id = TodoStore.Values.OrderByDescending(x => x.Id).FirstOrDefault()?.Id + 1 ?? 0;

        var toDo = new ToDo { Id = id, Owner = HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "name")?.Value, Title = todo.Title, };
        TodoStore.Add(id, toDo);

        return CreatedAtRoute("GetById", todo);
    }

    // PATCH api/values
    [HttpPatch("{id:int}")]
    public IActionResult Patch(int id, [FromBody] ToDo toDo)
    {
        if(id != toDo.Id) return NotFound();

        if(TodoStore.Values.FirstOrDefault(x => x.Id == id) == null) return NotFound();

        TodoStore.Remove(id);
        TodoStore.Add(id, toDo);

        return Ok(toDo);
    }
}
