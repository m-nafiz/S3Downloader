using System.Threading.Tasks;

namespace S3Downloader
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            CLIManager inputManager = new CLIManager();
            await inputManager.TakeInputs();
        }
    }
}
