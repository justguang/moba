using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class joystick : MonoBehaviour
{
    [SerializeField] private Canvas cs;
    [SerializeField] private Transform stick;
    public float max_R = 80f;

    private Vector2 touch_dir = Vector2.zero;
    public Vector2 dir { get { return this.touch_dir; } }

    void Start()
    {
        this.stick.localPosition = Vector2.zero;
        this.touch_dir = Vector2.zero;
    }

    void Update()
    {

    }

    public void on_stick_drag()
    {
        //Debug.Log("on_stick_drag");
        Vector2 pos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(this.transform as RectTransform, Input.mousePosition, this.cs.worldCamera, out pos);

        float len = pos.magnitude;
        if (len <= 0)
        {
            this.touch_dir = Vector2.zero;
            return;
        }

        this.touch_dir.x = pos.x / len;
        this.touch_dir.y = pos.y / len;

        if (len >= this.max_R)
        {
            //max_R / len = x` /x = y` / y
            float rate = this.max_R / len;
            pos.x = pos.x * rate;
            pos.y = pos.y * rate;
        }

        this.stick.localPosition = pos;
    }

    public void on_stick_end_drag()
    {
        //Debug.Log("on_stick_end_drag");
        this.stick.localPosition = Vector2.zero;
        this.touch_dir = Vector2.zero;
    }
}
