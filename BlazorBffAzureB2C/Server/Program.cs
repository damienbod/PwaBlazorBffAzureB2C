using BlazorHosted.Server;
using BlazorHosted.Server.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

var services = builder.Services;
var configuration = builder.Configuration;
var env = builder.Environment;
IServiceProvider? applicationServices = null;

services.AddScoped<MsGraphService>();
services.AddScoped<MsGraphClaimsTransformation>();

services.AddHttpClient();
services.AddOptions();

// Configuration.GetValue<string>("DownstreamApi:Scopes");
// string[] initialScopes = scopes?.Split(' ');
string[] initialScopes = Array.Empty<string>();

services.AddMicrosoftIdentityWebAppAuthentication(configuration, "AzureB2C")
    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
    .AddInMemoryTokenCaches();

services.Configure<MicrosoftIdentityOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Events.OnTokenValidated = async context =>
    {
        if (applicationServices != null && context.Principal != null)
        {
            using var scope = applicationServices.CreateScope();
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

var app = builder.Build();
applicationServices = app.Services;

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
        configuration["AzureB2C:Instance"]));

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

app.Run();
