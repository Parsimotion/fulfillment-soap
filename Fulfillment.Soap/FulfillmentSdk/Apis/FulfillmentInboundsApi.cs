using System;
using System.Collections.Generic;
using Fulfillment.Soap.FulfillmentSdk.Dtos;
using RestSharp;

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

		public virtual IEnumerable<Inbound> GetUpdatedAfter(DateTime date)
		{
			var parameters = new Parameter { Name = "updated_at__gt", Value = date.ToString("O"), Type = ParameterType.QueryString };
			return this.Get<IEnumerable<Inbound>>("orders", parameters);
		}

		private static string GetResource(int id)
		{
			return String.Format(Resource + "/{0}", id);
		}
	}
}