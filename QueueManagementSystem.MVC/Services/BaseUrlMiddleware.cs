using System.IO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

public class BaseUrlMiddleware
{
    private readonly RequestDelegate _next;

    public BaseUrlMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Retrieve the current base URL
        var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";

        // Define the path to the appsettings.json file
        var appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

        // Load the JSON file content
        var jsonContent = File.ReadAllText(appSettingsPath);
        var json = JObject.Parse(jsonContent);

        // Get the current CallbackUrl from the file
        var currentCallbackUrl = json["QMS"]?["CallbackUrl"]?.ToString();

        // Only update the file if the CallbackUrl differs from the base URL
        if (currentCallbackUrl != baseUrl)
        {
            // Update the CallbackUrl with the new base URL
            json["QMS"]["CallbackUrl"] = baseUrl;

            // Write the updated JSON back to the appsettings.json file
            File.WriteAllText(appSettingsPath, json.ToString());
        }

        // Proceed to the next middleware
        await _next(context);
    }
}
