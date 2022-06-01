namespace v0;

public static class Prompt
{
    public static string Ask(string message, string defaultVal = "") => Ask(message, defaultVal, s => s);
    public static T Ask<T>(string message, T defaultVal, Func<string, T> converter)
    {
        Console.Write($"{message}\t");

        var input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? defaultVal : converter(input);
    }

    public static void Print(string message) => Console.WriteLine(message);
}
