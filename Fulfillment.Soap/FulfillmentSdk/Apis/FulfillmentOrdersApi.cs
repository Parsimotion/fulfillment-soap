using System.Net;
using System.Threading.Tasks;
using Fulfillment.Mercadolibre.FulfillmentSdk.Configuration;
using Fulfillment.Mercadolibre.FulfillmentSdk.Dtos;
using Integrations.Mercadolibre.Sdk.Entities;
using Parsimotion.Utils.Exceptions;
using RestSharp;

namespace Fulfillment.Mercadolibre.FulfillmentSdk.Apis
{
	public class FulfillmentOrdersApi : FulfillmentApi
	{
		public FulfillmentOrdersApi(FulfillmentConfiguration config, MeliUserInformation user) : base(config, user) {}

		public virtual async Task<Order> Create(Order order)
		{
			await this.Post("orders", order);
			return order;
		}

		protected override void Validate(IRestResponse response)
		{
			if (response.StatusCode == HttpStatusCode.Conflict)
				throw new EntityAlreadyExistsException<Order>();
		}
	}
}
