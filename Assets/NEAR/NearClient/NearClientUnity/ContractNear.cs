using NearClientUnity.Providers;
using NearClientUnity.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NearClientUnity
{
  public class ContractNear
  {
    private readonly Account _account;
    private readonly string _contractId;
    private readonly string[] _availableChangeMethods;
    private readonly string[] _availableViewMethods;

    public ContractNear(Account account, string contractId, ContractOptions options)
    {
      _account = account;
      _contractId = contractId;
      _availableViewMethods = options.viewMethods as string[];
      _availableChangeMethods = options.changeMethods as string[];
    }

    public async Task<JObject> Change(string methodName, JObject args, ulong? gas = null, Nullable<UInt128> amount = null)
    {
      var rawResult = await _account.FunctionCallAsync(_contractId, methodName, args, gas, amount);
      return Provider.GetTransactionLastResult(rawResult);
    }


    public async Task<JObject> View(string methodName, JObject args)
    {
      var rawResult = await _account.ViewFunctionAsync(_contractId, methodName, args);
      JObject data = new JObject();
      var logs = new List<string>();
      var result = new List<byte>();
      foreach (var log in rawResult["logs"])
      {
        logs.Add((string)log);
      }
      foreach (var item in rawResult["result"])
      {
        result.Add((byte)item);
      }
      data["logs"] = new JArray(logs.ToArray());
      data["result"] = Encoding.UTF8.GetString(result.ToArray()).Trim('"');
      return data;
    }
  }
}