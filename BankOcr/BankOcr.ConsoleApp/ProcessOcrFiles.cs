using Spectre.Console;
using System.Reflection;
using BankOcr.Business.Models;
using BankOcr.Business.Services;

namespace BankOcr.ConsoleApp;

public class ProcessOcrFiles
{
    private readonly OcrService _orcService;

    public ProcessOcrFiles()
    {
        _orcService = new OcrService(new AccountNumberService());
    }

    public void MainMenu()
    {
        var files = GetFileNames();
        files.Insert(0, "Refresh");

        var file = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Please select a [green]file[/] To refresh this list choose 'refresh'")
                .PageSize(50)
                .MoreChoicesText("[grey](Move up and down to reveal more fruits)[/]")
                .AddChoices(files));

        if (file == "Refresh")
        {
            MainMenu();
        }
        else
        {
            DisplayFileContents(file);
        }
    }

    private List<string> GetFileNames()
    {
        // load all the file names found in the ./ocr-files directory   
        var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (assemblyDirectory == null)
        {
            AnsiConsole.Write(new Markup("[red]Unable to get the assembly directory[/]"));
            return new List<string>();
        }
        var directoryPath = Path.Combine(assemblyDirectory, "ocr-files");
        if (Directory.Exists(directoryPath))
        {
            var filePaths = Directory.GetFiles(directoryPath);
            var fileNames = filePaths.Select(path => Path.GetFileName(path)).ToList();
            return fileNames;
        }

        AnsiConsole.Write(new Markup($"[red]Directory {directoryPath} does not exist.[/]"));
        return new List<string>();
    }

    private void DisplayFileContents(string fileName)
    {
        var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (assemblyDirectory == null)
        {
            AnsiConsole.Write(new Markup("[red]Unable to get the assembly directory[/]"));
            return;
        }
        var filePath = Path.Combine(assemblyDirectory, "ocr-files", fileName);
        if (File.Exists(filePath))
        {
            var fileContents = File.ReadAllText(filePath).Replace("\r\n", "\n");

            var validationResult = _orcService.ValidateOcrFile(fileContents);

            if (!validationResult.IsValid)
            {
                DisplayValidationErrors(validationResult);
                return;
            }

            var accountNumbers = _orcService.GetAccountNumbersFromOcrFileContents(fileContents);

            // Create a table
            var table = new Table();

            // Add some columns
            table.AddColumn("Account number");
            table.AddColumn("Status");
            table.AddColumn("Possible numbers");

            // Add some rows
            accountNumbers.ForEach(accountNumber => table.AddRow(
                accountNumber.Data.Number, 
                accountNumber.Data.StatusFriendlyMessage,
                accountNumber.PossibleMatches != null ? string.Join(", ", accountNumber.PossibleMatches) : string.Empty)
            );

            // Render the table to the console
            AnsiConsole.Write(table);

            AnsiConsole.Markup("If you wish to load another file please press [green]space[/] otherwise press any other key to exit");

            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Spacebar)
            {
                Console.Clear();
                MainMenu();
            }
        }
        else
        {
            AnsiConsole.Write(new Markup($"[red]File {filePath} does not exist.[/]"));
        }
    }

    private void DisplayValidationErrors(OcrFileValidationResult validationResult)
    {
        Console.Clear();
        AnsiConsole.Markup($"[red]{validationResult.ValidationFailure}[/]");
        Console.WriteLine();
        AnsiConsole.Markup("If you wish to load another file please press [green]space[/] otherwise press any other key to exit");

        var key = Console.ReadKey();
        if (key.Key == ConsoleKey.Spacebar)
        {
            Console.Clear();
            MainMenu();
        }
    }

}