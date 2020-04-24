using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerController : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D coll) {

        if (coll.gameObject.tag == "Puck") { 
            Debug.Log("flicking");
            int id = 0;
            if (Master.me.puck.lastPlayerTouched) {
                id = Master.me.puck.lastPlayerTouched.playerId;
            }

            Master.me.SpawnPuck(this.transform, id);
        }
    }

   
}
