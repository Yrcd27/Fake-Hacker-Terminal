using System.Text.Json.Serialization;

namespace FakeHackerTerminal;

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

        // Serve index.html, style.css, script.js
        app.UseDefaultFiles();
        app.UseStaticFiles();

        // CTF state: single player for now, one instance
        int currentLevel = 1;

        app.MapPost("/api/terminal/interpret", (CommandDto dto) =>
        {
            var raw = dto.Command ?? "";
            var command = raw.Trim();
            if (string.IsNullOrWhiteSpace(command))
            {
                return Results.Json(new { text = "" });
            }

            var lower = command.ToLowerInvariant();

            // -------- Global commands (work in any level) --------

            if (lower == "clear")
            {
                // Special marker so frontend clears screen
                return Results.Json(new { text = "__clear__" });
            }

            if (lower == "level")
            {
                return Results.Json(new { text = $"Current level: {currentLevel}/5" });
            }

            if (lower == "help")
            {
                var helpText = currentLevel switch
                {
                    1 => "The system is quiet. When you arrive somewhere new, the first thing to do is usually to list what is around you.",
                    2 => "Pieces are lying around already. Some lines are ordinary, some feel rotated out of place.",
                    3 => "Sometimes all you see is a fingerprint. Old, famous fingerprints tend to be well documented.",
                    4 => "Six small numbers, one repeated key. Apply the same trick backwards and characters will appear.",
                    5 => "The answer is already written in the noise. Asking for just the interesting part helps.",
                    _ => "Explore with simple moves: ls, cat <file>, grep <pattern> <file>, submit <flag>, level, clear."
                };
                return Results.Json(new { text = helpText });
            }

            if (lower == "flag" || lower == "flag format")
            {
                return Results.Json(new { text = "\u001b[33mFLAG FORMAT: FAKE{CONTENT_HERE}\u001b[0m\n\nAll flags follow this format:\n- Start with FAKE{\n- End with }\n- Content is UPPERCASE with underscores\n\nExample: FAKE{EXAMPLE_FLAG}" });
            }

            if (lower == "submit" || lower == "how to submit")
            {
                return Results.Json(new { text = "\u001b[36mHOW TO SUBMIT:\u001b[0m\n\nUse the command: submit <flag>\n\nExample:\n  submit FAKE{YOUR_FLAG_HERE}\n\nMake sure to include the entire flag including FAKE{ and }" });
            }

            // Flag submission (must be correct for current level)
            if (lower.StartsWith("submit "))
            {
                var submitted = command.Substring("submit ".Length).Trim();

                string response;

                if (currentLevel == 1)
                {
                    if (submitted == "FAKE{BASE24_DECODED}")
                    {
                        currentLevel = 2;
                        response = @"✔ Flag accepted.
Level 1 complete.
Something new should appear if you look around again.";
                    }
                    else
                    {
                        response = "The first key does not look like that yet.";
                    }
                }
                else if (currentLevel == 2)
                {
                    if (submitted == "FAKE{ROT13_SECRET}")
                    {
                        currentLevel = 3;
                        response = @"✔ Flag accepted.
Level 2 complete.
Now the text gives up on letters and becomes a single long fingerprint.";
                    }
                    else
                    {
                        response = "Those letters are still misaligned.";
                    }
                }
                else if (currentLevel == 3)
                {
                    if (submitted == "FAKE{MD5_PASSWORD}")
                    {
                        currentLevel = 4;
                        response = @"✔ Flag accepted.
Level 3 complete.
Next, the message hid itself behind a one-byte mask.";
                    }
                    else
                    {
                        response = "The flag should describe what that famous hash actually stands for.";
                    }
                }
                else if (currentLevel == 4)
                {
                    if (submitted == "FAKE{XOR_PUZZLE}")
                    {
                        currentLevel = 5;
                        response = @"✔ Flag accepted.
Level 4 complete.
The last answer is already scribbled in the logs.";
                    }
                    else
                    {
                        response = "If the mask is wrong, the revealed word will be wrong too.";
                    }
                }
                else if (currentLevel == 5)
                {
                    if (submitted == "FAKE{FINAL_CTF_MASTER}")
                    {
                        currentLevel = 6;
                        response = @"✔ Flag accepted.
Level 5 complete.

╔════════════════════════════════════════════════════════════╗
║          CONGRATULATIONS, CTF MASTER!                      ║
╚════════════════════════════════════════════════════════════╝

You have successfully completed all 5 levels of the
Fake Hacker Terminal CTF Challenge!

Challenges Conquered:
  [✓] Level 1: Base64 Decoding
  [✓] Level 2: ROT13 Cipher
  [✓] Level 3: MD5 Hash Cracking
  [✓] Level 4: XOR Decryption
  [✓] Level 5: Log Analysis with Grep

Your final flag: FAKE{FINAL_CTF_MASTER}

Thank you for playing! You've demonstrated skills in:
- File system navigation
- Encoding/decoding techniques
- Cryptographic hash identification
- Binary operations
- Pattern matching and log analysis

Stay curious, keep hacking, and remember:
The best hackers never stop learning.

════════════════════════════════════════════════════════════";
                    }
                    else
                    {
                        response = "The final flag is already present in the log file. You have seen it.";
                    }
                }
                else
                {
                    response = "There are no more levels open right now.";
                }

                return Results.Json(new { text = response });
            }

            // -------- Level-specific behaviour --------

            string output = currentLevel switch
            {
                1 => HandleLevel1(lower, command),
                2 => HandleLevel2(lower, command),
                3 => HandleLevel3(lower, command),
                4 => HandleLevel4(lower, command),
                5 => HandleLevel5(lower, command),
                _ => "You seem to have left the main path. level command might help."
            };

            return Results.Json(new { text = output });
        });

        app.Run();
    }

    // ---------------- Level 1: Base64 ----------------

    private static string HandleLevel1(string lower, string full)
    {
        if (lower == "ls")
        {
            return "encoded1.txt\nreadme.txt\ntrash.bin";
        }

        if (lower == "cat encoded1.txt")
        {
            // RkFLRXtCQVNFMjRfREVDT0RFRH0=  ->  FAKE{BASE24_DECODED}
            return "RkFLRXtCQVNFMjRfREVDT0RFRH0=";
        }

        if (lower == "cat readme.txt")
        {
            return "Not everything here is meant for human eyes in its current shape.\nSome formats prefer a very specific alphabet and like to end with '='.";
        }

        if (lower == "cat trash.bin")
        {
            return "You sift through bits of broken data. Nothing useful rises to the surface.";
        }

        return "The environment is small for now. Simple file reads are usually enough here.";
    }

    // ---------------- Level 2: ROT13 ----------------

    private static string HandleLevel2(string lower, string full)
    {
        if (lower == "ls")
        {
            return "message2.rot\nold_notes.txt";
        }

        if (lower == "cat message2.rot")
        {
            // SNXR{EBG13_FRPERG} -> FAKE{ROT13_SECRET}
            return "SNXR{EBG13_FRPERG}";
        }

        if (lower == "cat old_notes.txt")
        {
            return "It already looks suspiciously like a flag.\nThe shape is right, the letters are just walking in circles.";
        }

        return "The answer is already on disk; the letters simply are not where you expect them.";
    }

    // ---------------- Level 3: MD5 hash ----------------

    private static string HandleLevel3(string lower, string full)
    {
        if (lower == "ls")
        {
            return "hash3.md5\nleak_notes.txt";
        }

        if (lower == "cat hash3.md5")
        {
            // Classic MD5 for the word 'password'
            return "5f4dcc3b5aa765d61d8327deb882cf99";
        }

        if (lower == "cat leak_notes.txt")
        {
            return "This fingerprint keeps appearing in old breach dumps.\nPeople who reuse obvious secrets tend to see it a lot.";
        }

        return "Here the entire challenge is that one line of hex characters.";
    }

    // ---------------- Level 4: XOR ----------------

    private static string HandleLevel4(string lower, string full)
    {
        if (lower == "ls")
        {
            return "xor4.bin\nkey_hint.txt";
        }

        if (lower == "cat xor4.bin")
        {
            // bytes: 3A 26 2F 20 3E 21  xor 0x55  =>  'F' 'A' 'K' 'E' '{' ...
            return "3A 26 2F 20 3E 21";
        }

        if (lower == "cat key_hint.txt")
        {
            return "Each of those bytes has been combined with the same one-byte key.\nApplying the same operation again with 0x55 will make the message readable.";
        }

        return "Six tiny numbers, one key. The rest is just repetition.";
    }

    // ---------------- Level 5: Logs + grep ----------------

    private static string HandleLevel5(string lower, string full)
    {
        if (lower == "ls")
        {
            return "logs5.txt\nnoise.dat";
        }

        if (lower == "cat logs5.txt")
        {
            return @"[INFO] System boot
[DEBUG] No issues detected
[WARN] Strange activity in sector 7
[INFO] Routine check passed
[DEBUG] User login ok
[INFO] FAKE{FINAL_CTF_MASTER}
[INFO] Audit trail complete";
        }

        if (lower.StartsWith("grep "))
        {
            // very tiny fake grep
            if (lower == "grep fake logs5.txt")
            {
                return "FAKE{FINAL_CTF_MASTER}";
            }

            return "Your pattern does not catch anything interesting here.";
        }

        if (lower == "cat noise.dat")
        {
            return "Static. Lots of it. If there is anything meaningful in there, it hides well.";
        }

        return "The last answer has already been logged. Asking the file the right question will make it repeat it to you.";
    }
}
