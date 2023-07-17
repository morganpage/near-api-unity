using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NearClientUnity.Providers
{
  public class ExecutionOutcome
  {
    public int GasBurnt { get; set; }
    public string[] Logs { get; set; }
    public string[] ReceiptIds { get; set; }
    public ExecutionStatus Status { get; set; }
    public ExecutionStatusBasic StatusBasic { get; set; }

    public static ExecutionOutcome FromDynamicJsonObject(JObject jsonObject)
    {
      if (jsonObject == null)
      {
        return null;
      }
      var logs = new List<string>();
      if (jsonObject["logs"] != null)
      {
        foreach (var log in jsonObject["logs"])
        {
          logs.Add((string)log);
        }
      }
      var receiptIds = new List<string>();
      if (jsonObject["receipt_ids"] != null)
      {
        foreach (var receipt in jsonObject["receipt_ids"])
        {
          receiptIds.Add((string)receipt);
        }
      }
      var result = new ExecutionOutcome()
      {
        GasBurnt = jsonObject["gas_burnt"] != null ? (int)jsonObject["gas_burnt"] : 0,
        Logs = logs.ToArray(),
        ReceiptIds = receiptIds.ToArray()
        // Status = jsonObject["status"] ?? null,
      };
      if (jsonObject["status"] != null)
      {
        result.Status = ExecutionStatus.FromDynamicJsonObject((JObject)jsonObject["status"]);
      }
      return result;
    }
  }
}