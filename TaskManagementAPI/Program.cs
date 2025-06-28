using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TaskManagementAPI.Data;
using TaskManagementAPI.Middleware;
using TaskManagementAPI.Models.Entities;
using TaskManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt");
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = "role"
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token here. Example: **Bearer your-token-here**"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new string[] {}
        }
    });
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Task API", Version = "v1" });

});

var app = builder.Build();

// Add test data after app is built
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (!dbContext.Users.Any())
    {
        var usersAdded = new List<User>
    {
        new User
        {
            Id = Guid.Parse("61c26ccd-dfb0-4f39-820c-067db886a858"),
            UserName = "Jhon",
            Password = "jhonpw",
            UserEmail = "jhon@gmail.com",
            Phone = "+974-10101011",
            Role = "Admin"
        },
        new User
        {
            Id = Guid.Parse("61c26ccd-dfb0-4f39-820c-067db886a859"),
            UserName = "Bob",
            Password = "bobpw",
            UserEmail = "bob@gmail.com",
            Phone = "+974-10101022",
            Role = "User"
        }
    };

        dbContext.Users.AddRange(usersAdded);
        dbContext.SaveChanges();
    }
    if (!dbContext.Tasks.Any())
    {
        var taskItemsAdded = new List<TaskItem>
    {
        new TaskItem
        {
            Id = 1,
            TaskName = "Adminstration",
            Description = "Prepare Report",
            Status = "Pending",
            UserId = "61c26ccd-dfb0-4f39-820c-067db886a858"
        },
        new TaskItem
        {
            Id = 2,
            TaskName = "Development",
            Description = "Development of Web Application",
            Status = "Active",
            UserId = "61c26ccd-dfb0-4f39-820c-067db886a859"
        },
        new TaskItem
        {
            Id = 3,
            TaskName = "Hiring",
            Description = "Hire Resources",
            Status = "InActive",
            UserId = "61c26ccd-dfb0-4f39-820c-067db886a860"
        }
    };

        dbContext.Tasks.AddRange(taskItemsAdded);
        dbContext.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Task API v1");

    options.ConfigObject.AdditionalItems["persistAuthorization"] = false;
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RoleBasedMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }
