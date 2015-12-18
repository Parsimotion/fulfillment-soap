using System;
using System.Collections.Generic;
using Fulfillment.Soap.FulfillmentSdk.Dtos;

namespace Fulfillment.Soap.FulfillmentSdk.Apis
{
	public class FulfillmentInboundsApi : FulfillmentApi
	{
		public FulfillmentInboundsApi(string username, string password) : base(username, password) { }

		private static string Resource
		{
			get { return "inbounds"; }
		}

		public virtual Inbound GetById(int id)
		{
			var resource = GetResource(id);
			return this.Get<Inbound>(resource);
		}

		public virtual IEnumerable<Inbound> Get()
		{
			return this.Get<IEnumerable<Inbound>>(Resource);
		}

		private static string GetResource(int id)
		{
			return String.Format(Resource + "/{0}", id);
		}
	}
}