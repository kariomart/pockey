using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterController : MonoBehaviour
{

    DimeController[] dimes;
    public int scoreBonus;
    public int dimeValue;

    // Start is called before the first frame update
    void Start()
    {
        dimes = new DimeController[transform.childCount];
        int i = 0;
        foreach(Transform c in transform) {
            dimes[i] = c.GetComponent<DimeController>();
            i++;
        }

        foreach(DimeController d in dimes) {
            d.value = dimeValue;
            d.valueText.text = "" + dimeValue;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
