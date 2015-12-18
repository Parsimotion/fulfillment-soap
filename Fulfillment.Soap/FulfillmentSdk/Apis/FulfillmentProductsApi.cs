using System;
using System.Collections.Generic;
using Fulfillment.Soap.FulfillmentSdk.Dtos;

namespace Fulfillment.Soap.FulfillmentSdk.Apis
{
	public class FulfillmentProductsApi : FulfillmentApi
	{
		public FulfillmentProductsApi(string username, string password) : base(username, password) {}

		private static string Resource
		{
			get { return "products"; }
		}

		public virtual Product GetById(int id)
		{
			var resource = GetResource(id);
			return this.Get<Product>(resource);
		}

		public virtual IEnumerable<Product> Get()
		{
			return this.Get<IEnumerable<Product>>(Resource);
		}

		private static string GetResource(int id)
		{
			return String.Format(Resource + "/{0}", id);
		}
	}
}
