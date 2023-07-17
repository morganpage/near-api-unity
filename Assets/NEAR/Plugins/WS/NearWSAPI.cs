using UnityEngine;
using System.Runtime.InteropServices;
using NearClientUnity;
using System.Threading.Tasks;

namespace NEAR
{

  public class NearWSAPI
  { //Wallet Selector API see - https://docs.near.org/tools/wallet-selector

#if PLATFORM_WEBGL

    [DllImport("__Internal")]
    public static extern void WS_StartUp(string contractId, string network = "testnet");

    [DllImport("__Internal")]
    public static extern void WS_SignIn();

    [DllImport("__Internal")]
    public static extern void WS_SignOut();

    [DllImport("__Internal")]
    public static extern void WS_IsSignedIn();

    [DllImport("__Internal")]
    public static extern void WS_GetAccountId();

    [DllImport("__Internal")]
    public static extern void WS_ViewMethod(string contractId, string methodName, string args ="{}");

    [DllImport("__Internal")]
    public static extern void WS_CallMethod(string contractId, string methodName, string args = "{}");

#else
    public static WalletAccount WalletAccount { get { return NearAPI.WalletAccount; } }
    private static string _signedIn = "false";//Just to mimic the actual functionality
    public static async void WS_StartUp(string contractId, string network = "testnet")
    {
      await NearAPI.StartUp(contractId, network);
    }

    public static void WS_SignIn()
    {
      NearAPI.RequestSignIn();
    }

    public static void WS_SignOut()
    {
      NearAPI.SignOut();
    }

    public static void WS_IsSignedIn()
    {
      NearAPI.IsSignedIn();
    }

    public static void WS_GetAccountId()
    {
      NearAPI.GetAccountId();
    }

    public static async void WS_ViewMethod(string contractId, string methodName, string args = "")
    {
      string accountId = WalletAccount.GetAccountId();
      await NearAPI.ContractMethod(accountId, contractId, methodName, args, false);
    }

    public static async Task WS_CallMethod(string contractId, string methodName, string args = "")
    {
      string accountId = WalletAccount.GetAccountId();
      await NearAPI.ContractMethod(accountId, contractId, methodName, args, true);
    }

#endif

  }
}