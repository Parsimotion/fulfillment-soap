using System.Configuration;
using Parsimotion.Utils.Auth;

namespace Fulfillment.Mercadolibre.FulfillmentSdk.Configuration
{
	public class FulfillmentConfiguration
	{
		private readonly BasicAuthenticationConfiguration basicConfiguration;

		public FulfillmentConfiguration(BasicAuthenticationConfiguration basicConfiguration)
		{
			this.basicConfiguration = basicConfiguration;
		}

		public string MasterToken { get { return this.basicConfiguration.MasterToken; } }
		public string FulfillmentUrl { get { return ConfigurationManager.AppSettings["FulfillmentUrl"]; } }
	}
}