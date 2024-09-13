
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
            var toCurrencies = new List<Currency>
            {
                new Currency { Code = "EUR" },  // Euro
                new Currency { Code = "GBP" },  // Libra Esterlina
                new Currency { Code = "JPY" },  // Iene Japonês
                new Currency { Code = "BRL" },   // Real Brasileiro
                new Currency { Code = "CADD" },   // Dólar Canadense
            };
            var response = await client.ConvertAsync(new CurrencyConversionRequest
            {
                From = new Currency { Code = "USD" },
                To = {toCurrencies},
                Amount = 1
            });

            try { 

                foreach (var currency in response.ConvertedAmounts)
                {
                    Console.WriteLine($"{currency.Key}: {currency.Value}");
                }
            
            }catch(RpcException e)
            {
                Console.WriteLine($"An error occured: {e.Message}");
            }
            
        }
    }
}
