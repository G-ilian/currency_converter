
using CurrencyConverter;
using Grpc.Core;
using Grpc.Net.Client;

namespace client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            GrpcChannel channel = GrpcChannel.ForAddress("http://localhost:5000");
            
            await ConvertCurrency(channel);
        }

        private static async Task ConvertCurrency(GrpcChannel channel)
        {
            var client = new CurrencyConverterService.CurrencyConverterServiceClient(channel);

            var response = await client.ConvertAsync(new CurrencyConversionRequest
            {
                From = new Currency { Code = "USD" },
                To = new Currency { Code = "EUR" },
                Amount = 100
            });

            Console.WriteLine($"100 USD is {response.ConvertedAmount} EUR");
        }
    }
}
