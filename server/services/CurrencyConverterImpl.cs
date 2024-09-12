using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using CurrencyConverter;

namespace server.services
{
    internal class CurrencyConverterImpl: CurrencyConverterService.CurrencyConverterServiceBase
    {
        public override Task<CurrencyConversionResponse> Convert(CurrencyConversionRequest request, ServerCallContext context)
        {
            var response = new CurrencyConversionResponse
            {
                From = request.From,
                To = request.To,
                InitialAmount = request.Amount,
                ConvertedAmount = request.Amount * 0.85
            };

            return Task.FromResult(response);
        }
    }
}
