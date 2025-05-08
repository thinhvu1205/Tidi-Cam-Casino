using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using TMPro;
using Globals;
using UnityEngine.UI.Extensions;


public class ItemGame : MonoBehaviour
{
    [SerializeField] SkeletonGraphic m_LargeSG, m_SmallSG, m_LeanSG;
    [SerializeField] Gradient2 m_LargeBorderG2, m_SmallBorderG2, m_LeanBorderG2;
    [SerializeField] TextNumberControl m_JackPotTNC;
    [HideInInspector] public int GameId;
    System.Action callbackClick = null;

    public void setInfo(int _gameID, SkeletonDataAsset skeAnim, Material material, System.Action callback, bool isShowAllGames = true)
    {
        GameId = _gameID;
        callbackClick = callback;
        if (skeAnim != null)
        {
            SkeletonGraphic shownSG = null;
            Gradient2 borderG2 = null;
            GradientColorKey[] colorsGCK = new GradientColorKey[2];
            colorsGCK[1] = new(Color.white, 1);
            GradientAlphaKey[] alphaGAK = new GradientAlphaKey[2];
            alphaGAK[0] = new(1, 0);
            alphaGAK[1] = new(1, 1);
            if ((!isShowAllGames && Config.listGameSlot.Contains(GameId)) ||
                        GameId == (int)GAMEID.TONGITS_OLD || GameId == (int)GAMEID.PUSOY || GameId == (int)GAMEID.LUCKY9)
            {
                shownSG = m_LargeSG;
                borderG2 = m_LargeBorderG2;
                Destroy(m_SmallBorderG2.transform.parent.gameObject);
            }
            else
            {
                shownSG = m_SmallSG;
                borderG2 = m_SmallBorderG2;
                Destroy(m_LargeBorderG2.transform.parent.gameObject);
            }
            Destroy(m_LeanBorderG2.transform.parent.gameObject);
            switch (GameId)
            {
                case (int)GAMEID.LUCKY9:
                    {
                        colorsGCK[0] = new GradientColorKey(new(0.4705883f, 0.5294118f, 0.9686275f), 0);
                        break;
                    }
                case (int)GAMEID.TONGITS_OLD:
                    {
                        colorsGCK[0] = new GradientColorKey(new(0.4901961f, 0.509804f, 0.9843138f), 0);
                        break;
                    }
                case (int)GAMEID.PUSOY:
                    {
                        colorsGCK[0] = new GradientColorKey(new(0.9725491f, 0.7254902f, 0.4235294f), 0);
                        break;
                    }
                case (int)GAMEID.TONGITS_JOKER:
                    {
                        colorsGCK[0] = new GradientColorKey(new(0.764706f, 0.6666667f, 1f), 0);
                        break;
                    }
                case (int)GAMEID.TONGITS:
                    {
                        colorsGCK[0] = new GradientColorKey(new(0.6901961f, 0.8901961f, 0.8392158f), 0);
                        break;
                    }
                case (int)GAMEID.BACCARAT:
                    {
                        colorsGCK[0] = new GradientColorKey(new(0.7411765f, 0.7843138f, 1), 0);
                        break;
                    }
                case (int)GAMEID.LUCKY_89:
                    {
                        colorsGCK[0] = new GradientColorKey(new(1, 0.7882354f, 0.5803922f), 0);
                        break;
                    }
                case (int)GAMEID.SABONG:
                    {
                        colorsGCK[0] = new GradientColorKey(new(0.7411765f, 0.7843138f, 1), 0);
                        break;
                    }
                case (int)GAMEID.SICBO:
                    {
                        colorsGCK[0] = new GradientColorKey(new(0.7725491f, 0.6745098f, 0.9960785f), 0);
                        break;
                    }
                case (int)GAMEID.SLOTTARZAN:
                    {
                        colorsGCK[0] = new GradientColorKey(new(0.627451f, 0.8470589f, 0.7921569f), 0);
                        break;
                    }
                case (int)GAMEID.SLOT_INCA:
                    {
                        colorsGCK[0] = new GradientColorKey(new(1, 0.7882354f, 0.5803922f), 0);
                        break;
                    }
                case (int)GAMEID.SLOT20FRUIT:
                    {
                        colorsGCK[0] = new GradientColorKey(new(0.7921569f, 0.9450981f, 0.9725491f), 0);
                        break;
                    }
                case (int)GAMEID.SLOTNOEL:
                    {
                        colorsGCK[0] = new GradientColorKey(new(0.9568628f, 0.5686275f, 0.7843138f), 0);
                        break;
                    }
                case (int)GAMEID.SLOT_JUICY_GARDEN:
                    {
                        colorsGCK[0] = new GradientColorKey(new(0.6941177f, 0.8980393f, 0.8470589f), 0);
                        break;
                    }
                case (int)GAMEID.SLOT_SIXIANG:
                    {
                        colorsGCK[0] = new GradientColorKey(new(0.5647059f, 0.5960785f, 0.9764706f), 0);
                        break;
                    }
                case (int)GAMEID.MINE_FINDING:
                    {
                        colorsGCK[0] = new GradientColorKey(new(0.9568628f, 0.5686275f, 0.7843138f), 0);
                        break;
                    }
            }
            borderG2.transform.parent.gameObject.SetActive(true);
            UnityEngine.Gradient gradientG = new();
            gradientG.SetKeys(colorsGCK, alphaGAK);
            borderG2.EffectGradient = gradientG;

            shownSG.skeletonDataAsset = skeAnim;
            shownSG.material = material;
            Spine.Animation[] ab = skeAnim.GetSkeletonData(false).Animations.ToArray();
            string nameAnim = ab[ab.Length - 1].Name;
            shownSG.Initialize(true);
            shownSG.startingAnimation = nameAnim;
            shownSG.AnimationState.SetAnimation(0, nameAnim, true);
        }
    }
    public void UpdateJackpot(long number)
    {
        m_JackPotTNC.setValue(number, true);
        m_JackPotTNC.transform.parent.gameObject.SetActive(true);
    }

    public void onClick()
    {
        if (callbackClick != null)
        {
            callbackClick.Invoke();
        }
    }
}
