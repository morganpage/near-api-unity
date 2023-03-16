using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIToken : MonoBehaviour
{
  [SerializeField] private TMP_Text _textTokenId;
  [SerializeField] private Image _image;
  [SerializeField] private TMP_Text _textTitle;
  [SerializeField] private Button _button;

  public void SetImage(Texture texture)
  {
    _image.sprite = Sprite.Create((Texture2D)texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
  }

  public void SetTitle(string title)
  {
    _textTitle.text = title;
  }

  public void SetTokenId(string tokenId)
  {
    _textTokenId.text = tokenId;
  }

  public void SetButtonListener(System.Action<string> listener)
  {
    _button.onClick.AddListener(() => listener(_textTokenId.text));
  }

}
