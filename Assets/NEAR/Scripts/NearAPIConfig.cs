using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NEAR
{
  [System.Serializable]
  public class NearAPIConfigNetwork
  {
    public string NetworkId;
    public string NodeUrl;
    public string WalletUrl;
  }

  [CreateAssetMenu(menuName = "ScriptableObjects/NearAPIConfig")]
  public class NearAPIConfig : ScriptableObject
  {
    public NearAPIConfigNetwork[] Networks;

    public NearAPIConfigNetwork GetNetwork(string networkId)
    {
      foreach (NearAPIConfigNetwork network in Networks)
      {
        if (network.NetworkId == networkId)
        {
          return network;
        }
      }
      return null;
    }

  }

}