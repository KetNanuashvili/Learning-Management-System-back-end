using Learning_Management_System.Data;
using Learning_Management_System.Models;
using Learning_Management_System.Repositories.LMS;
using Learning_Management_System.Services;

using Learning_Management_System.Services.AuthServiceFile;
using Learning_Management_System.Services.Interfaces;
using Learning_Management_System.Services.LMSServiceFile;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<ILMSRepository, LMSRepository>();
// ✅ DB Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ✅ Authentication/Authorization
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// ✅ Custom Services
builder.Services.AddScoped<IAuthService, AuthService>(); // შენი AuthService
builder.Services.AddScoped<ILMSService, LMSService>();   // შენი LMSService

// ✅ Controllers
builder.Services.AddControllers();

// ✅ Swagger (JWT-ის მხარდაჭერით)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LMS API",
        Version = "v1"
    });

    // 🔐 Add JWT support
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ✅ Seed Roles & Admin (შეამოწმეთ მხოლოდ dev გარემოში გაშვდეს თუ გსურს)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRolesAndAdmin.InitializeAsync(services);
}

// ✅ Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // მნიშვნელოვანია
app.UseAuthorization();

app.MapControllers();

app.Run();
