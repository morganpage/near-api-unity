using NearClientUnity.KeyStores;
using NearClientUnity.Providers;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace NearClientUnity
{
  public class Connection
  {
    private readonly string _networkId;
    private readonly Provider _provider;
    private readonly Signer _signer;

    public Connection(string networkId, Provider provider, Signer signer)
    {
      _networkId = networkId;
      _provider = provider;
      _signer = signer;
    }

    public string NetworkId => _networkId;
    public Provider Provider => _provider;
    public Signer Signer => _signer;

    public static Connection FromConfig(ConnectionConfig config)
    {
      Debug.Log("Connection.cs: FromConfig(ConnectionConfig config)");
      var provider = GetProvider(config.Provider);
      Debug.Log("Connection.cs: FromConfig(ConnectionConfig config)1");
      var signer = GetSigner(config.Signer);
      Debug.Log("Connection.cs: FromConfig(ConnectionConfig config)2");

      return new Connection(config.NetworkId, provider, signer);
    }

    private static Provider GetProvider(ProviderConfig config)
    {
      switch (config.Type)
      {
        case ProviderType.JsonRpc:
          {
            Debug.Log("Connection.cs: GetProvider(ProviderConfig config)");
            //string url = config.Args.Url as string;
            string url = "https://rpc.testnet.near.org";
            Debug.Log("Connection.cs: GetProvider(ProviderConfig config)1");
            return new JsonRpcProvider(url);
          }
        default:
          throw new Exception($"Unknown provider type {config.Type}");
      }
    }

    private static Signer GetSigner(SignerConfig config)
    {
      switch (config.Type)
      {
        case SignerType.InMemory:
          {
            InMemoryKeyStore keyStore = JsonConvert.DeserializeObject<InMemoryKeyStore>(config.ArgsJson["KeyStore"].ToString());
            return new InMemorySigner(keyStore);
            //return new InMemorySigner(config.Args.KeyStore as KeyStore);
          }
        default:
          throw new Exception($"Unknown signer type {config.Type}");
      }
    }
  }
}