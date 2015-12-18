using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fulfillment.Mercadolibre.FulfillmentSdk.Configuration;
using Fulfillment.Mercadolibre.FulfillmentSdk.Dtos;
using Fulfillment.Mercadolibre.Models;
using Parsimotion.Utils.Functional;
using RestSharp;

namespace Fulfillment.Mercadolibre.FulfillmentSdk.Apis
{
	public class FulfillmentProductsApi : FulfillmentApi
	{
		public FulfillmentProductsApi(FulfillmentConfiguration config, User user) : base(config, user) {}

		public virtual async Task<IEnumerable<Product>> GetProductsLinkedWith(string listingId, long? variationId = null)
		{
			var parameters = new List<Parameter>{ new Parameter { Name = "listing_id", Value = listingId, Type = ParameterType.QueryString } };
			variationId.ToOption().Match(value => parameters.Add(new Parameter { Name = "variation_id", Value = value, Type = ParameterType.QueryString }));
			return await this.Get<IEnumerable<Product>>(Resource, parameters.ToArray());
		}

		public virtual async Task UpdateProduct(Product product)
		{
			await this.ExecuteAuthorizedRequest(Method.PUT, GetResource(product.Id), product);
		}

		private static string Resource
		{
			get { return "products"; }
		}

		public virtual async Task<Product> GetById(int id)
		{
			var resource = GetResource(id);
			return await this.Get<Product>(resource);
		}

		private static string GetResource(int id)
		{
			return String.Format(Resource + "/{0}", id);
		}
	}
}
