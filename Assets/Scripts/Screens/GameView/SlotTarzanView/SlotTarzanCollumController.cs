using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotTarzanCollumController : CollumSpinController
{
    // Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
    public void addDiamondPosition(Vector2 posDiamond)
    {
        ((SlotTarzanView)gameView).listDiamondPos.Add(posDiamond);

    }
    public override void onCollumStop()
    {
        itemResult.listIdIcon.ForEach(id =>
        {
            if (id == 14)
            {
                Globals.Logging.Log("add Diamond Position");
                addDiamondPosition(itemResult.getPositionItem(itemResult.listIdIcon.IndexOf(id)));
            }
        });
        gameView.nextColStop();
    }
}
