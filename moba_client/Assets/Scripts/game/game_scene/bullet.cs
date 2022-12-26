using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    Main,
    Normal,
}

public class bullet : MonoBehaviour
{
    protected int type;
    protected int side;

    protected bullet_config config;

    int active_time;
    float passed_time;
    bool is_running = false;
    int logic_passed_time = 0;
    Vector3 logic_pos;

    protected void Update()
    {
        if (is_running)
        {
            float dt = Time.deltaTime;
            float total = (float)this.active_time / 1000f;
            this.passed_time += dt;
            if (passed_time > total)
            {
                dt -= (this.passed_time - total);
            }

            //跟新自动那位置
            Vector3 offset = this.transform.forward * this.config.speed * dt;
            this.transform.position += offset;

            if (this.passed_time >= total)
            {
                this.is_running = false;
            }
        }
    }

    void hit_test(Vector3 start_pos, float distance)
    {
        RaycastHit[] hits = Physics.RaycastAll(start_pos, this.transform.forward, distance);
        if (hits != null && hits.Length > 0)
        {
            int count = hits.Length;
            for (int i = 0; i < count; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.collider.gameObject.layer == (int)ObjectType.Hero)
                {
                    hero h = hit.collider.GetComponent<hero>();
                    if (h.side == this.side) continue;

                    h.on_attacked(this.config.attack);
                }
            }
        }
    }

    public void shoot_to(Vector3 pos)
    {
        this.transform.LookAt(pos);
        Vector3 dir = pos - this.transform.position;
        float len = dir.magnitude;
        this.active_time = ((int)(len * 1000 / this.config.speed));
        this.passed_time = 0f;
        this.logic_passed_time = 0;
        this.is_running = true;
        this.logic_pos = this.transform.position;
    }

    public virtual void Init(int side, int type)
    {
        this.side = side;
        this.type = type;
        switch (type)
        {
            case (int)TowerType.Main:
                this.config = game_config.main_bullet_config;
                break;
            case (int)TowerType.Normal:
                this.config = game_config.normal_bullet_config;
                break;
        }
    }

    public virtual void on_logic_update(int dt_ms)
    {
        this.logic_passed_time += dt_ms;
        if (this.logic_passed_time > this.active_time)
        {
            dt_ms -= (this.logic_passed_time - this.active_time);
        }

        float dt = (float)dt_ms / 1000f;
        Vector3 offset = this.transform.forward * this.config.speed * dt;

        //子弹路过攻击到了xxx
        this.hit_test(this.logic_pos, offset.magnitude);
        this.logic_pos += offset;

        if (this.logic_passed_time >= this.active_time)
        {
            game_zygote.Instance.remove_bullet(this);
        }
    }
}
