using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncerController : MonoBehaviour
{

    SpriteRenderer sprite;
    Color defColor;
    public int controlledID;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        defColor = sprite.color;
    }

    // Update is called once per frame
    void Update()
    {
   
    }

    public void Hit() {
        
    }

    public void Hit(int id) {
        ChangeColor(Master.me.playerColors[id]);
        controlledID = id;
        Master.me.CheckBouncers();
    }

    public void Reset() {
        ChangeColor(defColor);
        controlledID = -1;
    }

    public void ChangeColor(Color c) {
        sprite.color = c;
    }
}
