using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using Fulfillment.Soap.FulfillmentSdk.Apis;
using Fulfillment.Soap.FulfillmentSdk.Dtos;
using Omu.ValueInjecter;
using RestSharp.Extensions;

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
		public List<OrderWithCustomsNumber> GetUpdatedAfter(DateTime date)
		{
			var auth = this.AuthenticationInformation;
			var api = new FulfillmentOrdersApi(auth.UserName, auth.Password);
			var orders = api.GetUpdatedAfter(date);
			return this.ToOrderWithCustomsNumber(orders);
		}

		private List<OrderWithCustomsNumber> ToOrderWithCustomsNumber(IEnumerable<Order> orders)
		{
			var auth = this.AuthenticationInformation;
			var inboundsApi = new FulfillmentInboundsApi(auth.UserName, auth.Password);
			var serialNumbers = orders.SelectMany(it => it.Lines.SelectMany(line => line.SerialNumbers)).ToList();
			var findBySerialNumberPageSize = int.Parse(ConfigurationManager.AppSettings["FindBySerialNumberPageSize"]);
            var chunks = Split(serialNumbers, findBySerialNumberPageSize);
			var inbounds = chunks
				.SelectMany(chunk => inboundsApi.SearchBySerialNumber(chunk)).ToList();

			return orders.Select(it => this.Transform(it, inbounds)).ToList();
		}

		public static List<List<T>> Split<T>(List<T> collection, int size)
		{
			var chunks = new List<List<T>>();
			var chunkCount = collection.Count() / size;

			if (collection.Count % size > 0)
				chunkCount++;

			for (var i = 0; i < chunkCount; i++)
				chunks.Add(collection.Skip(i * size).Take(size).ToList());

			return chunks;
		}

		private OrderWithCustomsNumber Transform(Order order, IEnumerable<Inbound> inbounds)
		{
			var result = new OrderWithCustomsNumber();
			result.InjectFrom(order);
			result.Lines = order.Lines.Select(line => this.Transform(line, inbounds)).ToArray();
			return result;
		}

		private LineWithCustomsNumber Transform(Line line, IEnumerable<Inbound> inbounds)
		{
			var result = new LineWithCustomsNumber();
			result.ProductId = line.ProductId.Id;
			result.Quantity = line.Quantity;
			result.Price = line.Price;
			result.Title = line.ProductId.Title;
			result.SerialNumbers = line.SerialNumbers.Select(it => this.Transform(it, inbounds)).ToArray();
			return result;
		}

		private SerialNumberWithCustomsNumber Transform(string serialNumber, IEnumerable<Inbound> inbounds)
		{
			var result = new SerialNumberWithCustomsNumber();
			try
			{
				result.SerialNumber = serialNumber;
				var parsedSerialNumber = ParseSerialNumber(serialNumber);
				var inbound = inbounds.First(it => it.Lines.Any(line => line.SerialNumbers.Any(range => Contains(range, parsedSerialNumber))));
				result.CustomsNumber = inbound.ShippingMethod;
				result.CustomsDate = inbound.Eta;
				return result;
			}
			catch (InvalidOperationException)
			{
				throw new InvalidSerialNumberException();
			}
		}

		private bool Contains(SerialNumber range, Tuple<string, long> parsedSerialNumber)
		{
			var number = parsedSerialNumber.Item2;
			return range.Prefix == parsedSerialNumber.Item1 && range.FromNumber <= number && range.ToNumber >= number;
		}

		private Tuple<string, long> ParseSerialNumber(string serialNumber)
		{
			var match = Regex.Matches(serialNumber, @"(\d+|[^\d]+)");
			var number = match[match.Count - 1].Value;
			var prefix = serialNumber.Replace(number, "");
			return new Tuple<string, long>(prefix, long.Parse(number));
		}

		[WebMethod(Description = "Get an Order by Id")]
		[SoapHeader("AuthenticationInformation")]
		public OrderWithCustomsNumber GetById(int id)
		{
			var auth = this.AuthenticationInformation;
			var api = new FulfillmentOrdersApi(auth.UserName, auth.Password);
			var order = api.GetById(id);
			return this.ToOrderWithCustomsNumber(new [] { order }).First();
		}
	}
}
