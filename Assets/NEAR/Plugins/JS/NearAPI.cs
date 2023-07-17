using UnityEngine;
using System.Runtime.InteropServices;
using NearClientUnity;
using NearClientUnity.Utilities;
using NearClientUnity.KeyStores;
using NearClientUnity.Providers;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System;

namespace NEAR
{
  public class NearAPI
  {  //NEAR API JS see  https://docs.near.org/tools/near-api-js/quick-reference
    private static string _signedIn = "false";//Just to mimic the actual functionality
                                              //private static string _dummytokens = "[{token_id:'1',metadata:{title:'Fred',media:'https://roguefoxguild.mypinata.cloud/ipfs/QmYMT3s9C4ckQxfMEm7ew4PUVPSrYLeo36uhdhGSctDJdB/Character_1.png'} },{token_id:'2',metadata:{title:'Alice',media:'https://roguefoxguild.mypinata.cloud/ipfs/QmYMT3s9C4ckQxfMEm7ew4PUVPSrYLeo36uhdhGSctDJdB/Character_2.png'}}]";

#if PLATFORM_WEBGL

    public static void StartUp(string contractId, string networkId) { }

    [DllImport("__Internal")]
    public static extern void RequestSignIn(string contractId, string network = "testnet");

    [DllImport("__Internal")]
    public static extern void SignOut(string network = "testnet");

    [DllImport("__Internal")]
    public static extern void IsSignedIn(string network = "testnet");

    [DllImport("__Internal")]
    public static extern void NftTokensForOwner(string accountId, string contractId, string network = "testnet");

    [DllImport("__Internal")]
    public static extern void GetAccountId(string network = "testnet");

    [DllImport("__Internal")]
    public static extern void ContractMethod(string accountId, string contractId, string methodName, string args = "", bool changeMethod = false, string network = "testnet");

#else
    public static NearAPIConfig NearAPIConfig = new NearAPIConfig();
    public static WalletAccount WalletAccount { get; set; }
    public static Near Near { get; set; }
    private static string _contractId;
    private static string _networkId;

    public static async Task StartUp(string contractId, string networkId)
    {
      _contractId = contractId;
      _networkId = networkId;
      Application.deepLinkActivated += async (string url) => await CompleteSignIn(url);
      NearAPIConfigNetwork nearAPIConfigNetwork = NearAPIConfig.GetNetwork(networkId);
      Near = new Near(config: new NearConfig()
      {
        NetworkId = networkId,
        NodeUrl = nearAPIConfigNetwork.NodeUrl,
        ProviderType = ProviderType.JsonRpc,
        SignerType = SignerType.InMemory,
        KeyStore = new InMemoryKeyStore(),
        ContractName = contractId,
        WalletUrl = nearAPIConfigNetwork.WalletUrl
      });
      WalletAccount = new WalletAccount(
      Near,
      "",
      new AuthService(),
      new AuthStorage());
    }

    public static async Task CompleteSignIn(string url)
    {
      Debug.Log("CompleteSignIn: " + url);
      await NearAPI.WalletAccount.CompleteSignIn(url);
      _signedIn = WalletAccount.IsSignedIn() ? "true" : "false";
      NearCallbacks.Instance.IsSignedIn(_signedIn);
    }

    public static async void RequestSignIn(string contractId = "", string network = "testnet")
    {
      await WalletAccount.RequestSignIn(
        _contractId,
        "Near Unity Client",
        new Uri("nearclientunity://nearprotocol.com/success"),
        new Uri("nearclientunity://nearprotocol.com/fail"),
        new Uri("nearclientios://nearprotocol.com")
        );
      _signedIn = WalletAccount.IsSignedIn() ? "true" : "false";
    }

    public static void SignOut(string network = "testnet")
    {
      WalletAccount.SignOut();
      _signedIn = WalletAccount.IsSignedIn() ? "true" : "false";
    }

    public static void IsSignedIn(string network = "testnet")
    {
      _signedIn = WalletAccount.IsSignedIn() ? "true" : "false";
      NearCallbacks.Instance.IsSignedIn(_signedIn);
    }

    public static void GetAccountId(string network = "testnet")
    {
      var walletAccountId = WalletAccount.GetAccountId();
      NearCallbacks.Instance.GetAccountId(walletAccountId);
    }

    public static async void NftTokensForOwner(string accountId, string contractId, string network = "testnet")
    {
      Account account = new Account(Near.Connection, accountId);
      ContractOptions contractOptions = new ContractOptions();
      contractOptions.viewMethods = new string[] { "nft_total_supply", "nft_supply_for_owner", "nft_tokens_for_owner" };
      ContractNear contract = new ContractNear(account, contractId, contractOptions);
      var response = await contract.View("nft_tokens_for_owner", new JObject(new JProperty("account_id", accountId)));
      NearCallbacks.Instance.NftTokensForOwner(response["result"].ToString());
    }

    public static async Task ContractMethod(string accountId, string contractId, string methodName, string args = "", bool changeMethod = false, string network = "testnet")
    {
      Account account = new Account(Near.Connection, accountId);
      ContractOptions contractOptions = new ContractOptions();
      contractOptions.viewMethods = new string[] { methodName };
      ContractNear contract = new ContractNear(account, contractId, contractOptions);
      if (args == "")
      {
        args = "{}";
      }
      JObject argsJson = JObject.Parse(args);

      JObject response;
      if (changeMethod == true)
      {
        response = await contract.Change(methodName, argsJson, 300000000000000, 0);
        if (response != null && response["result"] != null)
        {
          NearCallbacks.Instance.CallMethod(response["result"].ToString());
          NearCallbacks.Instance.ContractMethod(response["result"].ToString());
        }
      }
      else
      {
        response = await contract.View(methodName, argsJson);
        NearCallbacks.Instance.ViewMethod(response["result"].ToString());
        NearCallbacks.Instance.ContractMethod(response["result"].ToString());
      }
    }


#endif

  }
}