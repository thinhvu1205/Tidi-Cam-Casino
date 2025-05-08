using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadImage : MonoBehaviour
{
    [SerializeField] Image img;
    // Start is called before the first frame update
    async void Start()
    {
      
    }

    // Update is called once per frame
    public  async Task<Sprite> GetRemoteSprite(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            // begin request:
            var asyncOp = www.SendWebRequest();

            // await until it's done: 
            while (asyncOp.isDone == false)
                await Task.Delay(1000 / 30);//30 hertz

            // read results:
            //if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            if (www.result != UnityWebRequest.Result.Success)// for Unity >= 2020.1
            {
                // log error:
                Debug.Log($"{www.error}, URL:{www.url}");
                // nothing to return on error:
                return null;
            }
            else
            {
                // return valid results:
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
                return sprite;
            }
        }
    }
    public async  void loadImage()
    {
        Debug.Log("Start Load Banner");
        img.sprite = await GetRemoteSprite("https://storage.googleapis.com/cdn.topmanilaclub.com/NewBanner/1004/Code/V01/1004T9.png");
        Debug.Log("Load xong");
        img.SetNativeSize();
    }
}
