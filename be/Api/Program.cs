using Lib.Infrastructure.App;
using Lib.Modules.Auth;
using Lib.Modules.Users;
using Lib.Shared.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppInfrastructure(builder.Configuration);
builder.Services.AddUsersModule();
builder.Services.AddAuthModule(builder.Configuration, builder.Environment);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment()) app.MapOpenApi("/api/openapi/{documentName}.json");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();