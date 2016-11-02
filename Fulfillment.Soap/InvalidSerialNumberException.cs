using System.Web.Services.Protocols;

namespace Fulfillment.Soap
{
	public class InvalidSerialNumberException : SoapException
	{
		public InvalidSerialNumberException(string serialNumber) : base($"Invalid Serial Number: {serialNumber}", ServerFaultCode) { }
	}
}