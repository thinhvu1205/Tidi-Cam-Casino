using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class InviteView : BaseView
{
    [SerializeField]
    ListViewCtrl listView;
    int agTable;
    static public InviteView instance;

    JArray dataList;
    protected override void Awake()
    {
        base.Awake();
        instance = this;

        Globals.CURRENT_VIEW.setCurView(Globals.CURRENT_VIEW.INVITE_PLAYERVIEW);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        SocketSend.getInviteTableList(agTable);
    }

    public void setAgTable(int _agTable)
    {
        agTable = _agTable;
    }

    public void receiveData(JObject jsonData)
    {
       
        dataList = JArray.Parse((string)jsonData["data"]);
        listView.setDataList(updateItem, dataList);
    }
    void updateItem(GameObject itemView, JObject itData)
    {
        if (itData == null) return;
        itemView.GetComponent<ItemInvite>().setInfo(itData, () =>
        {
            onClickInvite(itData);
        });
    }

    public void onClickInviteAll()
    {
        SoundManager.instance.soundClick();
        for (var i = 0; i < dataList.Count; i++)
        {
            SocketSend.sendInviteTable((int)dataList[i]["Id"] + "", agTable);
        }
        dataList.Clear();
        listView.setDataList(updateItem, dataList);
    }
    public void onClickRefresh()
    {
        SocketSend.getInviteTableList(agTable);
    }
    void onClickInvite(JObject itData)
    {
       
        SoundManager.instance.soundClick();
        SocketSend.sendInviteTable((int)itData["Id"] + "", agTable);
        try
        {
            //dataList.Remove(itData);
            for(var i =0; i < dataList.Count; i++)
            {
                if((int)itData["Id"] == (int)dataList[i]["Id"])
                {
                    dataList.RemoveAt(i);
                    break;
                }
            }
            listView.setDataList(updateItem, dataList);
        }
        catch(System.Exception e)
        {
            Globals.Logging.LogException(e);
        }
    }
}
