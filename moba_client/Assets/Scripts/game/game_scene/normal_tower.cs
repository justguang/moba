using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class normal_tower : tower
{
    private int now_fps;
    void Start()
    {
    }

    public override void Init(int side, int type)
    {
        base.Init(side, type);
        this.now_fps = this.config.shoot_logic_fps;
    }

    void shoot_at(Vector3 pos)
    {
        normal_bullet b = (normal_bullet)game_zygote.Instance.alloc_bullet(this.side, (int)BulletType.Normal);
        b.transform.position = this.transform.Find("point").transform.position;
        b.shoot_to(pos);
    }

    void do_shoot()
    {
        List<hero> heros = game_zygote.Instance.get_heors();

        hero target = null;
        float min_len = this.config.attack_R + 1;
        int count = heros.Count;
        for (int i = 0; i < count; i++)
        {
            hero h = heros[i];
            if (h.side == this.side) continue;

            Vector3 dir = h.transform.position - this.transform.position;
            float len = dir.magnitude;
            if (len > min_len) continue;

            target = h;
        }

        if (target != null)
        {
            CharacterController ctrl = target.GetComponent<CharacterController>();
            Vector3 pos = target.transform.position;
            pos.y += (ctrl.height * 0.6f);
            this.shoot_at(pos);
        }
    }

    public override void on_logic_update(int dt_ms)
    {
        this.now_fps++;
        if (this.now_fps >= this.config.shoot_logic_fps)
        {
            this.now_fps = 0;
            this.do_shoot();
        }
    }
}
