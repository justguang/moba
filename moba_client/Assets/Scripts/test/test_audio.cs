using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test_audio : MonoBehaviour
{
    public AudioClip clip;
    // 在第一帧更新之前调用Start
    void Start()
    {
        audio_manager.Instance.play_music(clip);
        //audio_manager.Instance.play_effect(clip);
        this.Invoke("test", 5f);
    }

    void test()
    {
        audio_manager.Instance.enable_music(false);
    }


    // 每帧调用一次Update
    void Update()
    {
        
    }
}
