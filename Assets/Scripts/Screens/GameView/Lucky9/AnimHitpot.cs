using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Spine.Unity;
using System.Threading.Tasks;
using System.Linq;
using Globals;
using UnityEngine.UI;

public class AnimHitpot : MonoBehaviour
{
    public TextMeshProUGUI lb_score;
    public Avatar avatarNode;
    public SkeletonGraphic animHitPot;
    public LayoutGroup layout_Hitpot;

    List<PlayerData> listPlayerHitPot = new List<PlayerData>();

    public void show()
    {
        gameObject.SetActive(true);
        if (listPlayerHitPot.Count > 0)
        {
            int winHitPot = listPlayerHitPot[0].pointWin;
            if (winHitPot > 0)
            {
                lb_score.text = winHitPot.ToString();
            }
        }
        arrangePlayerInLayout();
        animHitPot.AnimationState.SetAnimation(0, "animation", false);
        animHitPot.Initialize(true);
        animHitPot.AnimationState.Complete += delegate
        {
            animHitPot.gameObject.SetActive(false);
        };
    }

    public void addPlayerHitPot(PlayerData dataPlayer)
    {
        listPlayerHitPot.Add(dataPlayer);
    }

    public void arrangePlayerInLayout()
    {
        int length = listPlayerHitPot.Count;
        float scale = 1;
        HorizontalOrVerticalLayoutGroup layoutGroup = layout_Hitpot as HorizontalOrVerticalLayoutGroup;

        if (layoutGroup != null)
        {
            layoutGroup.spacing = 0;
        }

        //if (layout_Hitpot is GridLayoutGroup gridLayoutGroup)
        //{
        //    gridLayoutGroup.spacing = Vector2.zero;
        //    gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        //    gridLayoutGroup.constraintCount = 1;
        //}

        switch (length)
        {
            case 1:
                scale = 1;
                break;
            case 2:
                scale = 1;
                if (layoutGroup != null)
                {
                    //layoutGroup.spacing = 50;
                }
                break;
            case 3:
                scale = 0.9f;
                break;
            case 4:
                scale = 0.9f;
                if (layoutGroup != null)
                {
                    layoutGroup.spacing = -10;
                }
                break;
            case 5:
                scale = 0.7f;
                break;
            case 6:
                scale = .6f;
                break;
            case 7:
                scale = 0.5f;
                if (layout_Hitpot is GridLayoutGroup grid)
                {
                    grid.spacing = new Vector2(-10, -30);
                    grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                    grid.constraintCount = 2;
                }
                break;
        }

        foreach (Transform child in layout_Hitpot.transform)
        {
            Destroy(child.gameObject);
        }

        for (int index = 0; index < length; index++)
        {
            PlayerData element = listPlayerHitPot[index];
            Avatar player = Instantiate(avatarNode, layout_Hitpot.transform);

            player.transform.localScale = new Vector3(scale * 1.4f, scale * 1.4f, scale * 1.4f);
            player.gameObject.SetActive(true);
            player.loadAvatar(element.player.avatar_id, element.player.displayName, element.player.fid, element.player.displayName);
        }

        layout_Hitpot.gameObject.SetActive(false);
        StartCoroutine(ReactivateLayout());
    }

    private IEnumerator ReactivateLayout()
    {
        yield return new WaitForSeconds(0.5f);
        layout_Hitpot.gameObject.SetActive(true);
    }

    public void reset()
    {
        foreach (Transform child in layout_Hitpot.transform)
        {
            Destroy(child.gameObject);
        }
        //layout_Hitpot.GetComponent<RectTransform>().sizeDelta = new Vector2(450, layout_Hitpot.GetComponent<RectTransform>().sizeDelta.y);
        listPlayerHitPot.Clear();
    }

    public void hide()
    {
        StartCoroutine(HideCoroutine());
    }

    private IEnumerator HideCoroutine()
    {
        float duration = 0.5f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = new Vector3(0.2f, 0.2f, 0.2f);

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            transform.localScale = Vector3.Lerp(startScale, endScale, normalizedTime);
            yield return null;
        }

        transform.localScale = endScale;
        reset();
        transform.localScale = Vector3.one;
        gameObject.SetActive(false);
    }
}