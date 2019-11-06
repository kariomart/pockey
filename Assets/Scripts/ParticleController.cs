using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{

    public float lifetime;
    // Start is called before the first frame update
    void Start()
    {
        if (lifetime == 0) {
            lifetime = 5f;
        }
        Destroy(this.gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
