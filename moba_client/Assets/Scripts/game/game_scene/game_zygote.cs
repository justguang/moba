using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OptType
{
    JoyStick = 1,
    Attack1 = 2,
    Attack2 = 3,
    Attack3 = 4,
    Skill1 = 5,
    Skill2 = 6,
    Skill3 = 7,
}

public enum SideType
{
    SideA = 0,
    SideB = 1,
}

public enum ObjectType
{
    Bullet = 12,
    Hero = 13,
    Tower = 14,
}

public class game_zygote : MonoBehaviour
{
    public static game_zygote Instance;
    //test
    public joystick stick;
    //end

    [SerializeField] private GameObject[] hero_character_prefabs = null;//0 男， 1 女
    [SerializeField] private Transform map_entry_A;//Side A 方出生点
    [SerializeField] private Transform map_entry_B;
    [SerializeField] private Transform[] towers_A;//[主塔，left，right，front]
    [SerializeField] private Transform[] towers_B;

    private int sync_frameid = 0;
    private FrameOpts last_fram_opt;
    private List<hero> heroLst = new List<hero>();//所有玩家
    private List<tower> towers_A_Lst = new List<tower>();//[主塔，left，right，front]
    private List<tower> towers_B_Lst = new List<tower>();

    [SerializeField] private GameObject main_bullet_prefab;//子弹预制体
    [SerializeField] private GameObject normal_bullet_prefab;
    private List<bullet> tower_bullets = new List<bullet>();//管理所有子弹

    public const int LOGIC_FRAME_TIME = 66;//逻辑帧时间间隔【毫秒】

    void Start()
    {
        Instance = this;

        //event
        event_manager.Instance.add_event_listener("on_logic_update", this.on_logic_update);
        //end


        //创建英雄
        this.place_heroes();

        //创建防御塔
        this.place_towers();
    }

    void OnDestroy()
    {
        event_manager.Instance.remove_event_listener("on_logic_update", this.on_logic_update);
        Instance = null;
    }


    void upgrade_exp_by_time()
    {
        int count = this.heroLst.Count;
        for (int i = 0; i < count; i++)
        {
            this.heroLst[i].add_exp(game_config.add_exp_per_logic);
        }
    }

    public bullet alloc_bullet(int side, int type)
    {
        GameObject obj = null;
        bullet b = null;
        switch (type)
        {
            case (int)BulletType.Main:
                obj = GameObject.Instantiate(this.main_bullet_prefab);
                obj.transform.SetParent(this.transform, false);
                b = obj.AddComponent<normal_bullet>();
                b.Init(side, type);
                b.gameObject.name = "Main_Bullet";
                break;
            case (int)BulletType.Normal:
                obj = GameObject.Instantiate(this.normal_bullet_prefab);
                obj.transform.SetParent(this.transform, false);
                b = obj.AddComponent<normal_bullet>();
                b.Init(side, type);
                b.gameObject.name = "Normal_Bullet";
                break;
        }

        if (b != null)
        {
            this.tower_bullets.Add(b);
        }

        return b;
    }

    public void remove_bullet(bullet b)
    {
        if (b != null)
        {
            this.tower_bullets.Remove(b);
        }
        GameObject.Destroy(b.gameObject);
    }

    public List<hero> get_heors()
    {
        return this.heroLst;
    }

    void place_heroes()
    {
        hero h = this.place_hero_at(ugame.Instance.players_match_info[0], 0);
        this.heroLst.Add(h);
        h = this.place_hero_at(ugame.Instance.players_match_info[1], 1);
        this.heroLst.Add(h);


        h = this.place_hero_at(ugame.Instance.players_match_info[2], 0);
        this.heroLst.Add(h);
        h = this.place_hero_at(ugame.Instance.players_match_info[3], 1);
        this.heroLst.Add(h);
    }

