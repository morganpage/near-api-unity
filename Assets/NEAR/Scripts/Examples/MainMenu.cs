using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
  public void NFTExample()
  {
    SceneManager.LoadScene("NFTScene");
  }

  public void ContractExample()
  {
    SceneManager.LoadScene("ContractScene");
  }

  public void WalletSelectorExample()
  {
    SceneManager.LoadScene("WSScene");
  }

  public void Menu()
  {
    SceneManager.LoadScene("MenuScene");
  }

}
