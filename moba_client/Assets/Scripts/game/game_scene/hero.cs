using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

enum charactor_state
{
    walk = 1,
    free = 2,
    idle = 3,
    attack = 4,
    attack2 = 5,
    attack3 = 6,
    skill = 7,
    skill2 = 8,
    death = 9,
}

public class hero : MonoBehaviour
{
    public bool is_ghost;//true：其他玩家，   false：本机玩家
    public float speed = 8.0f;//角色移速
    public int seatid = -1;//座位号
    public int side = -1;//阵营【left， right】


    private CharacterController ctrl;
    private Animation anim;
    private charactor_state anim_state = charactor_state.idle;
    private Vector3 camera_offset;

    private int stick_x = 0;
    private int stick_y = 0;
    private charactor_state logic_state = charactor_state.idle;
    private Vector3 logic_pos;//保存当前逻辑帧的位置

    private int blood;
    private int level;
    private int exp;

    void Start()
    {
        GameObject ring_res = Resources.Load<GameObject>("effect/other/guangquan_fanwei");
        this.ctrl = this.GetComponent<CharacterController>();
        this.anim = this.GetComponent<Animation>();
        if (this.is_ghost == false)
        {
            //本机控制
            GameObject ring = GameObject.Instantiate(ring_res);
            ring.transform.SetParent(this.transform, false);
            ring.transform.localPosition = Vector3.zero;
            ring.transform.localScale = new Vector3(2, 1, 2);

            if (this.side == 1)
            {
                //side B   267  82  102
                Camera.main.transform.localPosition = new Vector3(267f, 82f, 102f);
                Camera.main.transform.localEulerAngles = new Vector3(50f, 255f, 0);
            }
            else
            {
                //side A
                Camera.main.transform.localPosition = new Vector3(37f, 82f, 87f);
                Camera.main.transform.localEulerAngles = new Vector3(50f, 45f, 0);
            }

            this.camera_offset = Camera.main.transform.position - this.transform.position;
        }

        this.init_hero_params();
        this.anim.Play("idle");
    }

    void Update()
    {
        this.on_joystick_anim_update();
    }

    void init_hero_params()
    {
        this.level = 0;
        this.blood = game_config.normal_hero_level_config[this.level].max_blood;
        this.exp = game_config.normal_hero_level_config[this.level].exp;
        this.sync_blood_ui();
        this.sync_exp_ui();
    }


    void sync_blood_ui()
    {
        if (this.is_ghost == false)
        {
            ui_blood_info info = new ui_blood_info();
            info.blood = this.blood;
            info.max_blood = game_config.normal_hero_level_config[this.level].max_blood;
            event_manager.Instance.dispatch_event("blood_ui_sync", info);
        }
    }

    void sync_exp_ui()
    {
        if (this.is_ghost == false)
        {
            ui_exp_info info = new ui_exp_info();
            int now = 0, total = 0;
            game_config.upgrade_level_info(game_config.normal_hero_level_config, this.exp, ref now, ref total);
            info.exp = now;
            info.total = total;

            event_manager.Instance.dispatch_event("exp_ui_sync", info);
        }
    }


    void on_joystick_anim_update()
    {
        if (this.logic_state != charactor_state.idle
            && this.logic_state != charactor_state.walk)
        {
            return;
        }

        if (this.stick_x == 0 && this.stick_y == 0)
        {
            if (this.anim_state == charactor_state.walk)
            {
                this.anim.CrossFade("idle");
                this.anim_state = charactor_state.idle;
            }
            return;
        }

        if (this.anim_state == charactor_state.idle)
        {
            this.anim.CrossFade("walk");
            this.anim_state = charactor_state.walk;
        }

        this.do_joystick_event(Time.deltaTime);

        if (is_ghost == false)
        {
            Camera.main.transform.position = this.transform.position + camera_offset;
        }
    }

