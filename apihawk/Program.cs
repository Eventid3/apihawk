using API_Tester;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var cliHandler = new CLIHandler(args);

        return await cliHandler.Initiate();
    }
}
