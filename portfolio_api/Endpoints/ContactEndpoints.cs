using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using portfolio_api.Models.Dtos;
using portfolio_api.Services;

namespace portfolio_api.Endpoints;

public static class ContactEndpoints
{
    public static IEndpointRouteBuilder MapContactEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/contact", async (
            [FromForm] ContactCreateDto dto,
            IValidator<ContactCreateDto> validator,
            IContactService service,
            CancellationToken ct) =>
        {
            var validationResult = await validator.ValidateAsync(dto, ct);

            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(
                    validationResult.ToDictionary());
            }

            await service.SendContactAsync(dto, ct);

            return Results.Ok(new
            {
                message = "Message sent successfully."
            });
        })
        .Accepts<ContactCreateDto>("multipart/form-data")
        .DisableAntiforgery()
        .RequireRateLimiting("contact");

        return app;
    }
}