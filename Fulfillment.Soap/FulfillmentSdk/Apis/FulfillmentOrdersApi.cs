using System;
using System.Collections.Generic;
using Fulfillment.Soap.FulfillmentSdk.Dtos;
using RestSharp;

namespace Fulfillment.Soap.FulfillmentSdk.Apis
{
	public class FulfillmentOrdersApi : FulfillmentApi
	{
		public FulfillmentOrdersApi(string username, string password) : base(username, password) {}

		public virtual IEnumerable<Order> GetUpdatedAfter(DateTime date)
		{
			var parameters = new Parameter { Name = "updated_at__gt", Value = date.ToString("O"), Type = ParameterType.QueryString };
			return this.Get<IEnumerable<Order>>("orders", parameters);
		}

		public Order GetById(int id)
		{
			return this.Get<Order>(String.Format("orders/{0}",id));
		}
	}
}
