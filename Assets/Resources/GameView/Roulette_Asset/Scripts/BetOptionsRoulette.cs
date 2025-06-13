using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BetOptionsRoulette : MonoBehaviour
{
    [SerializeField] public List<Image> imageChoosing;
    [SerializeField] public Button buttonBetOption;
    public int id;

    private bool isHolding = false;
    private Coroutine turnOffCoroutine = null;

    void Start()
    {
        buttonBetOption.onClick.AddListener(ClickButtonBetOption);
    }

    void OnEnable()
    {
        // Đảm bảo tất cả image choosing đều tắt khi enable lại
        if (imageChoosing != null)
        {
            foreach (var item in imageChoosing)
            {
                if (item != null)
                    item.gameObject.SetActive(false);
            }
        }
    }

    public void PointerUP()
    {
        if (!RouLetteView.instance.isBetTime) return;
        
        isHolding = false;
        if (turnOffCoroutine != null)
        {
            StopCoroutine(turnOffCoroutine);
        }
        turnOffCoroutine = StartCoroutine(TurnOffFlashEffect());
    }

    public void PointerDown()
    {
        if (!RouLetteView.instance.isBetTime) return;
        
        isHolding = true;
        if (turnOffCoroutine != null)
        {
            StopCoroutine(turnOffCoroutine);
            turnOffCoroutine = null;
        }

        if (imageChoosing != null)
        {
            foreach(var item in imageChoosing)
            {
                if (item != null)
                    item.gameObject.SetActive(true);
            }
        }
    }

    public void PointerExit()
    {
        // CheckGroup(false);
        //  foreach(var item in imageChoosing)
        // {
        //     item.gameObject.SetActive(false);
        // }
    }

    public void PointerEnter()
    {
        // if (!RouLetteView.instance.isBetTime)
        // {
        //     return;
        // }
        // CheckGroup(true);
        //  foreach(var item in imageChoosing)
        // {
        //     item.gameObject.SetActive(true);
        // }
    }

    private void ClickButtonBetOption()
    {
        if (!RouLetteView.instance.isBetTime) return;
        if (RouLetteView.instance.agRemaining < RouLetteView.instance.agTable) return;

        RouLetteView.instance.ClickButtonSendBet(id);
    }

    private IEnumerator TurnOffFlashEffect()
    {
        if (!isHolding)
        {
            yield return new WaitForSeconds(0.1f);
            
            if (!isHolding && imageChoosing != null) // Check again in case user pressed during wait
            {
                foreach(var item in imageChoosing)
                {
                    if (item != null)
                        item.gameObject.SetActive(false);
                }
            }
        }
    }

   
}
