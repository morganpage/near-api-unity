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



  public async void Login()
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
    await WalletAccount.CompleteSignIn("nearclientunity://nearprotocol.com/success?account_id=morganpage.testnet&public_key=ed25519%3ACjkngWokhqGRaApoMuaqiVWDcYdiV5NUK5BiFwq4ehNG&all_keys=ed25519%3A4ZjvEfgbBU78Pw2di1RxTC5LsYUD5SxHp6Co3JJCgUGk");
    //SceneManager.LoadScene("Near", LoadSceneMode.Single);

  }

  public void OpenSplash()
  {
    Debug.Log("OpenSplash");
    //Application.OpenURL("nearclientunity://nearprotocol.com/success");
    SceneManager.LoadScene("Splash", LoadSceneMode.Single);
  }
}
