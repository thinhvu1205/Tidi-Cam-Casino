using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;

public class ItemTopGame : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    TextMeshProUGUI lbStt, lbName, lbChip;

    [SerializeField]
    Image iconStt;

    [SerializeField]
    List<Sprite> sprStt = new List<Sprite>();

    [SerializeField]
    List<Sprite> sprBg = new List<Sprite>();

    [SerializeField]
    VipContainer VipContainer;

    [SerializeField]
    Avatar avatar;

    private JObject dataUser = new JObject();



    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void setInfo(JObject data, bool isMe = false)
    {
        dataUser = data;
        //transform.localPosition = new Vector2(0, transform.localPosition.y);
        int rank = (int)data["R"];
        if (rank < 4)
        {
            iconStt.gameObject.SetActive(true);
            iconStt.sprite = sprStt[rank - 1];
            iconStt.SetNativeSize();
            lbStt.gameObject.SetActive(false);
        }
        else
        {
            lbStt.gameObject.SetActive(true);
            lbStt.text = rank.ToString();
            iconStt.gameObject.SetActive(false);
        }
        if (!isMe)
        {
            // if(rank <= 3)
            // {
            //     GetComponent<Image>().sprite = sprBg[rank];
            // }
            // else
            // {
            //     GetComponent<Image>().sprite = sprBg[0];
            // }
            //if (rank == 1)
            //{
            //    GetComponent<Image>().sprite = sprBg[0];
            //}
            //else if (rank < 4)
            //{
            //    GetComponent<Image>().sprite = sprBg[1];
            //}
            //else
            //{
            //    GetComponent<Image>().sprite = sprBg[2];
            //}
            // GetComponent<Image>().SetNativeSize();
        }
        //GetComponent<RectTransform>().sizeDelta = new Vector2(2000, GetComponent<RectTransform>().sizeDelta.y);
        string name = (string)data["N"];
        lbName.text = name;
        lbChip.text = Globals.Config.FormatNumber((long)data["M"]);
        VipContainer.setVip((int)data["V"]);
        int avatarId = (int)data["Av"];
        string fbId = (string)data["Faid"];
        avatar.loadAvatar(avatarId, name, fbId);
        avatar.setVip((int)data["V"]);

    }
    public void onClickShowProfile()
    {
        return;
        SocketSend.searchFriend((string)dataUser["Id"]);
    }

}
