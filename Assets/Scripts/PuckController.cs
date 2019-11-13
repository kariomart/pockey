using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckController : MonoBehaviour
{

    SpriteRenderer spr;
    Rigidbody2D rb;
    Collider2D coll;
    ParticleSystem trail;

    public Vector2 vel;
    public float maxSpd;
    public float spd;
    public float damping;
    public bool controlled;

    Transform stick;
    public PlayerController playerControllingPuck;

    public AudioClip sfx_bounce;
    public AudioClip sfx_bumperHit;



    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollider2D>();
        trail = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {

        if (!controlled) {
            spd *= damping;
            rb.MovePosition((Vector2)transform.position + vel * spd);
        
        } else {
            transform.position = stick.position;
        }

    }

    public void Shoot(float inheritedVel, Vector2 dir) {
        controlled = false;
        playerControllingPuck = null;
        vel = dir;
        spd = inheritedVel + maxSpd;
        trail.Play();
    }

    public void Control(Transform stick) {
        this.stick = stick;
        controlled = true;
        trail.Stop();
    }

    public void DropPuck(Vector2 dir) {
        spd = vel.magnitude;
        vel = dir;
        controlled = false;
    }

    void OnCollisionEnter2D(Collision2D coll) {

        if (coll.gameObject.tag == "Wall") {
            PlayBounceSound();
            vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (vel.magnitude * 0.8f);
            Master.me.SpawnParticle(Master.me.collisionParticle, coll.contacts[0].point);
        }

        if (coll.gameObject.tag == "Flap") {
            PlayBounceSound();
        }

        if (coll.gameObject.tag == "Bumper" && !controlled) {
            vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (vel.magnitude * Random.Range(.9f, 1.3f));
            int pts = Random.Range(2, 5);
            StartCoroutine(PlayBumperSound(pts));
            Master.me.playerLastTouched.livePoints += pts;
            Master.me.livePoints += pts*2;
            Master.me.UpdateUI();
            coll.gameObject.GetComponent<BumperController>().StartCoroutine("ColorBlast");
            Master.me.SpawnParticle(Master.me.collisionParticle, coll.contacts[0].point);
        }

        if (coll.gameObject.tag == "Slingshot") {
            vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (vel.magnitude * Random.Range(1.5f,2f));
            Master.me.SpawnParticle(Master.me.collisionParticle, coll.contacts[0].point);
        }

    }

    void OnTriggerEnter2D(Collider2D coll) {

        if (coll.gameObject.tag == "Goal" && !controlled) {
            GoalController g = coll.GetComponent<GoalController>();
            Debug.Log("scored!");
            Master.me.GoalScored(g.playerId);
            trail.Stop();
        }

        if (coll.gameObject.tag == "Warp" && !controlled) {
            WarpController w = coll.gameObject.GetComponent<WarpController>();
            transform.position = w.otherWarp.transform.position + w.otherWarp.dir*1f;
            //vel.y*=-1;
        }

    }

    void PlayBounceSound() {
        SoundController.me.PlaySoundAtNormalPitch(sfx_bounce, 1f, transform.position.x);
    }

    IEnumerator PlayBumperSound(int n) {
        for (int i = 0; i < n; i++)
        {
            SoundController.me.PlaySoundAtPitch(sfx_bumperHit, .8f, Random.Range(0.8f, 1f) + (i*.1f));  
            yield return new WaitForSeconds(0.1f);
        }
    }
}
