using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Globals;

public class WalletInfo : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<Image> listImgWallet;
    [SerializeField] GameObject itemWallet;
    [SerializeField] Image firstImgItem;
    [SerializeField] GameObject itemWalletContainer;
    [SerializeField] TextMeshProUGUI lbWalletId;
    void Start()
    {

    }

    // Update is called once per frame
    public async void setInfo(List<JObject> data)
    {
        for (int i = 0; i < data.Count; i++)
        {
            JObject dataWallet = data[i];
            GameObject itemWal;
            if (i == 0)
            {
                Sprite spr = await Globals.Config.GetRemoteSprite((string)dataWallet["urlImg"]);
                if (spr != null)
                {
                    firstImgItem.sprite = spr;
                }
                lbWalletId.text = (string)dataWallet["walletId"];

            }
            if (i < itemWalletContainer.transform.childCount)
            {
                itemWal = itemWalletContainer.transform.GetChild(i).gameObject;
            }
            else
            {
                itemWal = Instantiate(itemWallet, itemWalletContainer.transform);

            }
            itemWal.GetComponent<WalletItem>().setInfo(dataWallet, i);
            ProfileView.instance.dropBox.listBtnDropbox.Add(itemWal.GetComponent<Button>());
        }
        ProfileView.instance.dropBox.setClickSelect();

    }
}
