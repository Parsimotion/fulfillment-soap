using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Fulfillment.Soap.FulfillmentSdk.Dtos
{
	public enum InboundStatus
	{
		Pending, InTransit, ReceivedOk, ReceivedWithDifferences, Cancelled
	}

	public class SerialNumber
	{
		public string From { get; set; }
		public string To { get; set; }
		public long FromNumber { get; set; }
		public long ToNumber { get; set; }
		public string Prefix { get; set; }
	}

	public class InboundLine
	{
		public ProductWithoutSerialNumbers Product { get; set; }
		public int ShippedQuantity { get; set; }
		public int ReceivedQuantity { get; set; }

		[XmlIgnore]
		public SerialNumber[] SerialNumbers { get; set; }

		[XmlArray("SerialNumbers")]
		public string[] ExplodedSerialNumbers { get { return this.SerialNumbers.SelectMany(ExplodeSerialNumbers).ToArray(); } set {throw new ApplicationException();} }

		private static IEnumerable<string> ExplodeSerialNumbers(SerialNumber serialNumber)
		{
			for (var i = serialNumber.FromNumber; i <= serialNumber.ToNumber; i++)
			{
				yield return serialNumber.Prefix + i;
			}
		}

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
