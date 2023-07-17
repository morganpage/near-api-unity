using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NEAR;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using NearClientUnity;

public class WSController : MonoBehaviour
{
  [SerializeField] private TMP_Text _textMessages;
  [SerializeField] private TMP_InputField _inputMessage;
  [SerializeField] Button _buttonSignInOut;
  [SerializeField] private TMP_Text _textWelcome;
  [SerializeField] Button _buttonAdd;

  private bool _signedIn;
  private string _accountId;

  private const string CONTRACT_ID = "guest-book.testnet";
  private const string NETWORK = "testnet";

  void OnEnable()
  {
    NearCallbacks.OnSignIn += OnSignIn;
    NearCallbacks.OnGetAccountId += OnGetAccountId;
    NearCallbacks.OnViewMethod += OnViewMethod;
    NearCallbacks.OnCallMethod += OnCallMethod;
  }

  void OnDisable()
  {
    NearCallbacks.OnSignIn -= OnSignIn;
    NearCallbacks.OnGetAccountId -= OnGetAccountId;
    NearCallbacks.OnViewMethod -= OnViewMethod;
    NearCallbacks.OnCallMethod -= OnCallMethod;
  }


  void Start()
  {
    NearWSAPI.WS_StartUp(CONTRACT_ID, NETWORK);
    OnSignIn(NearWSAPI.WalletAccount.IsSignedIn());
  }

  public void SignInOut()
  {
    if (_signedIn)
    {
      NearWSAPI.WS_SignOut();
      NearWSAPI.WS_IsSignedIn();
    }
    else
    {
      NearWSAPI.WS_SignIn();
      NearWSAPI.WS_IsSignedIn();
    }
  }

  private void OnSignIn(bool signedIn)
  {
    _signedIn = signedIn;
    if (_signedIn)
    {
      NearWSAPI.WS_GetAccountId();
      GetMessages();
      _buttonSignInOut.GetComponentInChildren<TMP_Text>().text = "Sign Out";
    }
    else
    {
      _textMessages.text = "";
      _textWelcome.text = "You must sign in...";
      _buttonSignInOut.GetComponentInChildren<TMP_Text>().text = "Sign In";
    }
  }

  private void OnGetAccountId(string accountId)
  {
    _accountId = accountId;
    _textWelcome.text = "Welcome " + _accountId;
  }

  void GetMessages()
  {
    NearWSAPI.WS_ViewMethod(CONTRACT_ID, "getMessages");
  }

  public async void AddMessage()
  {
    _buttonAdd.interactable = false;
    //args: { text: message }
    string args = @"{""text"":""" + _inputMessage.text + @"""}";
    var definition = new { text = _inputMessage.text };
    string json = JsonConvert.SerializeObject(definition);
    await NearWSAPI.WS_CallMethod(CONTRACT_ID, "addMessage", json);
    OnCallMethod(null);
  }

  void OnViewMethod(string result)
  {
    JArray data = JArray.Parse(result);
    IList<JToken> results = data.Children().ToList();
    string messages = "";
    foreach (JToken token in results)
    {
      string sender = token["sender"].ToString();
      string text = token["text"].ToString();
      if (sender.Length > 20) sender = sender.Substring(0, 20) + "...";
      if (text.Length > 20) text = text.Substring(0, 20) + "...";
      messages += sender + ": " + text + System.Environment.NewLine;
    }
    _textMessages.text = messages;
  }

  void OnCallMethod(string result)
  {
    _buttonAdd.interactable = true;
    _inputMessage.text = "";
    GetMessages();
  }

  public async void HandleDeepLink(string url)
  {
    Debug.Log("HandleDeepLink: " + url);
    await NearAPI.CompleteSignIn(url);
  }


}
