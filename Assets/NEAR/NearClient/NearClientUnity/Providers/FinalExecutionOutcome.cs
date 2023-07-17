using System.Collections.Generic;
using UnityEngine;

namespace NearClientUnity.Providers
{
  public class FinalExecutionOutcome
  {
    public ExecutionOutcomeWithId[] Receipts { get; set; }
    public FinalExecutionStatus Status { get; set; }
    public FinalExecutionStatusBasic StatusBasic { get; set; }
    public ExecutionOutcomeWithId Transaction { get; set; }

    public static FinalExecutionOutcome FromDynamicJsonObject(dynamic jsonObject)
    {
      var receipts = new List<ExecutionOutcomeWithId>();
      if (jsonObject.receipts != null)
      {
        foreach (var receipt in jsonObject.receipts)
        {
          receipts.Add(ExecutionOutcomeWithId.FromDynamicJsonObject(receipt));
        }
      }
      var result = new FinalExecutionOutcome()
      {
        Receipts = receipts.ToArray(),
        Status = FinalExecutionStatus.FromDynamicJsonObject(jsonObject.status),
        Transaction = ExecutionOutcomeWithId.FromDynamicJsonObject(jsonObject.transaction)
      };
      return result;
    }
  }
}