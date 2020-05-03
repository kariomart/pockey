using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerBarController : MonoBehaviour
{

    float time;
    float maxtime;
    Vector2 defScale;
    RectTransform rect;
    float maxX;
    // Start is called before the first frame update
    void Start()
    {
        time = Master.me.gameTime;
        maxtime = Master.me.periodLength;
        defScale = transform.localScale;
        rect = GetComponent<RectTransform>();
        maxX = rect.localScale.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Vector2 desScale = Vector2.Lerp(defScale, Vector2.zero, maxtime/time);
        float desXScale = Mathf.Lerp(maxX, 0, 1 - (Master.me.gameTime/Master.me.periodLength));
        //Debug.Log(Master.me.gameTime/Master.me.periodLength);
        rect.localScale = new Vector2(desXScale, rect.localScale.y);
        //rect.sizeDelta = new Vector2(desXScale, rect.sizeDelta.y);
        //transform.localScale = desScale;
    }
}
