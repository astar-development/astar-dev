using System.Reflection;
using System.Text;
using AStar.Dev.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AStar.Dev.Restful.Root.Document;

/// <summary>
///     The <see cref="RootDocument" /> class used to retrieve the default root document
/// </summary>
public sealed class RootDocument(ILogger<RootDocument> logger)
{
    /// <summary>
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public List<LinkResponse> GetLinks(Assembly assembly, CancellationToken cancellationToken = default)
    {
        logger.LogDebug($"{nameof(RootDocument)} GetLinks was called");

        var links = new List<LinkResponse>();

        if(cancellationToken.IsCancellationRequested) return links;

        IEnumerable<Type> endpoints = GetEndpoints(assembly);

        foreach(Type endpoint in endpoints)
        {
            RouteAttribute?         customAttributes = endpoint.GetCustomAttributes<RouteAttribute>().FirstOrDefault();
            var                     rel              = endpoint.ReflectedType?.Name;
            IEnumerable<MethodInfo> methods          = endpoint.GetMethods().Where(f => f.DeclaringType?.FullName?.Contains("Endpoint") == true);
            var                     routeBuilder     = new StringBuilder();
            _ = routeBuilder.Append(customAttributes?.Template ?? "");

            MethodInfo[] methodInfos = methods as MethodInfo[] ?? methods.ToArray();

            if(methodInfos.Length == 0) continue;

            foreach(MethodInfo method in methodInfos)
            {
                var                              template             = string.Empty;
                var                              httpMethod           = string.Empty;
                IEnumerable<HttpMethodAttribute> httpMethodAttributes = method.GetCustomAttributes<HttpMethodAttribute>();

                foreach(HttpMethodAttribute httpMethodAttribute in httpMethodAttributes)
                {
                    (string httpMethod, string template) res = GetHttpMethodWithTemplate(httpMethodAttribute);
                    template   = res.template;
                    httpMethod = res.httpMethod;
                }

                var routeTemplate = template.IsNotNullOrWhiteSpace() ? $"/{template}" : string.Empty;
                _ = routeBuilder.Append(routeTemplate);
                var route = routeBuilder.ToString().Replace("//", "/");

                if(route.IsNotNullOrWhiteSpace()) links.Add(new LinkResponse { Rel = rel ?? "self", Href = route, Method = httpMethod, });
            }
        }

        return links;
    }

    private static (string httpMethod, string template) GetHttpMethodWithTemplate(Attribute httpMethodAttribute)
    {
        var httpMethod = "GET";
        var template   = string.Empty;

        switch(httpMethodAttribute)
        {
            case HttpGetAttribute getAttribute:
                template   = getAttribute.Template;
                httpMethod = "GET";

                break;

            case HttpPostAttribute postAttribute:
                template   = postAttribute.Template;
                httpMethod = "POST";

                break;

            case HttpDeleteAttribute deleteAttribute:
                template   = deleteAttribute.Template;
                httpMethod = "DELETE";

                break;

            case HttpPutAttribute putAttribute:
                template   = putAttribute.Template;
                httpMethod = "PUT";

                break;
        }

        return (httpMethod, template!);
    }

    private static IEnumerable<Type> GetEndpoints(Assembly assembly)
        => assembly.GetTypes().Where(t => t.Namespace?.Contains("Endpoint") == true
                                          && !t.Name.EndsWith("Response")
                                          && !t.Name.EndsWith("Put")
                                          && !t.Name.EndsWith("Delete")
                                          && !t.Name.EndsWith("Post")
                                          && !t.Name.EndsWith("Create")
                                          && !t.Name.StartsWith('<'));
}
