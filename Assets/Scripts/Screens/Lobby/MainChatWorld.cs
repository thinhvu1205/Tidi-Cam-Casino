using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class MainChatWorld : MonoBehaviour
{
    // Start is called before the first frame update
    public static MainChatWorld instance;
    [SerializeField]
    List<GameObject> listItemMess = new List<GameObject>();
    private List<JObject> dataChatMain = new List<JObject>();
    void Awake()
    {
        instance = this;
    }
    void Start()
    {


    }
    public void setInfo(JObject data)
    {
        JArray items = JArray.Parse((string)data["data"]);
        if (items.Count > 3)
        {
            for (int i = items.Count - 3; i < items.Count; i++)
            {
                dataChatMain.Add(items[i] as JObject);
            }
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                dataChatMain.Add(items[i] as JObject);
            }
        }
        for (int i = 0; i < dataChatMain.Count; i++)
        {
            var name = (string)dataChatMain[i]["Name"];
            var vip = (int)dataChatMain[i]["Vip"];
            var message = (string)dataChatMain[i]["Data"];
            if (name.Length > 10)
            {
                name = name.Substring(0, 7) + "...";
            }
            if (message.Length > 20)
            {
                message = message.Substring(0, 17) + "...";
            }
            setMessage(listItemMess[i], vip, name,  Globals.Config.Utf16ToUtf8(message));
        }

    }
    // Update is called once per frame
    void Update()
    {

    }
    private void setMessage(GameObject item, int vip, string name, string message)
    {
        Globals.Logging.Log("message Main:" + message);
        string msgTemplate = $"<color=yellow>[V.{vip}]</color><color=green>{name}:</color><color=brown>{message}</color>";
        item.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = msgTemplate;
    }
    public void addMessage(JObject data)
    {

    }
}
