using BlazorHosted.Server.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System;

namespace BlazorHosted.Server;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }
    protected IServiceProvider? ApplicationServices { get; set; } = null;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<MsGraphService>();
        services.AddScoped<MsGraphClaimsTransformation>();

        services.AddHttpClient();
        services.AddOptions();

        // Configuration.GetValue<string>("DownstreamApi:Scopes");
        // string[] initialScopes = scopes?.Split(' ');
        string[] initialScopes = Array.Empty<string>(); 

        services.AddMicrosoftIdentityWebAppAuthentication(Configuration, "AzureB2C")
            .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
            .AddInMemoryTokenCaches();

        services.Configure<MicrosoftIdentityOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.Events.OnTokenValidated = async context =>
            {
                if (ApplicationServices != null && context.Principal != null)
                {
                    using var scope = ApplicationServices.CreateScope();
                    context.Principal = await scope.ServiceProvider
                        .GetRequiredService<MsGraphClaimsTransformation>()
                        .TransformAsync(context.Principal);
                }
            };
        });

        services.AddControllersWithViews();

        services.AddRazorPages().AddMvcOptions(options =>
        {
            //var policy = new AuthorizationPolicyBuilder()
            //    .RequireAuthenticatedUser()
            //    .Build();
            //options.Filters.Add(new AuthorizeFilter(policy));
        }).AddMicrosoftIdentityUI();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        ApplicationServices = app.ApplicationServices;

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseSecurityHeaders(
            SecurityHeadersDefinitions.GetHeaderPolicyCollection(env.IsDevelopment(),
                Configuration["AzureB2C:Instance"]));

        app.UseHttpsRedirection();
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
            endpoints.MapFallbackToPage("/_Host");
        });
    }
}