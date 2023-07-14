﻿using NearClientUnity.KeyStores;
using NearClientUnity.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Dynamic;
using System.Threading.Tasks;
using UnityEngine;

namespace NearClientUnity
{
  public class Near
  {
    private readonly AccountCreator _accountCreator;
    private readonly NearConfig _config;
    private readonly Connection _connection;

    public Near(NearConfig config)
    {
      _config = config;
      Debug.Log("Near.cs: Near(NearConfig config)");
      //dynamic providerArgs = new ExpandoObject();
      JObject providerArgsJson = new JObject();
      Debug.Log("Near.cs: Near(NearConfig config)1");
      //providerArgs.Url = config.NodeUrl;
      providerArgsJson["Url"] = config.NodeUrl;
      //dynamic signerArgs = new ExpandoObject();
      JObject signerArgsJson = new JObject();
      //signerArgs.KeyStore = config.KeyStore;
      signerArgsJson["KeyStore"] = JsonConvert.SerializeObject(config.KeyStore);
      Debug.Log("Near.cs: Near(NearConfig config)2:" + signerArgsJson["KeyStore"]);
      var connectionConfig = new ConnectionConfig()
      {
        NetworkId = config.NetworkId,
        Provider = new ProviderConfig()
        {
          Type = config.ProviderType,
          //Args = providerArgs,
          ArgsJson = providerArgsJson
        },
        Signer = new SignerConfig()
        {
          Type = config.SignerType,
          //Args = signerArgs,
          ArgsJson = signerArgsJson
        }
      };
      Debug.Log("Near.cs: Near(NearConfig config)3");
      _connection = Connection.FromConfig(connectionConfig);
      Debug.Log("Near.cs: Near(NearConfig config)4");

      if (config.MasterAccount != null)
      {
        Debug.Log("Near.cs: Near(NearConfig config)5");

        // TODO: figure out better way of specifiying initial balance.
        var initialBalance = config.InitialBalance > 0
            ? config.InitialBalance
            : new UInt128(1000 * 1000) * new UInt128(1000 * 1000);
        _accountCreator =
            new LocalAccountCreator(new Account(_connection, config.MasterAccount), initialBalance);
      }
      else if (config.HelperUrl != null)
      {
        Debug.Log("Near.cs: Near(NearConfig config)6");

        _accountCreator = new UrlAccountCreator(_connection, config.HelperUrl);
      }
      else
      {
        _accountCreator = null;
      }
      Debug.Log("Near.cs: Near(NearConfig config)7");

    }

    public AccountCreator AccountCreator => _accountCreator;
    public NearConfig Config => _config;
    public Connection Connection => _connection;

    public static async Task<Near> ConnectAsync(dynamic config)
    {
      // Try to find extra key in `KeyPath` if provided.
      if (config.KeyPath == null) return new Near(config);
      try
      {
        var accountKeyFile = await UnencryptedFileSystemKeyStore.ReadKeyFile(config.keyPath);
        if (accountKeyFile[0] != null)
        {
          // TODO: Only load key if network ID matches
          var keyPair = accountKeyFile[1];
          var keyPathStore = new InMemoryKeyStore();
          await keyPathStore.SetKeyAsync(config.NetworkId, accountKeyFile[0], keyPair);
          if (config.MasterAccount == null)
          {
            config.MasterAccount = accountKeyFile[0];
          }

          config.KeyStore = new MergeKeyStore(new KeyStore[] { config.KeyStore, keyPathStore });
        }
      }
      catch (Exception error)
      {
        Console.WriteLine($"Failed to load master account key from {config.KeyPath}: {error}");
      }

      return new Near(config);
    }

    public async Task<Account> AccountAsync(string accountId)
    {
      var account = new Account(_connection, accountId);
      await account.FetchStateAsync();
      return account;
    }

    public async Task<Account> CreateAccountAsync(string accountId, PublicKey publicKey)
    {
      if (_accountCreator == null)
      {
        throw new Exception(
            "Must specify account creator, either via masterAccount or helperUrl configuration settings.");
      }

      await _accountCreator.CreateAccountAsync(accountId, publicKey);
      return new Account(_connection, accountId);
    }
  }
}