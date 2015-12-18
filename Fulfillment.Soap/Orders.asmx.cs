using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;

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

		[WebMethod(Description = "Get a list of Orders with id newer than the given id")]
		[SoapHeader("AuthenticationInformation")]
		public List<Order> GetOrdersNewerThan(int id)
		{
			AuthenticateRequest();
			return new List<Order> { this.Order, this.Order };
		}

		private void AuthenticateRequest()
		{
			var auth = this.AuthenticationInformation;
			if (auth == null || auth.UserName != "username" || auth.Password != "password")
				throw new UnauthorizedSoapHeaderException();
		}

		private Order Order
		{
			get
			{
				return new Order
				{
					OrderId = 1234,
					Buyer = new BuyerWithAddress
					{
						Nickname = "JUANPEREZ",
						FirstName = "Juan",
						LastName = "Perez",
						Email = "juanperez@hotmail.com",
						Phone = new Phone
						{
							AreaCode = "11",
							Number = "4321-0987"
						},
                        Address = new Address
						{
							ZipCode = "36580",
							StreetName = "Saraza",
							StreetNumber = "1234",
							State = new ObjectWithName { Name = "Guanajuato" },
							Comment = "Piso 7 departamento D",
							City = new ObjectWithName { Name = "Irapuato" }
						},
						BillingInfo = new BillingInfo
						{
							DocType = "DNI",
                            DocNumber = "12345678"
						},
						Id = 8877665544
					},
					Lines = new[]
					{
						new Line
						{
							ProductId = 1234,
							Title = "MercadoPago POS Device",
							Quantity = 1,
							SerialNumber = "1234567890"
						}
					}
				};

			}
		}
	}
}
