using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NEAR;
using System.Threading.Tasks;

public class NFTController : MonoBehaviour
{
  [SerializeField] private string _contractId = "h00kd.near";
  [SerializeField] private string _network = "testnet";

  [SerializeField] private TMP_Text _textWelcome;
  [SerializeField] private TMP_Text _textTokens;
  [SerializeField] Button _buttonSignInOut;
  [SerializeField] UIToken _uiTokenPrefab;
  [SerializeField] Transform _uiTokenParent;

  private string _accountId;
  private bool _signedIn;

  void OnEnable()
  {
    NearCallbacks.OnSignIn += OnSignIn;
    NearCallbacks.OnGetAccountId += OnGetAccountId;
    NearCallbacks.OnNftTokensForOwner += OnNftTokensForOwner;
  }

  void OnDisable()
  {
    NearCallbacks.OnSignIn -= OnSignIn;
    NearCallbacks.OnGetAccountId -= OnGetAccountId;
    NearCallbacks.OnNftTokensForOwner -= OnNftTokensForOwner;
  }

  void Start()
  {
    NearAPI.IsSignedIn(_network);
  }

  private void OnSignIn(bool signedIn)
  {
    _signedIn = signedIn;
    if (_signedIn)
    {
      NearAPI.GetAccountId(_network);
      _buttonSignInOut.GetComponentInChildren<TMP_Text>().text = "Sign Out";
      _textTokens.text = "Click Get Tokens to continue...";
    }
    else
    {
      _textWelcome.text = "You must sign in...";
      _textTokens.text = "You must sign in...";
      _buttonSignInOut.GetComponentInChildren<TMP_Text>().text = "Sign In";
    }
  }
  private void OnGetAccountId(string accountId)
  {
    _accountId = accountId;
    _textWelcome.text = "Welcome " + _accountId;
  }
  private async void OnNftTokensForOwner(Token[] tokens)
  {
    foreach (Transform child in _uiTokenParent)
    {
      Destroy(child.gameObject);
    }

    foreach (var token in tokens)
    {
      _textTokens.text = "Select a token to continue...";

      // Get the image
      Texture texture = await RestAPI.GetImage(token.metadata.media);
      if (texture != null)
      {
        UIToken uiToken = Instantiate(_uiTokenPrefab);
        uiToken.SetImage(texture);
        uiToken.SetTitle(token.metadata.title);
        uiToken.SetTokenId(token.token_id);
        uiToken.SetButtonListener(OnTokenButtonClicked);
        uiToken.transform.SetParent(_uiTokenParent);
        uiToken.transform.localScale = Vector3.one;
        uiToken.transform.localPosition = Vector3.zero;
        _uiTokenParent.localPosition = Vector3.zero;
      }
    }
  }

  void OnTokenButtonClicked(string tokenId)
  {
    Debug.Log("OnTokenButtonClicked: " + tokenId);
    _textTokens.text = "Token Id: " + tokenId + " was selected!";
  }

  public void NftTokensForOwner()
  {
    if (!_signedIn)
    {
      _textTokens.text = "You must sign in to get tokens...";
      return;
    }
    NearAPI.NftTokensForOwner(_accountId, _contractId, _network);
  }

  public void SignInOut()
  {
    if (_signedIn)
    {
      NearAPI.SignOut(_network);
      NearAPI.IsSignedIn(_network);
    }
    else
    {
      NearAPI.RequestSignIn(_contractId, _network);//This will do a page refresh anyway
      NearAPI.IsSignedIn(_network);
    }
  }

}
