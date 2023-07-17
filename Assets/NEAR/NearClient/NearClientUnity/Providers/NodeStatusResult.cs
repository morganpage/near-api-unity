using Newtonsoft.Json.Linq;

namespace NearClientUnity.Providers
{
  public class NodeStatusResult
  {
    public string ChainId { get; set; }
    public string RpcAddr { get; set; }
    public SyncInfo SyncInfo { get; set; }
    public JArray Validators { get; set; }

    public static NodeStatusResult FromDynamicJsonObject(JObject jsonObject)
    {
      var result = new NodeStatusResult()
      {
        ChainId = jsonObject["chain_id"].ToString(),
        RpcAddr = jsonObject["rpc_addr"].ToString(),
        SyncInfo = new SyncInfo()
        {
          LatestBlockHash = jsonObject["sync_info"]["latest_block_hash"].ToString(),
          LatestBlockHeight = jsonObject["sync_info"]["latest_block_height"].ToObject<int>(),
          LatestBlockTime = jsonObject["sync_info"]["latest_block_time"].ToString(),
          LatestStateRoot = jsonObject["sync_info"]["latest_state_root"].ToString(),
          Syncing = jsonObject["sync_info"]["syncing"].ToObject<bool>()
        },
        Validators = jsonObject["validators"] as JArray
      };
      return result;
    }
  }
}