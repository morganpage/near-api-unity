using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NEAR;
using System.Threading.Tasks;

public class ContractController : MonoBehaviour
{
  [SerializeField] private TMP_Text _textWelcome;
  [SerializeField] private TMP_Text _textInfo;
  [SerializeField] private TMP_InputField _inputNetwork;
  [SerializeField] private TMP_InputField _inputContractId;
  [SerializeField] private TMP_InputField _inputMethodName;
  [SerializeField] private TMP_InputField _inputArgs;
  [SerializeField] private Toggle _IsChangeMethod;

  [SerializeField] Button _buttonSignInOut;

  private string _accountId;
  private bool _signedIn;

  void OnEnable()
  {
    NearCallbacks.OnSignIn += OnSignIn;
    NearCallbacks.OnGetAccountId += OnGetAccountId;
    NearCallbacks.OnContractMethod += OnContractMethod;
  }

  void OnDisable()
  {
    NearCallbacks.OnSignIn -= OnSignIn;
    NearCallbacks.OnGetAccountId -= OnGetAccountId;
    NearCallbacks.OnContractMethod -= OnContractMethod;
  }

  void Start()
  {
    _inputNetwork.text = "testnet";
    _inputContractId.text = "nft-unity-contract.morganpage1.testnet";
    _inputMethodName.text = "nft_tokens_for_owner";
  }

  private void OnSignIn(bool signedIn)
  {
    _signedIn = signedIn;
    if (_signedIn)
    {
      NearAPI.GetAccountId(_inputNetwork.text);
      _buttonSignInOut.GetComponentInChildren<TMP_Text>().text = "Sign Out";
    }
    else
    {
      _textWelcome.text = "You must sign in...";
      _buttonSignInOut.GetComponentInChildren<TMP_Text>().text = "Sign In";
    }
  }

  private void OnGetAccountId(string accountId)
  {
    _accountId = accountId;
    _textWelcome.text = "Welcome " + _accountId;
    _inputArgs.text = @"{ ""account_id"": """ + _accountId + @""" }";
  }

  public void SignInOut()
  {
    if (_signedIn)
    {
      NearAPI.SignOut(_inputNetwork.text);
      NearAPI.IsSignedIn(_inputNetwork.text);
    }
    else
    {
      NearAPI.RequestSignIn(_inputContractId.text, _inputNetwork.text);//This will do a page refresh anyway
      NearAPI.IsSignedIn(_inputNetwork.text);
    }
  }

  public void CallContractMethod()
  {
    //{ account_id: account.accountId }
    NearAPI.ContractMethod(_accountId, _inputContractId.text, _inputMethodName.text, _inputArgs.text, _IsChangeMethod.isOn, _inputNetwork.text);
  }

  void OnContractMethod(string json)
  {
    _textInfo.text = json;
  }


  public async void HandleDeepLink(string url)
  {
    Debug.Log("HandleDeepLink: " + url);
    await NearAPI.CompleteSignIn(url);
  }


}
