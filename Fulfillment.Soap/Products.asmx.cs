using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Web.Services.Protocols;
using Fulfillment.Soap.FulfillmentSdk.Apis;
using Fulfillment.Soap.FulfillmentSdk.Dtos;

namespace Fulfillment.Soap
{
	/// <summary>
	/// Summary description for Products
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	// [System.Web.Script.Services.ScriptService]
	public class Products : WebService
	{
		public AuthenticationHeader AuthenticationInformation;

		public class AuthenticationHeader : SoapHeader
		{
			public string UserName;
			public string Password;
		}

		[WebMethod(Description = "Get a list of Products")]
		[SoapHeader("AuthenticationInformation")]
		public List<Product> Get()
		{
			var auth = this.AuthenticationInformation;
			var api = new FulfillmentProductsApi(auth.UserName, auth.Password);
			return api.Get().ToList();
		}

		[WebMethod(Description = "Get a Product by Id")]
		[SoapHeader("AuthenticationInformation")]
		public Product GetById(int id)
		{
			var auth = this.AuthenticationInformation;
			var api = new FulfillmentProductsApi(auth.UserName, auth.Password);
			return api.GetById(id);
		}
	}
}
