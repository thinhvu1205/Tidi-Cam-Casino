using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json.Linq;
using DG.Tweening;

public class SlotJuicyGardenCollumController : CollumSpinController
{
    // Start is called before the first frame update
    private SlotJuicyGardenItemSpin itemSpinJuicy;
    protected override void Start()
    {
        base.Start();
        //SPEED_TYPE["NORMAL"] = 2.0f;
        //SPEED_TYPE["AUTO"] = 2.0f;
    }
    public void setLightAllItem()
    {
        listItem.ForEach(item =>{
            SlotJuicyGardenItemSpin itemJuicy = (SlotJuicyGardenItemSpin)item;
            itemJuicy.setLight();
        });
        
    }
    public override void startSpin(BaseSlotGameView reference)
    {
        base.startSpin(reference);
        //SlotJuicyGardenView gardenView = (SlotJuicyGardenView)reference;
        //if (gardenView.isBonusGame && gardenView.freespinLeft > 0)
        //{
        //    setDarkItem(true);
        //}
    }
}
