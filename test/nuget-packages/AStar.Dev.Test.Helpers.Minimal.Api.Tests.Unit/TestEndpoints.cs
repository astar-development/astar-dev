using Microsoft.AspNetCore.Http.HttpResults;

namespace AStar.Dev.Test.Helpers.Minimal.Api;

internal static class TestEndpoints
{
    public static Ok<string[]> GetAll()
    {
        string[] todos = ["a", "b", "c",];

        return TypedResults.Ok(todos);
    }

    public static Created<object> Create()
    {
        string[] todos = ["a", "b", "c",];

        return TypedResults.Created(new Uri("https://www.example.com/"), new object());
    }
}
