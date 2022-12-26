using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class main_tower : tower
{
    public override void Init(int side, int type)
    {
        base.Init(side, type);

    }

    public override void on_logic_update(int dt_ms)
    {
        //Debug.Log("main tower");
    }
}
