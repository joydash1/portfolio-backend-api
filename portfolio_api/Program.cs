using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using portfolio_api.Data;
using portfolio_api.Endpoints;
using portfolio_api.Middlewares;
using portfolio_api.Models.Dtos;
using portfolio_api.Repositories;
using portfolio_api.Services;
using portfolio_api.Validators;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

#region OpenAPI

builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "Portfolio API",
            Version = "v1",
            Description = "Portfolio API Documentation"
        };

        return Task.CompletedTask;
    });
});

#endregion

#region Database

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

#endregion

#region SMTP

builder.Services.Configure<SmtpOptions>(
    builder.Configuration.GetSection("Smtp"));

#endregion

#region Dependency Injection

builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IMailService, MailService>();

#endregion

#region Validation

builder.Services.AddValidatorsFromAssemblyContaining<ContactCreateDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<MailDtoValidator>();

#endregion

#region Exception Handling


#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("portfolio_ui", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:1998"
            //"https://your-domain.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

#endregion

#region Rate Limiting

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("contact", limiterOptions =>
    {
        limiterOptions.PermitLimit = 5;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
        limiterOptions.AutoReplenishment = true;
    });
});

#endregion

#region Request Size Limit

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB
});

#endregion

var app = builder.Build();

app.UseGlobalExceptionMiddleware();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("portfolio_ui");

app.UseRateLimiter();

// Map OpenAPI endpoint (required for Scalar)
app.MapOpenApi();

// Map Scalar API reference UI
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Portfolio API")
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
               .WithTheme(ScalarTheme.Purple);
    });
}

app.MapContactEndpoints();
app.MapMailEndpoints();

app.Run();