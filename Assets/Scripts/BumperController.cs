using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperController : MonoBehaviour
{
    SpriteRenderer sprite;
    Color defColor;

    public bool flapBumper;
    public GameObject flapPivot;
    public float desAngle;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        defColor = sprite.color;

        if (flapBumper)
        {
            flapPivot = transform.GetChild(1).gameObject;
            desAngle = flapPivot.transform.localEulerAngles.z;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (flapBumper)
        {
            Vector3 ang = flapPivot.transform.localEulerAngles;
            flapPivot.transform.localEulerAngles = new Vector3(ang.x, ang.y, Mathf.MoveTowards(ang.z, desAngle, speed));
        }
    }

    public IEnumerator ColorBlast() {

        sprite.color = Color.white;
        float duration = 15f;

        for (float i = 0; i <= duration; i++)
        {
            Color c = Color.Lerp(Color.white, defColor, i/duration);
            sprite.color = c;
            yield return new WaitForSeconds(.01f);
        }
    }

 
}
