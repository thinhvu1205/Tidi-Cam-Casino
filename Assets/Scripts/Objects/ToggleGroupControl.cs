using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleGroupControl : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] List<Toggle> listToggle;
    [SerializeField] List<GameObject> listTabView;

    [SerializeField] GameObject currentTab;

    void Start()
    {
        if (currentTab != null)
        {
            currentTab.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void onTabClick(int index)
    {
        if (currentTab != null)
        {
            currentTab.SetActive(false);
        }
        currentTab = listTabView[index];
        currentTab.SetActive(true);
    }
}
