using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Web.Services.Protocols;
using Fulfillment.Soap.FulfillmentSdk.Apis;
using Fulfillment.Soap.FulfillmentSdk.Dtos;

namespace Fulfillment.Soap
{
	/// <summary>
	/// Summary description for Inbounds
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	// [System.Web.Script.Services.ScriptService]
	public class Inbounds : WebService
	{
		public AuthenticationHeader AuthenticationInformation;

		public class AuthenticationHeader : SoapHeader
		{
			public string UserName;
			public string Password;
		}

		[WebMethod(Description = "Get a list of Inbounds")]
		[SoapHeader("AuthenticationInformation")]
		public List<Inbound> GetUpdatedAfter(DateTime date)

		{
			var auth = this.AuthenticationInformation;
			var api = new FulfillmentInboundsApi(auth.UserName, auth.Password);
			return api.GetUpdatedAfter(date).ToList();
		}

		[WebMethod(Description = "Get a Inbound by Id")]
		[SoapHeader("AuthenticationInformation")]
		public Inbound GetById(int id)
		{
			var auth = this.AuthenticationInformation;
			var api = new FulfillmentInboundsApi(auth.UserName, auth.Password);
			return api.GetById(id);
		}
	}
}
