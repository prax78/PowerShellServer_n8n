
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
                using var reader = new StreamReader(context.Request.Body);
                var code = await reader.ReadToEndAsync();
                var json= System.Text.Json.JsonSerializer.Deserialize<RecCode>(code);
                //Debug.WriteLine($"Received code:\n{json?.code}");
                var executePS = new ExecutePS(json?.code ?? "");
        
                //Debug.WriteLine( executePS.Result);
                return Results.Ok(new {result= executePS.Result });
            })
            .WithName("ExecuteCode");

            app.Run("http://0.0.0.0:10000");
        }
    }
}
