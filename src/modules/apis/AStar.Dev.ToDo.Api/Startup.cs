// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;

namespace AStar.Dev.ToDo.Api;

public sealed class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMicrosoftIdentityWebApiAuthentication(configuration);

        // services.AddAuthorization();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if(env.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else
            app.UseHsts();

        // app.UseHttpsRedirection();

        app.UseRouting();

        //app.UseAuthentication();
        //app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
