using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using RestSharp;

namespace GHC.Data
{
    public static class ServiceHelper
    {
        const string baseUrl = "https://globalhomecare.azurewebsites.net";

        public static async Task<string> Authenticate(string email, string password)
        {
            RestClient client = new RestClient(baseUrl);
            RestRequest request = new RestRequest("/token/create", Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", $"{{ Email: \"{email}\", Password: \"{password}\" }}", ParameterType.RequestBody);
            IRestResponse<string> response = await client.ExecuteTaskAsync<string>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return response.Data;
            else
                return null;
        }

        public static async Task<bool> Register(CustomerViewModel model)
        {
            RestClient client = new RestClient(baseUrl);
            RestRequest request = new RestRequest("/customers/register", Method.POST);
            request.AddHeader("content-type", "application/json");
            request.AddParameter("application/json", model, ParameterType.RequestBody);
            IRestResponse response = await client.ExecuteTaskAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return true;
            else
                return false;
        }

        public static async Task<List<PricingCategory>> GetPricingCategories()
        {
            RestClient client = new RestClient(baseUrl);
            RestRequest request = new RestRequest($"/pricingcategories/get", Method.GET);
            request.AddHeader("content-type", "application/json");
            IRestResponse<List<PricingCategory>> response = await client.ExecuteTaskAsync<List<PricingCategory>>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return response.Data;
            else
                return null;
        }

        public static async Task<List<HealthService>> GetServices(string token)
        {
            RestClient client = new RestClient(baseUrl);
            RestRequest request = new RestRequest($"/services/get", Method.GET);
            request.AddHeader("Authorization", $"bearer {token}");
            request.AddHeader("content-type", "application/json");
            IRestResponse<List<HealthService>> response = await client.ExecuteTaskAsync<List<HealthService>>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return response.Data;
            else
                return null;
        }
    }
}