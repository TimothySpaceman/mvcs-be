using Lib.Infrastructure.App;
using Lib.Modules.Auth;
using Lib.Modules.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppInfrastructure(builder.Configuration);
builder.Services.AddUsersModule();
builder.Services.AddAuthModule();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment()) app.MapGroup("/api").MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapGroup("/api").MapControllers();

app.Run();