    void do_joystick_event(float dt)
    {
        if (this.stick_x == 0 && this.stick_y == 0)
        {
            this.logic_state = charactor_state.idle;
            return;
        }

        this.logic_state = charactor_state.walk;
        float dir_x = (float)this.stick_x / (float)(1 << 16);
        float dir_y = (float)this.stick_y / (float)(1 << 16);
        float r = Mathf.Atan2(dir_y, dir_x);

        float s = this.speed * dt;
        float offset = (this.side == 0) ? (-Mathf.PI * 0.25f) : (Mathf.PI * 0.75f);
        float sx = s * Mathf.Cos(r + offset);
        float sz = s * Mathf.Sin(r + offset);
        this.ctrl.Move(new Vector3(sx, 0, sz));

        float degree = r * 180f / Mathf.PI;
        offset = (this.side == 0) ? 45 : -135;
        degree = 360f - degree + 90 + offset;
        this.transform.localEulerAngles = new Vector3(0, degree, 0);

    }

    //摇杆
    void handle_joystick_event(OptionEvent opt)
    {
        this.stick_x = opt.X;
        this.stick_y = opt.Y;
        if (this.stick_x == 0 && this.stick_x == 0)
        {
            this.logic_state = charactor_state.idle;
        }
        else
        {
            this.logic_state = charactor_state.walk;
        }
    }

    void sync_last_joystick_event(OptionEvent opt)
    {
        this.stick_x = opt.X;
        this.stick_y = opt.Y;
        this.transform.position = this.logic_pos;
        this.do_joystick_event(game_zygote.LOGIC_FRAME_TIME / 1000.0f);
        this.logic_pos = this.transform.position;
    }

    void jump_joystick_event(OptionEvent opt) => this.sync_last_joystick_event(opt);


    public void on_handler_frame_event(OptionEvent opt)
    {
        switch (opt.OptType)
        {
            case (int)OptType.JoyStick:
                this.handle_joystick_event(opt);
                break;
            case (int)OptType.Attack1:
                break;
            case (int)OptType.Skill1:
                break;
            default:
                break;
        }
    }

    public void on_sync_last_logic_frame(OptionEvent opt)
    {
        switch (opt.OptType)
        {
            case (int)OptType.JoyStick:
                this.sync_last_joystick_event(opt);
                break;
            case (int)OptType.Attack1:
                break;
            case (int)OptType.Skill1:
                break;
            default:
                break;
        }
    }

    public void on_jump_to_next_frame(OptionEvent opt)
    {
        switch (opt.OptType)
        {
            case (int)OptType.JoyStick:
                this.jump_joystick_event(opt);
                break;
            case (int)OptType.Attack1:
                break;
            case (int)OptType.Skill1:
                break;
            default:
                break;
        }
    }

    public void logic_init(Vector3 logic_pos)
    {
        this.stick_x = 0;
        this.stick_y = 0;
        this.logic_state = charactor_state.idle;
        this.logic_pos = logic_pos;
    }

    public void add_exp(int exp_value)
    {
        this.exp += exp_value;
        int level = game_config.exp2level(game_config.normal_hero_level_config, this.exp);
        if (level != this.level)
        {
            this.level = level;
            this.blood += game_config.normal_hero_level_config[this.level].add_blood;
            int max_blood = game_config.normal_hero_level_config[this.level].max_blood;
            this.blood = (this.blood > max_blood) ? max_blood : this.blood;

            this.sync_blood_ui();
        }
        this.sync_exp_ui();
    }

    public void on_attacked(int attack_value)
    {
        //Debug.Log("hero " + this.transform.name + " 被攻击了，value=" + attack_value);
        attack_value -= game_config.normal_hero_level_config[this.level].defense;
        if (attack_value <= 0) return;

        this.blood -= attack_value;
        this.blood = (this.blood < 0) ? 0 : this.blood;
        this.sync_blood_ui();
    }
}
