using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour
{

    public int playerId;
    private Player rewiredPlayer;

    Rigidbody2D rb;
    SpriteRenderer spr;
    Collider2D coll;

    Vector2 lStickDir;
    Vector2 vel;
    public Vector2 accel;
    public float jerk;
    public float deaccel;
    public float speed;
    public float maxAccel;
    public float maxSpeed;
    public float damping;

    public bool hasPuck;
    PuckController puck;

    // Start is called before the first frame update
    void Start()
    {
        rewiredPlayer = ReInput.players.GetPlayer(playerId);
        rb = GetComponent<Rigidbody2D>();
        spr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        SetColor(Master.me.playerColors[playerId]);
        coll = transform.GetChild(0).GetComponent<CircleCollider2D>();

        PlayerTuning tuning = Resources.Load<PlayerTuning>("MyTune");
        jerk = tuning.jerk;
        deaccel = tuning.deaccel;
        speed = tuning.speed;
        maxAccel = tuning.maxAccel;
        maxSpeed = tuning.maxSpeed;
        damping = tuning.damping;
        
    }

    // Update is called once per frame
    void Update()
    {
        lStickDir = new Vector2(rewiredPlayer.GetAxis("Move Horizontal"), rewiredPlayer.GetAxis("Move Vertical"));
        if (rewiredPlayer.GetButtonDown("Swing")) {
            Debug.Log("trying to shoot");
            if (hasPuck) {
                ShootPuck();
            }
        }

        if (rewiredPlayer.GetButtonDown("Call Puck")) {
            PuckController p = Master.me.puck;
            p.Control(transform.GetChild(0).transform.GetChild(0).transform.GetChild(0));
            ControlPuck(p);
        }
    }

    void FixedUpdate() {

        //Vector2 desAng = lStickDir;
        //transform.localEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.MoveTowards(transform.eulerAngles.z, Geo.ToAng(lStickDir), 10f));
        accel += lStickDir.normalized * jerk;
        accel = Vector2.ClampMagnitude(accel, maxAccel);
        if (lStickDir == Vector2.zero) {
            accel *= damping;
            vel *= damping;
        } else {
            //transform.localEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Geo.ToAng(lStickDir) - 90);
            transform.localEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Geo.ToAng(accel) - 90);
        }

        vel += accel;
        vel = Vector2.ClampMagnitude(vel, maxSpeed);
        //Debug.Log(vel);


        rb.MovePosition((Vector2)transform.position + vel);
    }

    void ShootPuck() {
        if (puck) {
            Vector2 dir = Geo.ToVect(transform.localEulerAngles.z + 90f);
            puck.Shoot(vel.magnitude, dir);
            hasPuck = false;
            puck = null;
        }
    }

    public void ControlPuck(PuckController p) {
        p.playerControllingPuck = this;
        hasPuck = true;
        puck = p;
    }

    void OnCollisionEnter2D(Collision2D coll) {

        if (coll.gameObject.tag == "Wall") {
            vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (vel.magnitude * 0.5f);
        }

        if (coll.gameObject.tag == "Puck") {
            PuckController p = coll.gameObject.GetComponent<PuckController>();
            Vector2 dir = (Vector2)p.transform.position - coll.contacts[0].point;
            p.vel = dir;
            p.spd = accel.magnitude * 100;
            //Debug.Log("DIR: " + p.vel);
            //Debug.Log("SPEED: " + p.spd);
        }

    }

    public void SetColor(Color c) {
        spr.color = c;
    }
}
