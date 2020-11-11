using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using ShoppingApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApi.Services
{
    public class GrpcPickupEstimator : IGenerateCurbsidePickupTimes
    {
        private readonly IOptions<PickupEstimatorConfiguration> _options;

        public GrpcPickupEstimator(IOptions<PickupEstimatorConfiguration> options)
        {
            _options = options;
        }

        public async Task<DateTime?> GetPickupDate(CurbsideOrder order)
        {
            var request = new PickupService.PickupRequest
            {
                For = order.For,
            };
            request.Items.AddRange(order.Items.Split(',').Select(int.Parse).ToArray());
            try
            {

                var channel = GrpcChannel.ForAddress(_options.Value.Url);
                var client = new PickupService.PickupEstimator.PickupEstimatorClient(channel);

                var response = await client.GetPickupTimeAsync(request);

                return response.PickupTime.ToDateTime();
            }
            catch(Exception)
            {
                //our weak-sauce plan b.
                return DateTime.Now.AddDays(5);
            }
        }
    }
}
