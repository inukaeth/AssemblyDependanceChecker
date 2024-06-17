using System.CommandLine;

namespace AssemblyDependancyChecker
{
    internal class Program
    {
        static  void  Main(string[] args)
        {
            var fileOption = new Option<DirectoryInfo?>(
           name: "--path",
           description: "The path to run the checker on");

            var rootCommand = new RootCommand("AssemblyDependancyChecker --path  <path_to_assemblies>");
            rootCommand.AddOption(fileOption);

            rootCommand.SetHandler((dir) =>
            {
                ReadFile(dir!);
            },
                fileOption);

            rootCommand.InvokeAsync(args).Wait();
        }

        static void ReadFile(DirectoryInfo dir)
        {
           Checker checker = new Checker();
            checker.Load(dir);
        }
    }
}