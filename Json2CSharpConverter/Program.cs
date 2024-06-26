using Json2CSharpConverter.Core;

if (args.Length == 0)
{
    Console.WriteLine("Missing command-line argument: Path to input JSON file");
    return;
}

string inputJsonFile = args[0];
try
{
    string content = File.ReadAllText(inputJsonFile);
    var jsonConverter = new JsonToCSharpConverter
    {
        Utf8WriterVariableName = "jsonWriter",
        EmitFlush = true,
        EmitSetup = true
    };
    Console.WriteLine(jsonConverter.Convert(content));
}
catch (FileNotFoundException)
{
    Console.WriteLine($"Cannot find file {inputJsonFile}");
}
catch (Exception e)
{
    Console.WriteLine("An error has occurred:");
    Console.WriteLine(e.ToString());
}
