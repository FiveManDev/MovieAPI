using AutoMapper;
using MovieAPI.Controllers;
using MovieAPI.Data;
using MovieAPI.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json.Nodes;
namespace MovieAPI.Services.MomoPayment
{
    public class PaymentRequest
    {
        public static async Task<string> SendPaymentRequest(string endpoint, object postJsonObject)
        { 
            try
            {
                using HttpClient client = new HttpClient();
                var json = JsonConvert.SerializeObject(postJsonObject);
                var jsonRequest = JsonValue.Parse(json);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var result = client.PostAsJsonAsync(endpoint, jsonRequest).Result;
                var streamRead = await result.Content.ReadAsStringAsync();
                var jsonResult = JObject.Parse(streamRead);
                return jsonResult.ToString();
            }
            catch (Exception ex)
            {
                Logger<PaymentRequest> a = null;
                a.LogInformation(ex.ToString());
                return null;
            }
            
        }

    }
}