    void place_towers()
    {
        tower t;
        //side A
        t = this.towers_A[0].gameObject.AddComponent<main_tower>();
        t.Init((int)SideType.SideA, (int)TowerType.Main);
        t.gameObject.name = "Tower_A_Main";
        this.towers_A_Lst.Add(t);//主塔

        t = this.towers_A[1].gameObject.AddComponent<normal_tower>();
        t.Init((int)SideType.SideA, (int)TowerType.Normal);
        t.gameObject.name = "Tower_A_Normal_Left";
        this.towers_A_Lst.Add(t);//left

        t = this.towers_A[2].gameObject.AddComponent<normal_tower>();
        t.Init((int)SideType.SideA, (int)TowerType.Normal);
        t.gameObject.name = "Tower_A_Normal_Right";
        this.towers_A_Lst.Add(t);//right

        t = this.towers_A[3].gameObject.AddComponent<normal_tower>();
        t.Init((int)SideType.SideA, (int)TowerType.Normal);
        t.gameObject.name = "Tower_A_Normal_Front";
        this.towers_A_Lst.Add(t);//front

        //side B
        t = this.towers_B[0].gameObject.AddComponent<main_tower>();
        t.Init((int)SideType.SideB, (int)TowerType.Main);
        t.gameObject.name = "Tower_B_Main";
        this.towers_B_Lst.Add(t);//主塔

        t = this.towers_B[1].gameObject.AddComponent<normal_tower>();
        t.Init((int)SideType.SideB, (int)TowerType.Normal);
        t.gameObject.name = "Tower_B_Normal_Left";
        this.towers_B_Lst.Add(t);//left

        t = this.towers_B[2].gameObject.AddComponent<normal_tower>();
        t.Init((int)SideType.SideB, (int)TowerType.Normal);
        t.gameObject.name = "Tower_B_Normal_Right";
        this.towers_B_Lst.Add(t);//right

        t = this.towers_B[3].gameObject.AddComponent<normal_tower>();
        t.Init((int)SideType.SideB, (int)TowerType.Normal);
        t.gameObject.name = "Tower_B_Normal_Front";
        this.towers_B_Lst.Add(t);//front
    }

    hero get_hero(int seatid)
    {
        int hero_count = this.heroLst.Count;
        for (int i = 0; i < hero_count; i++)
        {
            if (this.heroLst[i].seatid == seatid)
            {
                return this.heroLst[i];
            }
        }
        return null;
    }

    hero place_hero_at(PlayerMatchInfo match_info, int index)
    {
        if (match_info == null) return null;
        user_info uinfo = ugame.Instance.get_user_info(match_info.Seatid);
        if (uinfo == null) return null;
        int side = match_info.Side;

        GameObject hero_obj = GameObject.Instantiate(this.hero_character_prefabs[uinfo.usex]);
        hero_obj.name = uinfo.unick;
        hero_obj.transform.SetParent(this.transform, false);
        Vector3 born_pos = side == 0 ? this.map_entry_A.position : this.map_entry_B.position;
        born_pos.z -= 3f;
        born_pos.z += index * 2f;
        hero_obj.transform.position = born_pos;
        hero ctrl = hero_obj.AddComponent<hero>();
        ctrl.is_ghost = match_info.Seatid != ugame.Instance.self_seatid;
        ctrl.seatid = match_info.Seatid;
        ctrl.side = side;
        ctrl.logic_init(born_pos);

        return ctrl;
    }

    //采集下一帧事件操作
    void capture_player_opts()
    {
        NextFrameOpts next_frame = new NextFrameOpts
        {
            Frameid = this.sync_frameid + 1,
            Zid = ugame.Instance.zid,
            Matchid = ugame.Instance.matchid,
            Seatid = ugame.Instance.self_seatid,
        };

        //摇杆
        OptionEvent opt_stick = new OptionEvent();
        opt_stick.Seatid = ugame.Instance.self_seatid;
        opt_stick.OptType = (int)OptType.JoyStick;
        opt_stick.X = (int)(this.stick.dir.x * (1 << 16));//32位【16.16】
        opt_stick.Y = (int)(this.stick.dir.y * (1 << 16));//32位【16.16】
        next_frame.Opts.Add(opt_stick);

        //将采集好的操作发送给服务器
        logic_service_proxy.Instance.send_next_frame_opts(next_frame);
    }

