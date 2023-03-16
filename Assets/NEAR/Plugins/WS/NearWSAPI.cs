using UnityEngine;
using System.Runtime.InteropServices;

namespace NEAR
{

  public class NearWSAPI
  { //Wallet Selector API see - https://docs.near.org/tools/wallet-selector

#if !UNITY_EDITOR

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
    private static string _signedIn = "false";//Just to mimic the actual functionality
    //These are just for testing in the editor
    public static void WS_StartUp(string contractId, string network = "testnet")
    {
      Debug.Log("SignIn: " + contractId + " " + network);
      _signedIn = "true";
      NearCallbacks.Instance.IsSignedIn(_signedIn);
    }

    public static void WS_SignIn()
    {
      Debug.Log("SignIn: ");
      _signedIn = "true";
    }

    public static void WS_SignOut()
    {
      Debug.Log("SignOut: ");
      _signedIn = "false";
    }

    public static void WS_IsSignedIn()
    {
      NearCallbacks.Instance.IsSignedIn(_signedIn);
    }

    public static void WS_GetAccountId()
    {
      Debug.Log("GetAccountId: ");
      NearCallbacks.Instance.GetAccountId("testing.near");
    }

    public static void WS_ViewMethod(string contractId, string methodName, string args = "")
    {
      Debug.Log("ViewMethod: " + contractId + " " + methodName);
      string jsonExample = "[]";
      NearCallbacks.Instance.ViewMethod(jsonExample);
    }

    public static void WS_CallMethod(string contractId, string methodName, string args = "")
    {
      Debug.Log("CallMethod: " + contractId + " " + methodName);
      NearCallbacks.Instance.CallMethod("Json from CallMethod");
    }

#endif

  }
}