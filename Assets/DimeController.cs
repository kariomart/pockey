using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DimeController : MonoBehaviour
{

    public int index;
    public bool on;
    public int value;       
    SpriteRenderer spr;
    Color defColor;
    public Color onColor;
    public TextMeshPro valueText;

    int framesSinceOff;
    public int resetTimer;

    public AudioClip sfx_dime;


    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        defColor = spr.color;
        valueText = GetComponentInChildren<TextMeshPro>();
        valueText.text = "" + value;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (on) {
            framesSinceOff ++;
        }

        if (framesSinceOff > resetTimer) {
            TurnOff();
        }
        
    }

    public void TurnOn() {
        on = true;
        spr.color = onColor;
        framesSinceOff = 0;
        SoundController.me.PlaySound(sfx_dime, .8f);
    }

    public void TurnOff() {
        on = false;
        spr.color = defColor;
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (!on && coll.gameObject.tag == "Player") {
            coll.GetComponentInParent<PlayerController>().points++;
            Master.me.UpdateUI();
            TurnOn();
        } 
    }

    
}
