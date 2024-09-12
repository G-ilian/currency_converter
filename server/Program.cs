using Grpc.Core;
using server.services;
using CurrencyConverter;

namespace server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Server server = null;
            try
            {
                server = new Server
                {
                    Services = { CurrencyConverterService.BindService(new CurrencyConverterImpl()) },
                    Ports = { new ServerPort("localhost", 5000, ServerCredentials.Insecure) }
                };
                server.Start();
                Console.WriteLine("Server listening on port 50051. Press any key to stop the server...");
                Console.ReadKey();

            }
            catch(IOException e)
            {
                Console.WriteLine("An error occured: " + e.Message);
            }finally
            {
                if(server != null)
                {
                    server.ShutdownAsync().Wait();
                }
            }
        }
    }
}
