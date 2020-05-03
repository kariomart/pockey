using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickController : MonoBehaviour
{

    PlayerController player;
    Transform endOfStick;
    SpriteRenderer sprite;
    Color defColor;
    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.transform.GetComponentInParent<PlayerController>();
        endOfStick = transform.GetChild(0);
        sprite = GetComponent<SpriteRenderer>();
        defColor = sprite.color;
    }

    // Update is called once per frame
    void Update()
    {

        if (player.shooting) {
            Color tc = defColor;
            Color c = new Color(tc.r, tc.g, tc.b, .2f+player.shootSpd/player.maxShootSpd);
            sprite.color = c;
        } else {
            sprite.color = defColor;
        }
        
    }

    void OnTriggerEnter2D(Collider2D coll) {
        
        if (coll.gameObject.tag == "Puck") {
            PuckController p = coll.gameObject.GetComponent<PuckController>();
             if (player.puck == null ) {
                 if (player.CanShoot()) {
                    p.Control(endOfStick.transform, player);
                    p.playerControllingPuck = player;
                    player.ControlPuck(p);
                    p.coll.enabled = false;
                 }
             } else {
                if (!p.controlled) {
                    player.AddToOrbit(p);
                }
             }

            //Debug.Log(player.puck);

        }



    }
}
