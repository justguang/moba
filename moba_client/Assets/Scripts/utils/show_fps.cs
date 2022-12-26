using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class show_fps : MonoBehaviour
{
    private float time_delta = 1f;//每隔一段时间统计fps
    private float prev_time = 0.0f;//上一次统计FPS的时间

    private float fps = 0.0f;//计算出来的FPS
    private int i_frames = 0;//累积刷新的帧数

    private GUIStyle style;//GUI

    #region Unity Func
    void Awake()
    {
        Application.targetFrameRate = 60;
    }
    void Start()
    {
        this.prev_time = Time.realtimeSinceStartup;
        this.style = new GUIStyle();
        this.style.fontSize = 15;
        this.style.normal.textColor = new Color(255, 255, 255);
    }
    void OnGUI()
    {
        //GUI.Label(new Rect(0, 0, 200, 200), "FPS:" + this.fps.ToString("f2"), this.style);
        GUI.Label(new Rect(0, Screen.height - 20, 200, 200), "FPS:" + this.fps.ToString("f2"), this.style);
    }

    void Update()
    {
        this.i_frames++;
        if (Time.realtimeSinceStartup >= this.prev_time + this.time_delta)
        {
            this.fps = (float)this.i_frames / (Time.realtimeSinceStartup - this.prev_time);
            this.prev_time = Time.realtimeSinceStartup;
            this.i_frames = 0;
        }
    }
    #endregion
}
