using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleController : MonoBehaviour
{

    GameObject[] holes;
    // Start is called before the first frame update
    void Start()
    {
        holes = GameObject.FindGameObjectsWithTag("Hole");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D coll) {

        if (coll.gameObject.tag == "Puck") { 
            GameObject h = this.gameObject;

            while (h == this.gameObject) {
                h = holes[Random.Range(0, holes.Length)];
            }

            coll.gameObject.transform.position = h.transform.position;
            h.GetComponent<HoleController>().TurnOff();
        }
    }

    public void TurnOff() {
        
    }

    
}
