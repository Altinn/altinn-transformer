using System.Net;
using Altinn.Transformer.Configuration;
using Altinn.Transformer.Models.Dto;
using Altinn.Transformer.Services;
using Altinn.Transformer.Services.Interfaces;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<TransformerConfig>(builder.Configuration.GetSection(nameof(TransformerConfig)));
builder.Services
    .AddSingleton<IKeyResolverService, SettingsBasedKeyResolverService>()
    .AddSingleton<IEncryptionService, AesGcmEncryptionService>()
    .AddSingleton<ITransformerService, TransformerService>()
    .AddHttpContextAccessor()
    .AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/transform", async (IOptions<TransformerConfig> transformerConfig, ITransformerService transformerService, TransformerInputDto inputDto) =>
{
    try
    {
        var errors = inputDto.Validate(transformerConfig.Value);
        if (errors.Count > 0)
        {
            return Results.Problem(
                title: "Validation error",
                statusCode: (int) HttpStatusCode.BadRequest,
                detail: string.Join(", ", errors));
        }
        
        var input = inputDto.ToTransformerInput(transformerConfig.Value);
        var transformationResult = await transformerService.Transform(input);

        if (!transformationResult.Success)
        {
            return Results.Problem(
                title: "Transformation error",
                statusCode: (int) HttpStatusCode.BadRequest,
                detail: string.Join(", ", transformationResult.Errors));
        }
        
        var outputDto = transformationResult.Output.ToTransformerOutputDto();
        return Results.Ok(outputDto);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.Run();
