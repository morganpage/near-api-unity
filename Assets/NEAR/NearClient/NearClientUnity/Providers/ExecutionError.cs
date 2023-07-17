using Newtonsoft.Json.Linq;

namespace NearClientUnity.Providers
{
  public class ExecutionError
  {
    public string ErrorMessage { get; set; }
    public string ErrorType { get; set; }

    public static ExecutionError FromDynamicJsonObject(JObject jsonObject)
    {
      var result = new ExecutionError()
      {
        ErrorMessage = jsonObject["error_message"].ToString(),
        ErrorType = jsonObject["error_type"].ToString(),
      };
      return result;
    }
  }
}