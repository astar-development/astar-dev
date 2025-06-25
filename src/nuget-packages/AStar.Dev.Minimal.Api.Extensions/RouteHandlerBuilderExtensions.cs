using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AStar.Dev.Minimal.Api.Extensions;

/// <summary>
///     The <see cref="RouteHandlerBuilderExtensions" /> class adds a series of methods to reduce duplication across minimal APIs.
/// </summary>
public static class RouteHandlerBuilderExtensions
{
    /// <summary>
    ///     The AddBasicProduces method will add the successful, unauthorised and internal server error responses.
    /// </summary>
    /// <param name="routeBuilder">The instance of <see cref="RouteHandlerBuilder" /> to add the methods to</param>
    /// <typeparam name="T">The type of the successful response</typeparam>
    /// <returns>The original <see cref="RouteHandlerBuilder" /> to facilitate further chaining</returns>
    public static RouteHandlerBuilder AddBasicProduces<T>(this RouteHandlerBuilder routeBuilder) =>
        routeBuilder
           .Produces<T>()
           .Produces(401)
           .Produces(500);

    /// <summary>
    ///     The AddBasicWithAdditionalProduces method extends the <seealso cref="AddBasicProduces{T}" /> method to add:
    ///     403
    ///     404
    ///     400, and
    ///     422
    /// </summary>
    /// <param name="routeBuilder">The instance of <see cref="RouteHandlerBuilder" /> to add the methods to</param>
    /// <typeparam name="T">The type of the successful response</typeparam>
    /// <returns>The original <see cref="RouteHandlerBuilder" /> to facilitate further chaining</returns>
    public static RouteHandlerBuilder AddBasicWithAdditionalProduces<T>(this RouteHandlerBuilder routeBuilder) =>
        routeBuilder.AddBasicProduces<T>()
                    .Produces(404)
                    .Produces(403)
                    .Produces<ProblemDetails>(400)
                    .Produces<ProblemDetails>(422);
}
