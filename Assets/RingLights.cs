using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RingLights : MonoBehaviour
{

    public float lifetime;
    public float spd;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, lifetime);
        spd += Random.Range(-.1f, .1f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localScale*=spd;
    }
}
