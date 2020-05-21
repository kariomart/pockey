using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootController : MonoBehaviour
{

    public float destroy;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, destroy);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