    void on_handler_frame_event(FrameOpts frame)
    {
        //处理所有英雄的操作
        int count = frame.Opts.Count;
        for (int i = 0; i < count; i++)
        {
            int seatid = frame.Opts[i].Seatid;
            hero h = this.get_hero(seatid);
            if (h != null)
            {
                h.on_handler_frame_event(frame.Opts[i]);
            }
            else
            {
                Debug.LogError("cannot find hero: " + seatid);
            }
        }
        this.on_frame_handle_tower_bullet_logic();

        this.on_frame_handle_tower_logic();
    }

    void on_sync_last_logic_frame(FrameOpts frame)
    {
        int count = frame.Opts.Count;
        for (int i = 0; i < count; i++)
        {
            int seatid = frame.Opts[i].Seatid;
            hero h = this.get_hero(seatid);
            if (h != null)
            {
                h.on_sync_last_logic_frame(frame.Opts[i]);
            }
            else
            {
                Debug.LogError("cannot find hero: " + seatid);
            }
        }

    }

    void on_jump_to_next_frame(FrameOpts frame)
    {
        int count = frame.Opts.Count;
        for (int i = 0; i < count; i++)
        {
            int seatid = frame.Opts[i].Seatid;
            hero h = this.get_hero(seatid);
            if (h != null)
            {
                h.on_jump_to_next_frame(frame.Opts[i]);
            }
            else
            {
                Debug.LogError("cannot find hero: " + seatid);
            }

        }
        this.on_frame_handle_tower_bullet_logic();

        this.on_frame_handle_tower_logic();
    }

    //塔的logic
    void on_frame_handle_tower_logic()
    {
        int count = this.towers_A_Lst.Count;
        for (int i = 0; i < count; i++)
        {
            this.towers_A_Lst[i].on_logic_update(LOGIC_FRAME_TIME);
        }

        count = this.towers_B_Lst.Count;
        for (int i = 0; i < count; i++)
        {
            this.towers_B_Lst[i].on_logic_update(LOGIC_FRAME_TIME);
        }
    }

    //子弹的logic
    void on_frame_handle_tower_bullet_logic()
    {
        for (int i = 0; i < this.tower_bullets.Count; i++)
        {
            this.tower_bullets[i].on_logic_update(LOGIC_FRAME_TIME);
        }
    }

    void on_logic_update(string event_name, object udata)
    {
        LogicFrame frame = (LogicFrame)udata;
        if (frame.Frameid < this.sync_frameid) return;

        //step1：同步上一帧 本机客户端上一帧逻辑帧的操作
        if (this.last_fram_opt != null)
        {
            this.on_sync_last_logic_frame(this.last_fram_opt);
        }

        //step2：同步丢帧 sync_framid + 1  ----> frame.frameid - 1  位同步的所有帧【丢失的帧】
        int frame_count = frame.UnsyncFrames.Count;
        for (int i = 0; i < frame_count; i++)
        {
            if (this.sync_frameid >= frame.UnsyncFrames[i].Frameid) continue;
            if (frame.UnsyncFrames[i].Frameid >= frame.Frameid) break;

            this.on_jump_to_next_frame(frame.UnsyncFrames[i]);
            this.upgrade_exp_by_time();
        }

        //step3：同步到最新帧 获取最新帧【最后一个帧操作】，计算处理
        this.sync_frameid = frame.Frameid;//同步到的事件帧ID
        if (frame.UnsyncFrames != null && frame.UnsyncFrames.Count > 0)
        {
            this.last_fram_opt = frame.UnsyncFrames[frame.UnsyncFrames.Count - 1];
            this.on_handler_frame_event(this.last_fram_opt);
            this.upgrade_exp_by_time();
        }
        else
        {
            this.last_fram_opt = null;
        }



        //采集下一个帧的操作事件，发送给服务器
        this.capture_player_opts();
    }


}
