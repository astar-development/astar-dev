using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;

namespace AStar.Dev.Restful.Root.Document;

public sealed class RootDocumentShould
{
    [Fact]
    public void Return()
    {
        var sut = new RootDocument(new NullLogger<RootDocument>());

        List<LinkResponse> response = sut.GetLinks(Assembly.GetExecutingAssembly(), CancellationToken.None);

        response.ShouldBeEquivalentTo(new List<LinkResponse>
                                      {
                                          new() { Rel = "self", Href = "/delete/{id}", Method    = "DELETE", },
                                          new() { Rel = "self", Href = "/", Method               = "GET", },
                                          new() { Rel = "self", Href = "/{id}", Method           = "GET", },
                                          new() { Rel = "self", Href = "/something/{id}", Method = "GET", },
                                          new() { Rel = "self", Href = "/something", Method      = "GET", },
                                          new() { Rel = "self", Href = "/post/{id}", Method      = "POST", },
                                          new() { Rel = "self", Href = "/put/{id}", Method       = "PUT", },
                                      });
    }
}
