using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlapController : MonoBehaviour
{

    public float startAng;
    public float endAng;
    public float flapDuration;
    public bool flapping;
    Transform parent;

    SpriteRenderer sprite;
    Color defColor;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent;
        sprite = GetComponent<SpriteRenderer>();
        defColor = sprite.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            //StartCoroutine(Flap());
        }
        
    }

    public void StartFlap() {
        if (!flapping) {
            StartCoroutine(Flap());
        }
    }

    public IEnumerator Flap() {
        flapping = true;
        for (float i = 0; i < flapDuration; i++)
        {
            float newAng = Mathf.Lerp(startAng, endAng, i/flapDuration);
            //Debug.Log(newAng);
            parent.eulerAngles = new Vector3(parent.eulerAngles.x, parent.eulerAngles.y, newAng);
            yield return new WaitForSeconds(.001f);
        }

         for (float i = 0; i < flapDuration; i++)
        {
            float newAng = Mathf.Lerp(endAng, startAng, i/flapDuration);
            //Debug.Log(newAng);
            parent.eulerAngles = new Vector3(parent.eulerAngles.x, parent.eulerAngles.y, newAng);
            yield return new WaitForSeconds(.001f);
        }
        flapping = false;
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

    void OnCollisionEnter2D(Collision2D coll) {

        if (coll.gameObject.tag == "Puck") {
            StartCoroutine(ColorBlast());
            PuckController p = coll.gameObject.GetComponent<PuckController>();
            Vector2 dir = (Vector2)p.transform.position - coll.contacts[0].point;
            p.vel = dir;
            if (!flapping) {
                p.spd = p.spd*.9f;
            } else {
                //Debug.Log(p.spd);
                p.spd = p.spd*8f;
            }
        }

    }
}
