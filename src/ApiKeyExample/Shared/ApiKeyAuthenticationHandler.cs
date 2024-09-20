using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ApiKeyExample.Shared;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options,
		ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
	{
	}

	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(Options.HeaderName))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

		string headerValue = Request.Headers[Options.HeaderName].ToString();

        // here you can call your database for example to find and match the API key.

		if (headerValue != "this-is-a-test-key")
		{
			return Task.FromResult(AuthenticateResult.Fail("Wrong or expired API key"));
		}

		var claims = new Claim[]
		{
			new(ClaimTypes.NameIdentifier, headerValue),
			new(ClaimTypes.Name, "ExternalApp")
		};

		var identity = new ClaimsIdentity(claims, nameof(ApiKeyAuthenticationHandler));
		var principal = new ClaimsPrincipal(identity); ;
		var ticket = new AuthenticationTicket(principal, Scheme.Name);

		return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
	public const string DefaultScheme = "ApiKeyScheme";
	public static string Scheme => DefaultScheme;
	public string HeaderName { get; set; } = "x-api-key";
}
