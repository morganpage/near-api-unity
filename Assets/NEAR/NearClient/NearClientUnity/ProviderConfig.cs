using Newtonsoft.Json.Linq;

namespace NearClientUnity
{
  public class ProviderConfig
  {
    //public dynamic Args { get; set; }
    public JObject ArgsJson { get; set; }
    public ProviderType Type { get; set; }
  }
}