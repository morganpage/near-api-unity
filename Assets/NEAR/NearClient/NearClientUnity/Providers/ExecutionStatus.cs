using System;
using Newtonsoft.Json.Linq;

namespace NearClientUnity.Providers
{
  public class ExecutionStatus
  {
    public ExecutionError Failure { get; set; }
    public string SuccessReceiptId { get; set; }
    public string SuccessValue { get; set; }

    public static ExecutionStatus FromDynamicJsonObject(JObject jsonObject)
    {
      if (jsonObject.ToString() == "Unknown")
      {
        return new ExecutionStatus();
      }

      var isFailure = jsonObject["Failure"] != null;

      if (isFailure)
      {
        return new ExecutionStatus()
        {
          Failure = ExecutionError.FromDynamicJsonObject((JObject)jsonObject["Failure"]),
        };
      }

      return new ExecutionStatus()
      {
        SuccessReceiptId = jsonObject["SuccessReceiptId"].ToString(),
        SuccessValue = jsonObject["SuccessValue"].ToString(),
      };
    }

    public static implicit operator ExecutionStatus(JToken v)
    {
      throw new NotImplementedException();
    }
  }
}