using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinFX : MonoBehaviour
{

    public Vector2 destination;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, destination, .3f);

        if (Vector2.Distance(transform.position, destination) < .1f) {
            Destroy(this.gameObject);
        }
    }
}
