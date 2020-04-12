using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckController : MonoBehaviour
{

    SpriteRenderer spr;
    Rigidbody2D rb;
    public Collider2D coll;
    ParticleSystem trail;

    public Vector2 vel;
    public float spd;
    public float baseSpd;
    public float maxSpd;
    public float damping;
    public bool controlled;

    int aftertouchFrames;

    Transform stick;
    public PlayerController playerControllingPuck;
    public PlayerController lastPlayerTouched;
    public PlayerController playerLastShot;
    public int collTime;
    public int collTimer = 60;

    public AudioClip sfx_bounce;

    public GameObject reticle;

    LineRenderer line;



    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollider2D>();
        trail = GetComponentInChildren<ParticleSystem>();
        line = GetComponentInChildren<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {

        aftertouchFrames++;

        if (aftertouchFrames < 8 && playerLastShot) {
            //vel += playerLastShot.lStickDir*.5f;
        }
        //vel.Normalize();
        collTime ++;

       
    }

    void LateUpdate() {

         if (!controlled) {
            spd *= damping;
            if (spd > maxSpd)
            {
                spd = maxSpd;
            }
            rb.MovePosition((Vector2)transform.position + vel * spd);
            rb.velocity = Vector2.zero;
        
        } else {
            transform.position = stick.position;
        }
    }

    public void Shoot(float inheritedVel, Vector2 dir) {
        aftertouchFrames = 0;
        controlled = false;
        playerLastShot = playerControllingPuck;
        playerControllingPuck = null;
        vel = dir;
        spd = baseSpd + inheritedVel;
        trail.Play();
        coll.enabled = true;
        spr.enabled = true;
        collTime = 0;
        UpdatePuckColor();
    }

    public void Control(Transform stick, PlayerController p) {
        Debug.Log(p);
        lastPlayerTouched = p;
        this.stick = stick;
        controlled = true;
        UpdatePuckColor();
        coll.enabled = false;
        spr.enabled = false;
        trail.Stop();
    }

    public void DropPuck(Vector2 dir) {
        spd = vel.magnitude;
        vel = dir;
        controlled = false;
        playerControllingPuck = null;
        coll.enabled = true;
        spr.enabled = true;
        UpdatePuckColor();
    }


    void OnCollisionEnter2D(Collision2D coll) {

        if (coll.gameObject.tag == "Wall") {
            if (!controlled) {
                PlayBounceSound();
            }
            vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (vel.magnitude * 0.8f);
            Master.me.SpawnParticle(Master.me.collisionParticle, coll.contacts[0].point);
        }

        else if (coll.gameObject.tag == "Player") {
            PlayerController p = coll.gameObject.GetComponent<PlayerController>();
            if (playerLastShot != p || collTime > collTimer) {
                //vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (vel.magnitude * 0.8f);
                //lastPlayerTouched = coll.gameObject.GetComponent<PlayerController>();
                UpdatePuckColor();
            }
        }

        else if (coll.gameObject.tag == "Flap") {
            PlayBounceSound();
        }

        else if (coll.gameObject.tag == "Bumper" && !controlled) {
            vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal);
            spd *= Random.Range(1f, 2f);
            BumperController b = coll.gameObject.GetComponent<BumperController>();
            Master.me.SpawnParticle(Master.me.collisionParticle, coll.contacts[0].point);
            if (b) {
                b.Hit(this);
            }
        }

        else if (coll.gameObject.tag == "Bouncer" && !controlled) {
            vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal);
            spd *= Random.Range(1f, 2f);
            BouncerController b = coll.gameObject.GetComponent<BouncerController>();
            if (lastPlayerTouched) {
                b.Hit(lastPlayerTouched.playerId);
            } else {
                b.Hit();
            }
        }

        else if (coll.gameObject.tag == "GoalChanger" && !controlled) {
            vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal);
            spd *= Random.Range(1f, 2f);
            GoalChangeController b = coll.gameObject.GetComponent<GoalChangeController>();
            if (lastPlayerTouched) {
                b.Hit(lastPlayerTouched.playerId);
            } else {
                b.Hit();
            }
        }

        else if (coll.gameObject.tag == "Slingshot") {
            vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (vel.magnitude * Random.Range(1.5f,2f));
            Master.me.SpawnParticle(Master.me.collisionParticle, coll.contacts[0].point);
        }

    }

    public void UpdatePuckColor() {
        if (lastPlayerTouched) {
            ChangeColor(Master.me.playerColors[lastPlayerTouched.playerId]);
        } else {
            ChangeColor(Color.white);
        }
    }

    public void ChangeColor(Color c) {
        spr.color = c;
    }

    public void UpdateLine() {
        line.SetPosition(0, reticle.transform.position);
        line.SetPosition(1, transform.position);
    }

    void OnTriggerEnter2D(Collider2D coll) {

        if (coll.gameObject.tag == "Goal") {
            GoalController g = coll.GetComponent<GoalController>();
            Debug.Log("scored!");
            Master.me.GoalScored(this, g);
            Master.me.players[0].hasPuck = false;
            Master.me.players[1].hasPuck = false;
            Destroy(this.gameObject);
            trail.Stop();
        }

        if (coll.gameObject.tag == "Warp" && !controlled) {
            WarpController w = coll.gameObject.GetComponent<WarpController>();
            transform.position = w.otherWarp.transform.position + w.otherWarp.dir*1f;
            //vel.y*=-1;
        }

        if (coll.gameObject.tag == "ZoomGate") {
            //spd*=Random.Range(1.5f, 2.5f);
            spd*=2;
        }

    }

    void PlayBounceSound() {
        //SoundController.me.PlaySoundAtNormalPitch(sfx_bounce, 1f, transform.position.x);
        SoundController.me.PlayRandomSound(SoundController.me.sfx_wallHits);
    }

}
