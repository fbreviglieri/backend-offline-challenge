using FileReaderLibrary;
using FileReaderLibrary.Encryption;
using FileReaderLibrary.Security;

var factory = new FileReaderFactory(new ReverseEncryptionStrategy(), new SimpleRoleAuthorizationService());

Console.WriteLine("File Reader Library — interactive CLI");
Console.WriteLine("(admin can read anything; user can read .txt; auditor can read .txt/.xml)");
Console.WriteLine();

var keepGoing = true;
while (keepGoing)
{
    try
    {
        var fileType = PromptFileType();
        var encrypted = PromptYesNo("Is the file encrypted?");
        var roleSecured = PromptYesNo("Use role-based security?");
        var role = roleSecured ? PromptNonEmpty("Your role: ") : null;
        var path = PromptNonEmpty("Path to the file: ");

        var reader = factory.Create(fileType, encrypted, roleSecured, role);
        var content = reader.Read(path);

        Console.WriteLine();
        Console.WriteLine("--- File content ---");
        Console.WriteLine(content);
        Console.WriteLine("--------------------");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }

    Console.WriteLine();
    keepGoing = PromptYesNo("Read another file?");
    Console.WriteLine();
}

static FileType PromptFileType()
{
    while (true)
    {
        Console.Write("File type (text/xml/json): ");
        var input = Console.ReadLine()?.Trim().ToLowerInvariant();
        switch (input)
        {
            case "text": return FileType.Text;
            case "xml": return FileType.Xml;
            case "json": return FileType.Json;
            default:
                Console.WriteLine("Please enter 'text', 'xml', or 'json'.");
                break;
        }
    }
}

static bool PromptYesNo(string question)
{
    while (true)
    {
        Console.Write($"{question} (y/n): ");
        var input = Console.ReadLine()?.Trim().ToLowerInvariant();
        if (input is "y" or "yes") return true;
        if (input is "n" or "no") return false;
        Console.WriteLine("Please answer 'y' or 'n'.");
    }
}

static string PromptNonEmpty(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        var input = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(input)) return input;
        Console.WriteLine("This cannot be empty.");
    }
}
