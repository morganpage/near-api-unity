using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NearClientUnity;
using NearClientUnity.Utilities;
using NearClientUnity.KeyStores;
using NearClientUnity.Providers;
using UnityEngine.SceneManagement;

public class BasicScript : MonoBehaviour
{
  public static NearPersistentManager Instance { get; private set; }
  public WalletAccount WalletAccount { get; set; }
  public Near Near { get; set; }



  public void Login()
  {
    Debug.Log("Login");
    NearConfig config = new NearConfig();
    Debug.Log("Login-a");
    Near = new Near(config: new NearConfig()
    {
      NetworkId = "default",
      NodeUrl = "https://rpc.testnet.near.org",
      ProviderType = ProviderType.JsonRpc,
      SignerType = SignerType.InMemory,
      KeyStore = new InMemoryKeyStore(),
      ContractName = "nft-unity-contract.morganpage1.testnet",
      WalletUrl = "https://testnet.mynearwallet.com/"
    });
    Debug.Log("Login1");
    WalletAccount = new WalletAccount(
    Near,
    "",
    new AuthService(),
    new AuthStorage());
    Debug.Log("Login2");

  }

  public void OpenSplash()
  {
    Debug.Log("OpenSplash");
    //Application.OpenURL("nearclientunity://nearprotocol.com/success");
    SceneManager.LoadScene("Splash", LoadSceneMode.Single);
  }
}
