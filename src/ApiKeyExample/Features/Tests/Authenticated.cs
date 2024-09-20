using FastEndpoints;

namespace ApiKeyExample.Features.Tests;

public record Response(string Message);

public class Authenticated : EndpointWithoutRequest<Response>
{
    public override void Configure()
    {
        Get("/api/authenticated");
        Description(d => d.Produces<Response>(200));
        Policies("ApiKey");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = new Response("Success!");
        await SendOkAsync(result, ct);
    }
}
