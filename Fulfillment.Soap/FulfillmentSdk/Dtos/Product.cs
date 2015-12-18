using System;

namespace Fulfillment.Soap.FulfillmentSdk.Dtos
{
	public class Product : ProductWithoutSerialNumbers
	{
		public string[] SerialNumbers { get; set; }
	}

	public class ProductWithoutSerialNumbers
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public int AvailableQuantity { get; set; }
		public string ListingId { get; set; }
		public long? VariationId { get; set; }
		public DateTime CreationDate { get; set; }

		public bool HasStock { get { return AvailableQuantity > 0; } }
	}
}
