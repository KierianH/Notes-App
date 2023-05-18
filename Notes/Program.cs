using System.Diagnostics;
using Newtonsoft.Json;

namespace Notes;

static class Program
{

    private static string _defaultEditor = "nvim";
    private static readonly string ConfigFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".notesconfig.json"
    );

    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide the filename as the first argument.");
            return;
        }
        var homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var notesPath = Path.Combine(homePath, "notes");
        var filePath = Path.Combine(notesPath, args[0]);

        if (args[0] == "--editor")
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Please specify the editor command.");
                return;
            }

            var config = new { Editor = args[1] };
            File.WriteAllText(ConfigFilePath, JsonConvert.SerializeObject(config));
            Console.WriteLine($"Default editor has been set to: {args[1]}");
            return;
        }

        
        if (args[0] == "ls")
        {
            ListNotes(notesPath);
            return;
        }
        if (args[0] == "update")
        {
            SyncWithGit(notesPath, pullOnly: true);
            Console.WriteLine("Notes updated from remote repository.");
            return;
        }
        if (args[0] == "--origin")
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Please specify the URL of the remote repository.");
                return;
            }

            SetOrigin(notesPath, args[1]);
            Console.WriteLine($"Origin has been set to: {args[1]}");
            return;
        }
        if (File.Exists(ConfigFilePath))
        {
            var config = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(ConfigFilePath));
            _defaultEditor = config!.Editor;
        }


        if (!Directory.Exists(notesPath))
        {
            Directory.CreateDirectory(notesPath);
        }
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Dispose();
        }
        // Pull the latest changes before opening the file
        SyncWithGit(notesPath, pullOnly: true);

        OpenFileInEditor(filePath, _defaultEditor);

        SyncWithGit(notesPath, pullOnly: false);
        
    }

    private static void OpenFileInEditor(string filePath, string editor)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = editor,
            ArgumentList = { filePath },
            UseShellExecute = true
        };

        using var process = Process.Start(startInfo);
        if (process != null) 
            process.WaitForExit();
    }
    private static void SetOrigin(string directoryPath, string remoteUrl)
    {
        RunCommand(directoryPath, "git", "init");
        RunCommand(directoryPath, "git", $"remote add origin {remoteUrl}");
    }

    private static bool ValidateGitSettings(string directoryPath)
    {
        var output = RunCommand(directoryPath, "git", "remote -v");
        if (!output.Contains("origin"))
        {
            Console.WriteLine("Remote 'origin' is not set. Please enter the URL of the remote repository:");
            string? remoteUrl = Console.ReadLine();
            RunCommand(directoryPath, "git", $"remote add origin {remoteUrl}");
        }

        output = RunCommand(directoryPath, "git", "branch");
        if (!output.Contains("* main"))
        {
            Console.WriteLine("Setting up 'main' branch...");
            RunCommand(directoryPath, "git", "checkout -b main");
        }

        return true;
    }

    private static void SyncWithGit(string directoryPath, bool pullOnly = false)
    {
        RunCommand(directoryPath, "git", "init");

        
        if (pullOnly)
        {
            if (ValidateGitSettings(directoryPath))
            {
                RunCommand(directoryPath, "git", "pull origin main");
            }
        }
        else
        {
            RunCommand(directoryPath, "git", "add .");
            RunCommand(directoryPath, "git", "commit -m \"Notes updated\"");

            if (ValidateGitSettings(directoryPath))
            {
                RunCommand(directoryPath, "git", "push origin main");
            }
        }
    }
    private static void ListNotes(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine("No notes directory found.");
            return;
        }

        var files = Directory.GetFiles(directoryPath);
        if (files.Length == 0)
        {
            Console.WriteLine("No notes found.");
            return;
        }

        foreach (var file in files.Select(Path.GetFileName))
        {
            Console.WriteLine(file);
        }
    }
    private static string RunCommand(string workingDirectory, string command, string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            WorkingDirectory = workingDirectory,
            FileName = command,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();
        process.WaitForExit();

        var output = process.StandardOutput.ReadToEnd();
        if (process.ExitCode != 0)
        {
            var errorMessage = process.StandardError.ReadToEnd();
            Console.WriteLine(errorMessage);
        }
        return output;
    }
}