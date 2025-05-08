using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyBonusCollumSpin : CollumSpinController
{
    public virtual void startSpin()
    {
        isStop = false;
        for (int i = 0; i < listItem.Count; i++)
        {
            listItem[i].startSpin();
        }
    }
    public override void onCollumStop()
    {
        DailyBonusView.instance.nextColStop();
    }
    public override void onCollumStopCompleted()
    {
        DailyBonusView.instance.spinFinish();
    }
}
