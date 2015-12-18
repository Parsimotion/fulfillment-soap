using System;
using System.Linq;
using Fulfillment.Mercadolibre.FulfillmentSdk.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Fulfillment.Mercadolibre.FulfillmentSdk.Apis
{
	public abstract class FulfillmentApi : RestApi
	{
		private readonly IDeserializer deserializer;

		protected FulfillmentApi(FulfillmentConfiguration config, MeliUserInformation user) : base(config.FulfillmentUrl)
		{
			this.deserializer = new SnakeCaseJsonConverter();
			this.RestClient.Authenticator = new HttpBasicAuthenticator(user.UserId.ToString(), config.MasterToken);
		}
		
		protected override T DeserializeResponse<T>(IRestResponse response)
		{
			return this.deserializer.Deserialize<T>(response);
		}

		protected override void Authorize(RestRequest request) { }

		protected override RestRequest BuildRequest(Method method, string resource)
		{
			var request = base.BuildRequest(method, resource);
			request.JsonSerializer = new SnakeCaseJsonConverter();
			return request;
		}
	}

	public abstract class RestApi : BasicRestApi
	{
		protected RestApi(string apiBaseUrl) : base(apiBaseUrl) { }
		protected void ExecuteAuthorizedRequest(Method method, string resource, object body)
		{
			var request = this.BuildRequest(method, resource);
			request.AddBody(body);
			this.ExecuteAuthorizedRequest(request);
		}

		protected T Get<T>(string resource, params Parameter[] parameters)
		{
			var request = this.BuildRequest(Method.GET, resource);
			request.Parameters.AddRange(parameters);
			var response = this.ExecuteAuthorizedRequest(request);
			return this.DeserializeResponse<T>(response);
		}

		protected void Post(string resource, object body)
		{
			this.ExecuteAuthorizedRequest(Method.POST, resource, body);
		}

		protected void Put(string resource, object body)
		{
			this.ExecuteAuthorizedRequest(Method.PUT, resource, body);
		}

		protected abstract T DeserializeResponse<T>(IRestResponse response);
	}

	public abstract class BasicRestApi
	{
		protected readonly RestClient RestClient;

		protected BasicRestApi(string apiBaseUrl)
		{
			this.RestClient = new RestClient(apiBaseUrl);
			this.RestClient.ClearHandlers();
			this.RestClient.AddHandler("*", new JsonDeserializer());
		}

		protected abstract void Authorize(RestRequest request);
		protected virtual IRestResponse ExecuteAuthorizedRequest(RestRequest request)
		{
			request.AddHeader("Accept", "application/json");
			this.Authorize(request);
			return this.RestClient.Execute(request);
		}

		protected virtual RestRequest BuildRequest(Method method, string resource)
		{
			var request = new RestRequest(method);
			request.RequestFormat = DataFormat.Json;
			request.Resource = resource;
			return request;
		}
	}

	public interface MeliUserInformation
	{
		long UserId { get; set; }
		string SiteId { get; set; }
	}

	public class SnakeCaseJsonConverter : ISerializer, IDeserializer
	{
		/// <summary>
		/// Default serializer
		/// </summary>
		public SnakeCaseJsonConverter()
		{
			this.ContentType = "application/json; charset=utf-8";
		}

		/// <summary>
		/// Serialize the object as JSON
		/// </summary>
		/// <param name="obj">Object to serialize</param>
		/// <returns>JSON as String</returns>
		public string Serialize(object obj)
		{

			return JsonConvert.SerializeObject(obj, this.Settings);
		}

		public T Deserialize<T>(IRestResponse response)
		{
			return JsonConvert.DeserializeObject<T>(response.Content, this.Settings);

		}

		private JsonSerializerSettings Settings
		{
			get
			{
				return new JsonSerializerSettings
				{
					MissingMemberHandling = MissingMemberHandling.Ignore,
					NullValueHandling = NullValueHandling.Ignore,
					DefaultValueHandling = DefaultValueHandling.Include,
					ContractResolver = new SnakeCaseProperiesContractResolver(),
					Converters = new JsonConverter[] { new StringEnumConverter(), new DoubleConverter() }
				};
			}
		}

		/// <summary>
		/// Unused for JSON Serialization
		/// </summary>
		public string DateFormat { get; set; }

		/// <summary>
		/// Unused for JSON Serialization
		/// </summary>
		public string RootElement { get; set; }

		/// <summary>
		/// Unused for JSON Serialization
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		/// Content type for serialized content
		/// </summary>
		public string ContentType { get; set; }
	}

	public class SnakeCaseProperiesContractResolver : DefaultContractResolver
	{
		protected override string ResolvePropertyName(string propertyName)
		{
			// lower case the first letter of the passed in name
			return propertyName.ToSnakeCase();
		}
	}

	public static class StringHelper
	{
		public static string ToSnakeCase(this string input)
		{
			return string.Concat(input.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
		}

	}

	public class DoubleConverter : JsonConverter
	{
		private const int NUMBER_OF_DECIMALS = 2;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var number = (double)value;
			if (number % 1 == 0)
				writer.WriteValue((int)number);
			else
				writer.WriteValue(Math.Round(number, NUMBER_OF_DECIMALS));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
			JsonSerializer serializer)
		{
			throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
		}

		public override bool CanRead
		{
			get { return false; }
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(double) || objectType == typeof(double?);
		}
	}
}
