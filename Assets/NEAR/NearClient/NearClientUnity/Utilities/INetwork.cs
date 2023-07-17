using Newtonsoft.Json.Linq;

namespace NearClientUnity.Utilities
{
  public interface INetwork
  {
    string ChainId { get; set; }
    string Name { get; set; }

    JObject DefaultProvider(JArray providers);
  }
}