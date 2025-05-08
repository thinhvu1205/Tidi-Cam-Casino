using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

public class WalletItem : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Image imgIcon;
    public string idWallet = "";
    void Start()
    {

    }

    // Update is called once per frame
    public async void setInfo(JObject data, int index)
    {
        Sprite spr = await Globals.Config.GetRemoteSprite((string)data["urlImg"]);
        if (spr != null)
        {
            imgIcon.sprite = spr;
        }
        idWallet = (string)data["walletId"];
        ProfileView.instance.dropBox.SetSlectWithIndex(index);

    }
}
