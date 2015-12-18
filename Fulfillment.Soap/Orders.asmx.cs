using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using Fulfillment.Soap.FulfillmentSdk.Apis;
using Fulfillment.Soap.FulfillmentSdk.Dtos;

namespace Fulfillment.Soap
{
	/// <summary>
	/// Summary description for Orders
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	// [System.Web.Script.Services.ScriptService]
	public class Orders : System.Web.Services.WebService
	{
		public AuthenticationHeader AuthenticationInformation;

		public class AuthenticationHeader : SoapHeader
		{
			public string UserName;
			public string Password;
		}

		[WebMethod(Description = "Get a list of Orders that have been updated after the given date in ISO 8601 Format")]
		[SoapHeader("AuthenticationInformation")]
		public List<Order> GetUpdatedAfter(DateTime date)
		{
			var auth = this.AuthenticationInformation;
			var api = new FulfillmentOrdersApi(auth.UserName, auth.Password);
			return api.GetUpdatedAfter(date).ToList();
		}

		[WebMethod(Description = "Get an Order by Id")]
		[SoapHeader("AuthenticationInformation")]
		public Order GetById(int id)
		{
			var auth = this.AuthenticationInformation;
			var api = new FulfillmentOrdersApi(auth.UserName, auth.Password);
			return api.GetById(id);
		}
	}
}
