using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Services.Protocols;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using RestSharp.Extensions;
using RestSharp.Serializers;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Fulfillment.Soap.FulfillmentSdk.Apis
{
	public abstract class FulfillmentApi : RestApi
	{
		private readonly IDeserializer deserializer;

		protected FulfillmentApi(string username, string password) : base(ConfigurationManager.AppSettings["FulfillmentUrl"])
		{
			this.deserializer = new SnakeCaseJsonConverter();
			this.RestClient.Authenticator = new HttpBasicAuthenticator(username, password);
		}
		
		protected override T DeserializeResponse<T>(IRestResponse response)
		{
			return this.deserializer.Deserialize<T>(response);
		}

		protected override void Validate(IRestResponse response)
		{
			switch (response.StatusCode)
			{
				case HttpStatusCode.Unauthorized:
					throw new UnauthorizedSoapHeaderException();
				case HttpStatusCode.NotFound:
					throw new SoapException("NotFound", new XmlQualifiedName("NotFound"));
			}
		}

		protected override void Authorize(RestRequest request) {}

		protected override RestRequest BuildRequest(Method method, string resource)
		{
			var request = base.BuildRequest(method, resource);
			request.JsonSerializer = new SnakeCaseJsonConverter();
			return request;
		}
	}

	public abstract class RestApi : BasicRestApi
	{
		protected RestApi(string apiBaseUrl) : base(apiBaseUrl) {}
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
			this.Validate(response);
			return this.DeserializeResponse<T>(response);
		}

		protected abstract void Validate(IRestResponse response);

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
					Converters = new JsonConverter[] { new SnakeCaseEnumConverter(), new DoubleConverter() }
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

	public class SnakeCaseEnumConverter : StringEnumConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{

			var val = value.GetString().ToSnakeCase();
			serializer.Serialize(writer, val);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
				return null;

			var value = serializer
				.Deserialize<string>(reader)
				.ToPascalCase(CultureInfo.CurrentCulture);

			return EnumUtils.ParseEnum(objectType, value);
		}
	}

	public static class EnumUtils
	{
		/// <summary>
		/// Get all the values of the given <typeparam name="TEnum"></typeparam>
		/// </summary>
		public static IEnumerable<TEnum> GetValues<TEnum>()
		{
			return (TEnum[])Enum.GetValues(typeof(TEnum));
		}

		public static IEnumerable<NamedObject<TEnum>> ToNamedObjects<TEnum>()
		{
			return GetValues<TEnum>()
				.Select(it => new NamedObject<TEnum>(it));
		}

		public static IEnumerable<NamedObject<TEnum>> ToOrderedNamedObjects<TEnum>()
		{
			return GetValues<TEnum>()
				.Select((it, position) => new OrderedNamedObject<TEnum>(it, position));
		}

		/// <summary>
		/// Get value of the given <typeparam name="TEnum"></typeparam> from a string
		/// </summary>
		public static TEnum ParseEnum<TEnum>(this string value)
		{
			return (TEnum)ParseEnum(typeof(TEnum), value);
		}

		/// <summary>
		/// Get value of the given <typeparam name="TEnum"></typeparam> from a string
		/// </summary>
		public static object ParseEnum(Type enumType, string value)
		{
			if (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>))
				return ParseNullableEnum(enumType, value);
			return Enum.Parse(enumType, value, true);
		}

		/// <summary>
		/// Get value of the given Generic Argument <typeparam name="Nullable<TEnum>"></typeparam> from a string
		/// </summary>
		private static object ParseNullableEnum(Type nullableEnumType, string value)
		{
			var genericType = nullableEnumType.GetGenericArguments().First();
			return ParseEnum(genericType, value);
		}

		public static string GetString(this object value)
		{
			return Enum.GetName(value.GetType(), value);
		}

		public static bool IsNullableEnum(this Type t)
		{
			var u = Nullable.GetUnderlyingType(t);
			return (u != null) && u.IsEnum;
		}
	}

	public class NamedObject<T>
	{
		public NamedObject(T element)
		{
			this.Name = element;
		}

		public T Name { get; set; }
	}

	public class OrderedNamedObject<T> : NamedObject<T>
	{
		public OrderedNamedObject(T element, int order) : base(element)
		{
			this.Order = order;
		}

		public int Order { get; set; }
	}
}
