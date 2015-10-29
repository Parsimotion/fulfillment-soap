using System.Collections.Generic;
using System.Web;

namespace Fulfillment.Soap
{
	public class Order
	{
		public long OrderId { get; set; }
		public BuyerWithAddress Buyer { get; set; }
		public Line[] Lines { get; set; }
	}

	public class BuyerWithAddress : Buyer
	{
		public Address Address { get; set; }
	}

	public class Line
	{
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public string Title { get; set; }
	}

	public class Phone
	{
		public string AreaCode { get; set; }
		public string Number { get; set; }
		public object Extension { get; set; }
	}

	public class BillingInfo
	{
		public string DocType { get; set; }
		public string DocNumber { get; set; }
	}

	public class Buyer
	{
		public long Id { get; set; }
		public string Nickname { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public Phone Phone { get; set; }
		public BillingInfo BillingInfo { get; set; }
		public Buyer()
		{
			this.Phone = new Phone();
			this.BillingInfo = new BillingInfo();
		}
	}

	public class Address
	{
		public string StreetName { get; set; }
		public string StreetNumber { get; set; }
		public string ZipCode { get; set; }
		public ObjectWithName City { get; set; }
		public ObjectWithName State { get; set; }
		public string Comment { get; set; }
		public string ReceiverName { get; set; }
		public string ReceiverPhone { get; set; }
	}

	public class ObjectWithName
	{
		public ObjectWithName() { }
		public string Name { get; set; }
	}
}
