using Microsoft.AspNetCore.Authorization;
using PolicyBasedAuthentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSingleton<IAuthorizationHandler, MinimumAgeHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, SoccerRoleRequirementHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, BasketballRoleRequirementHandler>();
//builder.Services.AddSingleton<IAuthorizationHandler, SecretKeyRequirementHandler>();

//builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);

builder.Services.AddAuthorization(options =>
{
    options.InvokeHandlersAfterFailure = false;

    options.AddPolicy("SoccerRolePolicy",
                          policy => policy
                          .AddRequirements(new SoccerRequirement("testApiKey", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJSb2xlIjoiU29jY2VyIiwiaWF0IjoxNTE2MjM5MDIyfQ.qDpR6UVqCDICLy0WNj_9JacaM5Bthl38joIEmK9yIYE")));

    options.AddPolicy("BasketballRolePolicy",
                          policy => policy
                          .AddRequirements(new BasketballRequirement("testApiKey", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJSb2xlIjoiQmFza2V0YmFsbCIsImlhdCI6MTUxNjIzOTAyMn0.XhN6MBzlcJJud0Ux0xQ-7jcwFGITUHsVrUs5qqIvw1g")));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
