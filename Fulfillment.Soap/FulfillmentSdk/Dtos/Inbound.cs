using System;

namespace Fulfillment.Soap.FulfillmentSdk.Dtos
{
	public enum InboundStatus
	{
		InTransit, ReceivedOk, ReceivedWithDifferences, Cancelled
	}

	public class InboundLine
	{
		public ProductWithoutSerialNumbers Product { get; set; }
		public int ShippedQuantity { get; set; }
		public int ReceivedQuantity { get; set; }
		public string[] SerialNumbers { get; set; }
	}

	public class Inbound
	{
		public int Id { get; set; }
		public string ShippingMethod { get; set; }
		public string TrackingNumber { get; set; }
		public InboundStatus Status { get; set; }
		public DateTime CreationDate { get; set; }
		public DateTime Eta { get; set; }
		public DateTime ReceptionDate { get; set; }
		public bool Cancelled { get; set; }
		public InboundLine[] Lines { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}