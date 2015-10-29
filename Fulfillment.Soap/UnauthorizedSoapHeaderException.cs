using System.Web.Services.Protocols;

namespace Fulfillment.Soap
{
	public class UnauthorizedSoapHeaderException : SoapHeaderException
	{
		public UnauthorizedSoapHeaderException() : base("Unauthorized", ServerFaultCode){ }
	}
}