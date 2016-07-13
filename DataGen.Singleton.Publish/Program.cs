using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataGen.Extensions.Publish
{
    class Program
    {
        private static IEnumerable<string> BuildConfigurations
        {
            get
            {
                return GetBuildConfigurations();
            }
        }

        static void Main(string[] args)
        {
            DisplayMainMenu();
        }

        private static IDictionary<int, string> GetProducts()
        {
            return new Dictionary<int, string>()
            {
                { 1, "DataGen.Extensions"},
                { 2, "DataGen.RomanNumerals"},
                { 3, "DataGen.NumberToWords"},
                { 4, "DataGen.Cryptography"},
            };
        }

        private static IEnumerable<string> GetBuildConfigurations()
        {
            yield return "Release 3.5";
            yield return "Release 4.0";
            yield return "Release 4.5";
            yield return "Release 4.6";
        }

        private static string ProductName = "DataGen.Singleton";

        private static Version CurrentVersion = GetAssemblyCurrentVersion();

        private static string GetCurrentVersionString()
        {
            if (CurrentVersion.IsNull())
                return string.Empty;

            return string.Format("{0}.{1}.{2}", CurrentVersion.Major, CurrentVersion.Minor, CurrentVersion.Build);
        }

        private static void GetUserCommand(Action<string> menuCommandHandler)
        {
            var key = Console.ReadKey();
            Console.WriteLine();
            menuCommandHandler(key.KeyChar.ToString());
        }

        private static void DisplayMainMenu()
        {
            Console.WriteLine();
            Console.WriteLine(string.Format("---{0} Main menu---", ProductName));
            Console.WriteLine("1 - Change verion");
            Console.WriteLine("2 - Rebuild");
            Console.WriteLine("3 - Publish to NuGet");
            Console.WriteLine("0 - Product menu");

            GetUserCommand(new Action<string>(HandleMainMenuCommand));
        }

        private static void HandleMainMenuCommand(string command)
        {
            switch (command)
            {
                case "1":
                    ChangeVersion();
                    break;
                case "2":
                    Rebuild();
                    break;
                case "3":
                    Publish();
                    break;
                case "0":
                    Environment.Exit(0);
                    break;
                default:
                    WrongCommand();
                    break;
            }

            DisplayMainMenu();
        }

        private static void Rebuild()
        {
            Console.WriteLine("--Rebuilding--");
            foreach (var buildConfiguration in BuildConfigurations)
            {
                RebuildProjectWithConfiguration(buildConfiguration);
            }
        }

        private static void RebuildProjectWithConfiguration(string configurationName)
        {
            string fileName = "C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319\\msbuild.exe";
            string arguments = "..\\..\\..\\" + ProductName + "\\" + ProductName + ".csproj /nologo /v:m /t:Rebuild /p:WarningLevel=0 /property:Configuration=\"" + configurationName + "\";Platform=\"AnyCPU\"";
            Process(fileName, arguments);
        }

        private static Version GetAssemblyCurrentVersion()
        {
            string fileContent;
            var fileName = "..\\..\\..\\" + ProductName + "\\Properties\\AssemblyInfo.cs";
            string regexPattern = "\\[assembly: AssemblyVersion\\(\\\"\\d+\\.\\d+\\.\\d+.\\*\\\"\\)\\]";
            Match match = GetVersionRegexMatch(fileName, regexPattern, out fileContent);
            if (match.Success)
            {
                match = Regex.Match(match.Value, "\\d+\\.\\d+\\.\\d+");
                if (match.Success)
                    return new Version(match.Value);
            }

            return new Version();
        }

        private static Match GetVersionRegexMatch(string fileName, string regexPattern, out string fileContent)
        {
            fileContent = File.ReadAllText(fileName);
            var match = Regex.Match(fileContent, regexPattern);
            return match;
        }

        private static void DisplayChangeVersionMenu()
        {
            Console.WriteLine();
            Console.WriteLine(string.Format("---{0} Change version menu---", ProductName));
            Console.WriteLine("1 - Increase build version number");
            Console.WriteLine("2 - Increase minor version number");
            Console.WriteLine("3 - Increase major version number");
            Console.WriteLine("4 - Change version");
            Console.WriteLine("5 - Show current version");
            Console.WriteLine("6 - Commit current version");
            Console.WriteLine("0 - Main menu");

            GetUserCommand(new Action<string>(HandleChangeVersionMenuCommand));
        }

        private static void HandleChangeVersionMenuCommand(string command)
        {
            switch (command)
            {
                case "1":
                    IncreaseBuildVersionNumber();
                    ChangeVersionInFiles();
                    break;
                case "2":
                    IncreaseMinorVersionNumber();
                    ChangeVersionInFiles();
                    break;
                case "3":
                    IncreaseMajorVersionNumber();
                    ChangeVersionInFiles();
                    break;
                case "4":
                    EnterVersion();
                    ChangeVersionInFiles();
                    break;
                case "5":
                    ShowCurrentVersion();
                    break;
                case "6":
                    CommitCurrentVersion();
                    break;
                case "0":
                    DisplayMainMenu();
                    break;
                default:
                    WrongCommand();
                    break;
            }

            DisplayChangeVersionMenu();
        }

        private static void IncreaseBuildVersionNumber()
        {
            CurrentVersion = new Version(string.Format("{0}.{1}.{2}", CurrentVersion.Major, CurrentVersion.Minor, CurrentVersion.Build + 1));
        }

        private static void IncreaseMinorVersionNumber()
        {
            CurrentVersion = new Version(string.Format("{0}.{1}.{2}", CurrentVersion.Major, CurrentVersion.Minor + 1, 0));
        }

        private static void IncreaseMajorVersionNumber()
        {
            CurrentVersion = new Version(string.Format("{0}.{1}.{2}", CurrentVersion.Major + 1, 0, 0));
        }

        private static void EnterVersion()
        {
            Console.WriteLine("Enter version in format 'Major.Minor.Build'.");
            string version = Console.ReadLine();
            if (Regex.Match(version, "^\\d+\\.\\d+\\.\\d+$").Success)
            {
                CurrentVersion = new Version(version);
            }
            else
            {
                Console.WriteLine("Wrong format!");
                DisplayChangeVersionMenu();
            }
        }

        private static void ShowCurrentVersion()
        {
            Console.WriteLine(string.Format("Current version: {0}", GetCurrentVersionString()));
        }

        private static void CommitCurrentVersion()
        {
            Console.WriteLine("--Commiting changed version--");
            Git("add ..\\..\\..\\" + ProductName + "\\Properties\\AssemblyInfo.cs");
            Git("add ..\\..\\..\\NuGet\\" + ProductName + ".nuspec");
            Git("commit -m \"Change version\"");
        }

        private static void Git(string arguments)
        {
            string fileName = "C:\\Program Files\\Git\\cmd\\git.exe";
            Process(fileName, arguments);
        }

        private static void Process(string fileName, string arguments)
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            process.Start();
            while (!process.StandardOutput.EndOfStream)
            {
                string processOutput = process.StandardOutput.ReadLine();
                Console.WriteLine(processOutput);
            }
        }

        private static void ChangeVersion()
        {
            DisplayChangeVersionMenu();
        }

        private static void ChangeVersionInFiles()
        {
            Console.WriteLine("--Changing version--");
            ChangeAssemblyInfoVerion();
            ChangeNuSpecVersion();
            ShowCurrentVersion();
        }

        private static void ChangeAssemblyInfoVerion()
        {
            string fileName = "..\\..\\..\\" + ProductName + "\\Properties\\AssemblyInfo.cs";
            string regexPattern = "\\[assembly: AssemblyVersion\\(\\\"\\d+\\.\\d+\\.\\d+.\\*\\\"\\)\\]";
            string versionPlaceholder = "[assembly: AssemblyVersion(\"{0}.*\")]";
            ChangeVersionInFile(fileName, regexPattern, versionPlaceholder);
        }

        private static void ChangeNuSpecVersion()
        {
            string fileName = "..\\..\\..\\NuGet\\" + ProductName + ".nuspec";
            string regexPattern = "<version>\\d+\\.\\d+\\.\\d+</version>";
            string versionPlaceholder = "<version>{0}</version>";
            ChangeVersionInFile(fileName, regexPattern, versionPlaceholder);
        }

        private static void ChangeVersionInFile(string fileName, string regexPattern, string placeholder)
        {
            string fileContent;

            Match match = GetVersionRegexMatch(fileName, regexPattern, out fileContent);
            if (match.Success)
            {
                fileContent = fileContent.Replace(match.Value, string.Format(placeholder, GetCurrentVersionString()));
            }

            File.WriteAllText(fileName, fileContent);
        }

        private static void Publish()
        {
            Console.WriteLine("--Publishing--");
            PackNuspec();
            PushNupkg();
        }

        private static void PackNuspec()
        {
            string fileName = "..\\..\\..\\Tools\\nuget.exe";
            string arguments = "pack ..\\..\\..\\NuGet\\" + ProductName + ".nuspec -OutputDirectory \"..\\..\\..\\NuGet\"";
            Process(fileName, arguments);
        }

        private static void PushNupkg()
        {
            string fileName = "..\\..\\..\\Tools\\nuget.exe";
            string arguments = string.Format("push ..\\..\\..\\NuGet\\" + ProductName + ".{0}.nupkg", GetCurrentVersionString());
            Process(fileName, arguments);
        }

        private static void WrongCommand()
        {
            Console.WriteLine("Wrong command! Try again.");
        }
    }
}
