using ApiKeyExample.Shared;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(ApiKeyAuthenticationOptions.Scheme)
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
    ApiKeyAuthenticationOptions.DefaultScheme, options =>
    {
        options.HeaderName = "x-api-key";
    });
builder.Services.AddAuthorization(options =>
{
    var defaultPolicyBuilder = new AuthorizationPolicyBuilder(
        ApiKeyAuthenticationOptions.DefaultScheme);

    defaultPolicyBuilder = defaultPolicyBuilder.RequireAuthenticatedUser();

    options.DefaultPolicy = defaultPolicyBuilder.Build();

    var apiKeyPolicy = new AuthorizationPolicyBuilder(ApiKeyAuthenticationOptions.Scheme);
    options.AddPolicy("ApiKey", apiKeyPolicy.RequireAuthenticatedUser().Build());
});

builder.Services.AddFastEndpoints();

builder.Services.SwaggerDocument(o =>
{
    o.EnableJWTBearerAuth = false;
    o.DocumentSettings = s =>
    {
        s.Title = "Code Example";
        s.Version = "v1";
        s.AddAuth("ApiKey", new()
        {
            Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
            In = NSwag.OpenApiSecurityApiKeyLocation.Header,
            Name = "x-api-key",
            Scheme = ApiKeyAuthenticationOptions.Scheme
        });
    };
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints().UseSwaggerGen();

app.Run();
