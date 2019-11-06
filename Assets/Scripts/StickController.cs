using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickController : MonoBehaviour
{

    PlayerController player;
    Transform endOfStick;
    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.transform.GetComponentInParent<PlayerController>();
        endOfStick = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D coll) {
        
        if (coll.gameObject.tag == "Puck" && !player.hasPuck) {
            Debug.Log("puck hit by end of stick !");
            PuckController p = coll.gameObject.GetComponent<PuckController>();
            p.Control(endOfStick.transform);
            if (p.playerControllingPuck) {
                p.playerControllingPuck.hasPuck = false;
            }
            p.playerControllingPuck = player;
            player.ControlPuck(p);

        }


    }
}
