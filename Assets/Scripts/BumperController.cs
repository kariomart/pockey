using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BumperController : MonoBehaviour
{
    SpriteRenderer sprite;
    public SpriteRenderer middleSprite;
    Color defColor;

    public bool flapBumper;
    public GameObject flapPivot;
    public float desAngle;
    public float speed;

    public PlayerController controller;

    public AudioClip sfx_bumperHit;
    FlapController flapper;
    public GameObject fx_hitparticle;
    public GameObject fx_particles;
    public SpriteRenderer ringLight;
    public Light2D mainLight;    

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        defColor = sprite.color;
        FlapController flapper = GetComponentInChildren<FlapController>();

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

    public void Hit(PuckController p) {
       // if (!controller && p.lastPlayerTouched) {
            controller = p.lastPlayerTouched;
        //} else {
          //  if (Random.value > .098f) {
            UpdateColor();
          //  }
       // }

        if (flapBumper) {
            desAngle = Random.Range(20, 100);
        }

        Master.me.shake.SetScreenshake(.5f, .4f);
        //Instantiate(fx_particles, transform.position, Quaternion.identity);
        int pts = Random.Range(2, 5);

        if (p.lastPlayerTouched) {
            //p.lastPlayerTouched.points += pts;
            StartCoroutine(BumperAnimation(pts, p.lastPlayerTouched.color));
            StartCoroutine(Master.me.AddPoints(pts,  p.lastPlayerTouched.playerId));
        } else {
            Reset();
        }
        StartCoroutine(Master.me.AddPoints(pts));
        StartCoroutine("ColorBlast");
        // GameObject g = Instantiate(fx_hitparticle, transform.position, Quaternion.identity);
        // ParticleSystem ps = g.GetComponent<ParticleSystem>();
        // var col = ps.colorOverLifetime;
        // Gradient grad = new Gradient();
        // Color c = Master.me.playerColors[controller.playerId];
    
        // grad.SetKeys( new GradientColorKey[] { new GradientColorKey(c, 0.0f), new GradientColorKey(c, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) } );
        // col.color = grad;

    }

    public void Reset() {
        controller = null;
        UpdateColor();
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

        UpdateColor();
    }

    IEnumerator BumperAnimation(int n, Color c) {
        for (int i = 0; i < n; i++)
        {
            SoundController.me.PlaySoundAtPitch(sfx_bumperHit, .2f, Random.Range(0.8f, 1f) + (i*.1f));  
            //Light2D l = Instantiate(ringLight, transform.position, Quaternion.identity);
            //l.color = c;
            SpriteRenderer s = Instantiate(ringLight, transform.position, Quaternion.identity);
            s.color = c;

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void UpdateColor() {
        if (controller) {
            middleSprite.color = controller.color;;
            mainLight.color = controller.color;
        } else {
            middleSprite.color = Color.white;
            mainLight.color = Color.white;
        }
    }


 
}
