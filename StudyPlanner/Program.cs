using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Study_Planner._DLL.IRepository;
using Study_Planner._DLL.Repository;
using Study_Planner._DLL.Service;
using Study_Planner.BLL.IServices;
using Study_Planner.BLL.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
var configuration = builder.Configuration;

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton(configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserPreferenceRepository, UserPreferenceRepository>();
builder.Services.AddScoped < ISubjectsRepository ,SubjectsRepository>();



builder.Services.AddScoped<ISubjectsService, SubjectsService>();
builder.Services.AddScoped<IAuthService, AuthService>();


var jwtSettings = configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true
        };
    });


builder.Services.AddAuthorization();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");

app.UseAuthentication(); // Ensure authentication middleware is added
app.UseAuthorization();

app.MapControllers();
app.Run();
