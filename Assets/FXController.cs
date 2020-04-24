using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXController : MonoBehaviour
{

    public int lifetime;
    
    void Start()
    {
        Destroy(this.gameObject, lifetime);
    }

}
