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

  public class NearAPIConfig
  {
    private NearAPIConfigNetwork[] _networks;

    public NearAPIConfig()
    {
      _networks = new NearAPIConfigNetwork[]
      {
        new NearAPIConfigNetwork()
        {
          NetworkId = "testnet",
          NodeUrl = "https://rpc.testnet.near.org",
          WalletUrl = "https://testnet.mynearwallet.com/"
        },
        new NearAPIConfigNetwork()
        {
          NetworkId = "mainnet",
          NodeUrl = "https://rpc.mainnet.near.org",
          WalletUrl = "https://app.mynearwallet.com/"
        }
      };
    }


    public NearAPIConfigNetwork GetNetwork(string networkId)
    {
      foreach (NearAPIConfigNetwork network in _networks)
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