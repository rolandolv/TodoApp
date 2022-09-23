using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TodoApi.StartupConfig;
using WatchDog;

var builder = WebApplication.CreateBuilder(args);

builder.AddStandardServices();
builder.AddCustomServices();
builder.AddHealthCheckServices();
builder.AddAuthServices();

var app = builder.Build();

app.UseWatchDogExceptionLogger();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}).AllowAnonymous();
app.MapHealthChecksUI().AllowAnonymous();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseWatchDog(options =>
{
    options.WatchPageUsername = app.Configuration.GetValue<string>("WatchDog:Username");
    options.WatchPagePassword = app.Configuration.GetValue<string>("WatchDog:Password");
    options.Blacklist = "health";
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();