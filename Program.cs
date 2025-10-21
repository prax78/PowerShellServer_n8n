
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace PowerShellServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();
           
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthorization();

       
            app.MapPost("/ExecuteCode", async (HttpContext context) =>
            {
                ILogger logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("PowerShellServer");
                try
                {
                    using var reader = new StreamReader(context.Request.Body);
                    var code = await reader.ReadToEndAsync();
                    var json = System.Text.Json.JsonSerializer.Deserialize<RecCode>(code);
                   
                    logger.LogCritical($"Incoming Connection {context.Connection.RemoteIpAddress}");
                    logger.LogWarning("Received code to execute.");
                    logger.LogInformation(json?.code);

                    var executePS = new ExecutePS(json?.code ?? "");

                    logger.LogWarning("Execution completed.");

                    return Results.Ok(new { result = executePS.Result });
                }
                catch (Exception ex)
                {
                   
                    logger.LogCritical("Error processing request: " + ex.Message);
                    return Results.Problem("Error processing request: " + ex.Message);
                }
            })
            .WithName("ExecuteCode");

            app.Run("http://0.0.0.0:10000");
        }
    }
}
