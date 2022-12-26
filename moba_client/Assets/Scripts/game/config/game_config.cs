using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class game_config
{
    public static tower_config main_tower_config = new tower_config
    {
        hp = 10,
        attack_R = 10,
        shoot_logic_fps = 3,
    };

    public static tower_config normal_tower_config = new tower_config
    {
        hp = 10,
        attack_R = 10,
        shoot_logic_fps = 5,
    };

    public static bullet_config main_bullet_config = new bullet_config
    {
        attack = 10,
        speed = 20,
        max_distance = 20,
    };

    public static bullet_config normal_bullet_config = new bullet_config
    {
        attack = 10,
        speed = 20,
        max_distance = 20,
    };

    public static hero_level_config[] normal_hero_level_config = new hero_level_config[]
    {
        new hero_level_config
        {
            defense = 1,
            attack = 1,
            add_blood = 0,
            max_blood = 200,
            exp = 0,
        },
        new hero_level_config
        {
            defense = 2,
            attack = 1,
            add_blood = 50,
            max_blood = 400,
            exp = 200,
        },
        new hero_level_config
        {
            defense = 3,
            attack = 1,
            add_blood = 100,
            max_blood = 600,
            exp = 400,
        },
        new hero_level_config
        {
            defense = 4,
            attack = 1,
            add_blood = 150,
            max_blood = 800,
            exp = 600,
        },
        new hero_level_config
        {
            defense = 5,
            attack = 1,
            add_blood = 200,
            max_blood = 1000,
            exp = 800,
        },
    };

    public static int add_exp_per_logic = 1;//每一逻辑帧成长1点
    public static int exp2level(hero_level_config[] configs, int exp)
    {
        int level = 0;

        while (level + 1 < configs.Length
            && exp > configs[level + 1].exp)
        {
            level++;
            exp -= configs[level].exp;
        }

        return level;
    }

    public static void upgrade_level_info(hero_level_config[] configs, int exp, ref int now, ref int total)
    {
        int level = 0;
        while (level + 1 < configs.Length && exp > configs[level + 1].exp)
        {
            level++;
            exp -= configs[level].exp;
        }

        if (level + 1 >= configs.Length)
        {
            now = total = configs[level].exp;
        }
        else
        {
            now = exp;
            total = configs[level + 1].exp;
        }

    }
}

public class tower_config
{
    public int hp;//生命力
    public int attack_R;//攻击范围
    public int shoot_logic_fps;//子弹频率
}

public class bullet_config
{
    public int attack;//攻击力
    public int speed;//子弹移速
    public int max_distance;//子弹最大有效范围
}

public class hero_level_config
{
    public int defense;//防御
    public int attack;//攻击力
    public int max_blood;//当前等级最大血量
    public int add_blood;//当前等级增加的血量
    public int exp;
}


