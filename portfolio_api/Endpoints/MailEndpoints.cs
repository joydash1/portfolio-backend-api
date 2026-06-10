using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using portfolio_api.Models.Dtos;
using portfolio_api.Services;

namespace portfolio_api.Endpoints;

public static class MailEndpoints
{
    public static IEndpointRouteBuilder MapMailEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/mail", async (
                [FromForm] MailDto dto,
                [FromServices] IValidator<MailDto> validator,
                [FromServices] IMailService service,
                CancellationToken ct) =>
        {
            var validationResult = await validator.ValidateAsync(dto, ct);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            await service.SendMailAsync(dto, ct);

            return Results.Ok(new
            {
                message = "Message sent successfully."
            });
        })
        .Accepts<MailDto>("multipart/form-data")
        .RequireRateLimiting("contact");

        return app;
    }
}