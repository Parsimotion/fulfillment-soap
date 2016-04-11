using System.Web.Services.Protocols;

namespace Fulfillment.Soap
{
	public class InvalidSerialNumberException : SoapException
	{
		public InvalidSerialNumberException() : base("Invalid Serial Number", ServerFaultCode) { }
	}
}