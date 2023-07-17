using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UnityEngine;

namespace NearClientUnity.Utilities
{
  public static class Web
  {
    public static async Task<JObject> FetchAsync(ConnectionInfo connection, string json = "")
    {
      var url = connection.Url;
      var result = await FetchAsync(url, json);
      return result;
    }

    public static async Task<JObject> FetchAsync(string url, string json = "")
    {
      using (var client = new HttpClient())
      {
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        HttpResponseMessage response;

        if (!string.IsNullOrEmpty(json))
        {
          HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
          content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
          response = client.PostAsync(url, content).Result;
        }
        else
        {
          response = await client.GetAsync(url);
        }

        if (response.IsSuccessStatusCode)
        {
          string jsonString = await response.Content.ReadAsStringAsync();
          JObject rawResult = JObject.Parse(jsonString);
          if (rawResult["error"] != null && rawResult["error"]["data"] != null)
          {
            Debug.Log("Web.cs: FetchAsync: rawResult ERROR: " + rawResult);
            return null;
            //throw new Exception($"[{rawResult["error"]["code"]}]: {rawResult["error"]["data"]["error_type"]}: {rawResult["error"]["data"]["error_message"]}");
          }
          return (JObject)rawResult["result"];
        }
        else
        {
          throw new HttpException((int)response.StatusCode, response.Content.ToString());
        }
      }
    }

  }
}