using NearClientUnity.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Dynamic;
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
      Debug.Log("JsonRpcProvider.cs: JsonRpcProvider(string url)");
      var connectionInfo = new ConnectionInfo
      {
        Url = url
      };
      Debug.Log("JsonRpcProvider.cs: JsonRpcProvider(string url)2");
      _connection = connectionInfo;
      Debug.Log("JsonRpcProvider.cs: JsonRpcProvider(string url)3");

    }

    private int _id { get; set; } = 123;

    public override async Task<BlockResult> GetBlockAsync(int blockId)
    {
      // var parameters = new dynamic[1];
      // parameters[0] = blockId;
      var parameters = new JArray();
      parameters.Add(blockId);
      var result = await SendJsonRpcNew("block", parameters);
      BlockResult blockResult = JsonConvert.DeserializeObject<BlockResult>(result.ToString());
      return blockResult;
    }

    public override async Task<ChunkResult> GetChunkAsync(string chunkId)
    {
      // var parameters = new dynamic[1];
      // parameters[0] = chunkId;
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
      // var rawStatusResul = await SendJsonRpc("status", new dynamic[0]);
      var rawStatusResul = await SendJsonRpcNew("status", new JArray());
      var result = NodeStatusResult.FromDynamicJsonObject(rawStatusResul);
      return result;
    }

    public override async Task<FinalExecutionOutcome> GetTxStatusAsync(byte[] txHash, string accountId)
    {
      // var parameters = new dynamic[2];
      // parameters[0] = Base58.Encode(txHash);
      // parameters[1] = accountId;
      // var result = await SendJsonRpc("tx", parameters);
      var parameters = new JArray();
      parameters.Add(Base58.Encode(txHash));
      parameters.Add(accountId);
      var result = await SendJsonRpcNew("tx", parameters);
      FinalExecutionOutcome finalExecutionOutcome = FinalExecutionOutcome.FromDynamicJsonObject(result);
      return finalExecutionOutcome;
    }

    public override async Task<dynamic> QueryAsync(string path, string data)
    {
      Debug.Log("JsonRpcProvider.cs: QueryAsync(string path, string data):" + path + ":" + data);
      // var parameters = new dynamic[2];
      // parameters[0] = path;
      // parameters[1] = data;
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

    public override async Task<JObject> QueryAsyncJO(string path, string data)
    {
      Debug.Log("JsonRpcProvider.cs: QueryAsync(string path, string data):" + path + ":" + data);
      // var parameters = new dynamic[2];
      // parameters[0] = path;
      // parameters[1] = data;
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
      var bytes = signedTransaction.ToByteArray();
      // var parameters = new dynamic[1];
      // parameters[0] = Convert.ToBase64String(bytes, 0, bytes.Length);
      var parameters = new JArray();
      parameters.Add(Convert.ToBase64String(bytes, 0, bytes.Length));
      var rawOutcomeResult = await SendJsonRpcNew("broadcast_tx_commit", parameters);
      var result = FinalExecutionOutcome.FromDynamicJsonObject(rawOutcomeResult);
      return result;
    }

    private async Task<JObject> SendJsonRpcNew(string method, JArray parameters)
    {
      JObject requestJson = new JObject();
      requestJson["method"] = method;
      requestJson["parameters"] = parameters;
      requestJson["id"] = _id++;
      requestJson["jsonrpc"] = "2.0";
      var requestString = JsonConvert.SerializeObject(requestJson).Replace("\"parameters\":", "\"params\":");
      Debug.Log("JsonRpcProvider.cs: SendJsonRpc:" + requestString);
      try
      {
        var result = await Web.FetchAsync(_connection, requestString);
        Debug.Log("JsonRpcProvider.cs: SendJsonRpc: result:" + result);
        return result;
      }
      catch (HttpException e)
      {
        throw new Exception($"{e.ErrorCode}: {e.Message}");
      }
    }



    // private async Task<dynamic> SendJsonRpc(string method, dynamic[] parameters)
    // {
    //   dynamic request = new ExpandoObject();
    //   request.method = method;
    //   request.parameters = parameters;
    //   request.id = _id++;
    //   request.jsonrpc = "2.0";
    //   var requestString = JsonConvert.SerializeObject(request).Replace("\"parameters\":", "\"params\":");
    //   Debug.Log("JsonRpcProvider.cs: SendJsonRpc:" + requestString);
    //   try
    //   {
    //     var result = await Web.FetchJsonAsync(_connection, requestString);
    //     return result;
    //   }
    //   catch (HttpException e)
    //   {
    //     throw new Exception($"{e.ErrorCode}: {e.Message}");
    //   }
    // }
  }
}