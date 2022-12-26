using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class normal_bullet : bullet
{

    public override void Init(int side, int type)
    {
        base.Init(side, type);

    }


    public override void on_logic_update(int dt_ms)
    {
        base.on_logic_update(dt_ms);
        //Debug.Log("normal bullet");
    }
}
