using CliFx;

namespace bitcoin_from_scratch
{
    public class Program
    { 
        public static async Task<int> Main(string[] args)
        {
            return await new CliApplicationBuilder()
                .AddCommandsFromThisAssembly()
                .Build()
                .RunAsync();
        }
    }
}
