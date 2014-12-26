using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Console;
using System;
using System.Linq;
using Newtonsoft.Json;

namespace BeboApiCodeExample
{
	public class Program
	{
		private const string BeboApiBase = "https://api.chattyheads.com{0}";
		private const string BeboApiKey = "wt2Iq5HqbzETpHRM4sn9J_iAwGNO1BfWPF64mHjlkFsfQBpe9_88r4hwxd9G6Xb0X6MP6p42AS8HfLYp--k6jkNK_SAqFPED";
		private const string BeboApiToken = "";

		private const string SendMessageEndpoint = "/chat/{0}/message";

		public void Main(string[] args)
		{
			// Send Message to User
			var response = SubmitFormPostRequestAsync<SendMessageResponse>(string.Format(SendMessageEndpoint, "577634b29da542e0ab34f3446cbb0e9d"),
				new Dictionary<string, string>
				{
					{ "message", "%23ballin" },
					{ "message_id", Guid.NewGuid().ToString().ToUpper() }
				}).Result;

			if (response.Code != null)
				WriteLine("Error submitting message. Reason: {0}", response.Reason);
			else
				WriteLine("Message saying `{0}` successfully sent to `{1}`", response.Results.First().Message, response.Results.First().Name);

			ReadLine();
		}

		public async Task<Response<T>> SubmitFormPostRequestAsync<T>(string endpoint, Dictionary<string, string> formParams)
			where T : Result
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.TryAddWithoutValidation("X-Api-Key", BeboApiKey);
			client.DefaultRequestHeaders.TryAddWithoutValidation("X-Api-Token", BeboApiToken);

			var response = await client.PostAsync(string.Format(BeboApiBase, endpoint), new FormUrlEncodedContent(formParams));
			var stringResponse = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<Response<T>>(stringResponse);
		}

		public class Response<T>
			where T : Result
		{
			[JsonProperty("count")]
			public int Count { get; set; }

			[JsonProperty("event_id")]
			public string EventId { get; set; }

			[JsonProperty("results")]
			public T[] Results { get; set; }

			[JsonProperty("sync")]
			public long Sync { get; set; }

			#region Error Fields

			[JsonProperty("code")]
			public string Code { get; set; }

			[JsonProperty("reason")]
			public string Reason { get; set; }

			#endregion
		}

		public abstract class Result { }

		public class SendMessageResponse
			: Result
		{
			[JsonProperty("chat_id")]
			public string ChatId { get; set; }

			[JsonProperty("created_at")]
			public long CreatedAt { get; set; }

			[JsonProperty("html")]
			public string Html { get; set; }

			[JsonProperty("message")]
			public string Message { get; set; }

			[JsonProperty("messge_id")]
			public Guid MessageId { get; set; }

			[JsonProperty("name")]
			public string Name { get; set; }

			[JsonProperty("updated_at")]
			public long UpdatedAt { get; set; }

			[JsonProperty("user_id")]
			public string UserId { get; set; }
		}
	}
}
