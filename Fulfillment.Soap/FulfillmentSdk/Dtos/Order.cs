using System.Collections.Generic;
using Parsimotion.Integrations.Meli.Model.Entities.Orders;

namespace Fulfillment.Mercadolibre.FulfillmentSdk.Dtos
{
	public class Order
	{
		public long OrderId { get; set; }
		public BuyerWithAddress Buyer { get; set; }
		public ShipmentLabel ShipmentLabel { get; set; }
		public IEnumerable<Line> Lines { get; set; }
	}

	public class BuyerWithAddress : Buyer
	{
		public Address Address { get; set; }
	}

	public class ShipmentLabel
	{
		public string Zpl2 { get; set; }
	}
}
