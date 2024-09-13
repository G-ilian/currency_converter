using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using CurrencyConverter;
using System.Text.Json;
using server.enums;

namespace server.services
{
    internal class CurrencyConverterImpl: CurrencyConverterService.CurrencyConverterServiceBase
    {
        private readonly HttpClient _httpClient = new HttpClient();

        private bool isValidCurrencyCode(string code)
        {
            return Enum.TryParse(typeof(currencyCode), code.ToUpper(),out _);
        }

        public override Task<CurrencyConversionResponse> Convert(CurrencyConversionRequest request, ServerCallContext context)
        {
            string apiKey = "KEY_PADRAO";
            string url = $"https://api.currencyapi.com/v3/latest";

            HttpResponseMessage httpResponse;

            if (!isValidCurrencyCode(request.From.Code) || !isValidCurrencyCode(request.To.Code))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Código de moeda não suportado ou inválido"));
            }

            try
            {
                httpResponse = _httpClient.GetAsync($"{url}?apikey={apiKey}&base_currency={request.From.Code}&currencies={request.To.Code}").Result;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Falha na requisição HTTP: {ex.Message}"));
            }

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new RpcException(new Status(StatusCode.Internal, "Falha ao chamar a API de conversão de moedas."));
            }

            string responseContent = httpResponse.Content.ReadAsStringAsync().Result;

            var responseJson = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);

            var data = responseJson["data"] as JsonElement?;

            if (data == null)
            {
                throw new RpcException(new Status(StatusCode.Internal, "Resposta inválida da API de conversão de moedas."));
            }

            var currencyData = data?.GetProperty(request.To.Code);
            double conversionRate = currencyData?.GetProperty("value").GetDouble() ?? 0.0;
            double convertedAmount = request.Amount * conversionRate;

            var response = new CurrencyConversionResponse
            {
                From = request.From,
                To = request.To,
                InitialAmount = request.Amount,
                ConvertedAmount = convertedAmount
            };

            return Task.FromResult(response);
        }
    }
}
