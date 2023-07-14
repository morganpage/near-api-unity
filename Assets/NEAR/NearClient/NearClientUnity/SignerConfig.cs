using Newtonsoft.Json.Linq;

namespace NearClientUnity
{
  public class SignerConfig
  {
    //public dynamic Args { get; set; }
    public JObject ArgsJson { get; set; }
    public SignerType Type { get; set; }
  }
}