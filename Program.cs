using eLearningApi.DataAccess;
using eLearningApi.Services;
using eLearningApi.UserRolesAndPermissions;
using eLearningApi.UserRolesAndPermissions.Attributes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection")));

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<TokenManager>();
builder.Services.AddScoped<ISubjectRepository, BaseSubjectRepository>();
builder.Services.AddScoped<IAuthorizationHandler, AccessOwnResourceOnlyAttribute>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

// apply policy
builder.Services.AddAuthorization(options =>
{
    foreach (Policy policy in Enum.GetValues(typeof(Policy)))
    {
        var roles = policy.GetAttribute<PermittedAttribute>()?.Roles;

        if (roles is null || !roles.Any()) continue;

        string[] roleNamesArr = [.. roles.Select(r => r.ToString())];

        options.AddPolicy(policy.ToString(), authPolicy =>
        {
            authPolicy.RequireRole(roleNamesArr);
        });
    }
    options.AddPolicy(nameof(Policy.OwnerPolicy), authPolicy =>
    {
        authPolicy.RequireAuthenticatedUser()
        .AddRequirements(new OwnerAuthorizationRequirement());
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
