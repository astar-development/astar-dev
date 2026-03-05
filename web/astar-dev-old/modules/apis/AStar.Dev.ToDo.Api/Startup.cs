// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Identity.Web;

namespace AStar.Dev.ToDo.Api;

public sealed class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        _ = services.AddMicrosoftIdentityWebApiAuthentication(configuration);

        // services.AddAuthorization();
        _ = services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        _ = services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if(env.IsDevelopment())
            _ = app.UseDeveloperExceptionPage();
        else
            _ = app.UseHsts();

        // app.UseHttpsRedirection();

        _ = app.UseRouting();

        //app.UseAuthentication();
        //app.UseAuthorization();

        _ = app.UseEndpoints(endpoints => { _ = endpoints.MapControllers(); });
    }
}
