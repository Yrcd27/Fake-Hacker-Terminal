using System.Text.Json.Serialization;

namespace FakeHackerTerminal;   // <--- NEW NAMESPACE FIX

public record CommandDto(string Command);

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = null;
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        var app = builder.Build();

        // Serve index.html, style.css, script.js from wwwroot
        app.UseDefaultFiles();
        app.UseStaticFiles();

        var random = new Random();

        string GenerateRandomPassword(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // --------------------------
        // API ENDPOINTS
        // --------------------------

        app.MapGet("/api/terminal/random-line", () =>
        {
            string[] lines =
            {
                "Initializing secure protocol...",
                "Syncing clocks with remote node...",
                "Bypassing firewall rule set 7...",
                "Injecting payload into process ID 1337...",
                "Escalating privileges for current session...",
                "Brute forcing access token...",
                "Spoofing MAC address...",
                "Deploying keylogger module...",
                "Encrypting target backup archive...",
                "Exfiltrating logs to hidden node..."
            };

            var text = lines[random.Next(lines.Length)];
            return Results.Json(new { text });
        });

        app.MapGet("/api/terminal/password-line", () =>
        {
            string[] users = { "admin", "root", "guest", "yasiru", "testuser", "superuser" };

            string user = users[random.Next(users.Length)];
            int length = random.Next(6, 14);
            string password = GenerateRandomPassword(length);

            string text = $"Trying {user}:{password}";
            return Results.Json(new { text });
        });

        app.MapGet("/api/terminal/ip-line", () =>
        {
            string ip = $"{random.Next(1, 255)}.{random.Next(0, 255)}.{random.Next(0, 255)}.{random.Next(1, 255)}";
            string[] statuses = { "OPEN", "CLOSED", "FILTERED", "STEALTH" };
            string status = statuses[random.Next(statuses.Length)];

            string text = $"Scanning {ip} ... {status}";
            return Results.Json(new { text });
        });

        app.MapPost("/api/terminal/interpret", (CommandDto dto) =>
        {
            string cmd = dto.Command?.Trim().ToLower() ?? "";

            string response = cmd switch
            {
                "help" => "Commands: help, hack, scan, status, selfdestruct, cats, clear, exit",
                "hack" => "Launching multi-vector attack... Target compromised.",
                "scan" => "Scanning local network... 42 devices found. 5 are vulnerable.",
                "status" => "All systems running at 1337% efficiency.",
                "selfdestruct" => "Self-destruct sequence armed. Countdown started.",
                "cats" => "Uploading 1,000 cat pictures to target server.",
                "clear" => "__clear__",
                "exit" => "Nice try. There is no escape from the terminal.",
                _ => $"Command '{dto.Command}' not recognized. Brute forcing anyway."
            };

            return Results.Json(new { text = response });
        });

        app.Run();
    }
}
