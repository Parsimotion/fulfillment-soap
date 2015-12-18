using System;

namespace Fulfillment.Soap.FulfillmentSdk.Dtos
{
	public class Product
	{
		public int Id { get; set; }
		public int WarehouseId { get; set; }
		public int AvailableQuantity { get; set; }
		public string ListingId { get; set; }
		public long? VariationId { get; set; }
		public DateTime CreationDate { get; set; }
		public string[] SerialNumbers { get; set; }

		public bool HasStock { get { return AvailableQuantity > 0; } }
	}
}
