using NearClientUnity.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using System.Web;
using UnityEngine;

namespace NearClientUnity.Providers
{
  public class JsonRpcProvider : Provider
  {
    private readonly ConnectionInfo _connection;

    public JsonRpcProvider(string url)
    {
      var connectionInfo = new ConnectionInfo
      {
        Url = url
      };
      _connection = connectionInfo;
    }

    private int _id { get; set; } = 123;

    public override async Task<BlockResult> GetBlockAsync(int blockId)
    {
      var parameters = new JArray();
      parameters.Add(blockId);
      var result = await SendJsonRpcNew("block", parameters);
      BlockResult blockResult = JsonConvert.DeserializeObject<BlockResult>(result.ToString());
      return blockResult;
    }

    public override async Task<ChunkResult> GetChunkAsync(string chunkId)
    {
      var parameters = new JArray();
      parameters.Add(chunkId);
      var result = await SendJsonRpcNew("chunk", parameters);
      ChunkResult chunkResult = JsonConvert.DeserializeObject<ChunkResult>(result.ToString());
      return chunkResult;
    }

    public override Task<ChunkResult> GetChunkAsync(int[,] chunkId)
    {
      throw new NotImplementedException();
    }

    public override INetwork GetNetwork()
    {
      INetwork result = null;

      result.Name = "test";
      result.ChainId = "test";

      return result;
    }

    public override async Task<NodeStatusResult> GetStatusAsync()
    {
      var rawStatusResul = await SendJsonRpcNew("status", new JArray());
      var result = NodeStatusResult.FromDynamicJsonObject(rawStatusResul);
      return result;
    }

    public override async Task<FinalExecutionOutcome> GetTxStatusAsync(byte[] txHash, string accountId)
    {
      Debug.Log("JsonRpcProvider.cs: GetTxStatusAsync(byte[] txHash, string accountId):" + Base58.Encode(txHash) + ":" + accountId);
      var parameters = new JArray();
      parameters.Add(Base58.Encode(txHash));
      parameters.Add(accountId);
      var result = await SendJsonRpcNew("tx", parameters);
      FinalExecutionOutcome finalExecutionOutcome = FinalExecutionOutcome.FromDynamicJsonObject(result);
      return finalExecutionOutcome;
    }

    // public override async Task<JObject> QueryAsync(string path, string data)
    // {
    //   Debug.Log("JsonRpcProvider.cs: QueryAsync(string path, string data):" + path + ":" + data);
    //   var parameters = new JArray();
    //   parameters.Add(path);
    //   parameters.Add(data);
    //   try
    //   {
    //     var result = await SendJsonRpcNew("query", parameters);
    //     //var result = await SendJsonRpc("query", parameters);
    //     return result;
    //   }
    //   catch (Exception e)
    //   {
    //     throw new Exception($"Quering {path} failed: {e.Message}.");
    //   }
    // }

    public override async Task<JObject> QueryAsync(string path, string data)
    {
      Debug.Log("JsonRpcProvider.cs: QueryAsync(string path, string data):" + path + ":" + data);
      var parameters = new JArray();
      parameters.Add(path);
      parameters.Add(data);
      try
      {
        var result = await SendJsonRpcNew("query", parameters);
        //var result = await SendJsonRpc("query", parameters);
        return result;
      }
      catch (Exception e)
      {
        throw new Exception($"Quering {path} failed: {e.Message}.");
      }
    }


    public override async Task<FinalExecutionOutcome> SendTransactionAsync(SignedTransaction signedTransaction)
    {
      Debug.Log("JsonRpcProvider.cs: SendTransactionAsync(SignedTransaction signedTransaction):" + signedTransaction);
      var bytes = signedTransaction.ToByteArray();
      var parameters = new JArray();
      parameters.Add(Convert.ToBase64String(bytes, 0, bytes.Length));
      var rawOutcomeResult = await SendJsonRpcNew("broadcast_tx_commit", parameters);
      return new FinalExecutionOutcome();
      // var result = FinalExecutionOutcome.FromDynamicJsonObject(rawOutcomeResult);
      // return result;
    }

    private async Task<JObject> SendJsonRpcNew(string method, JArray parameters)
    {
      Debug.Log("JsonRpcProvider.cs: SendJsonRpcNew:" + method + ":" + parameters);
      JObject requestJson = new JObject();
      requestJson["method"] = method;
      requestJson["parameters"] = parameters;
      requestJson["id"] = _id++;
      requestJson["jsonrpc"] = "2.0";
      var requestString = JsonConvert.SerializeObject(requestJson).Replace("\"parameters\":", "\"params\":");
      try
      {
        var result = await Web.FetchAsync(_connection, requestString);
        return result;
      }
      catch (HttpException e)
      {
        throw new Exception($"{e.ErrorCode}: {e.Message}");
      }
    }



  }
}