using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace NEAR
{

  public class RestAPI
  {
    public const string IPFSURL = "https://cloudflare-ipfs.com/ipfs/";

    async static public Task<Texture> GetImage(string imageIPFS)
    {
      Debug.Log("GetImage: " + imageIPFS);
      imageIPFS = imageIPFS.Replace("///", IPFSURL);
      using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageIPFS))
      {
        try
        {
          await webRequest.SendWebRequest();
          Texture texture = DownloadHandlerTexture.GetContent(webRequest);
          return texture;
        }
        catch (System.Exception)
        {
          Debug.Log("Error: " + imageIPFS);
          return null;
        }
      }
    }
  }

}