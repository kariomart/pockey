using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour
{

    public int playerId;
    private Player rewiredPlayer;

    Rigidbody2D rb;
    public SpriteRenderer spr;
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
    public float dashSpeed;

    public bool hasPuck;
    PuckController puck;
    public bool shooting;
    bool justShot;
    int framesSinceShot = 11;
    int shotTimer = 10;
    public float shootSpd;
    public float maxShootSpd;
    float framesSinceDash;
    public float dashTimer;

    public int livePoints;
    public int points;

    public AudioClip sfx_shoot;

    public GoalieController goal;



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

        if (rewiredPlayer.GetButtonDown("Restart")) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        lStickDir = new Vector2(rewiredPlayer.GetAxis("Move Horizontal"), rewiredPlayer.GetAxis("Move Vertical"));

        if (rewiredPlayer.GetButton("Swing")) {
            if (hasPuck) {
                shooting = true;
                shootSpd += maxShootSpd/80f;
                shootSpd = Mathf.Clamp(shootSpd, 0, maxShootSpd);
            }
        }

        if (rewiredPlayer.GetButtonDown("Swing") && !hasPuck) {
            Dash();
        }   


        if (rewiredPlayer.GetButtonUp("Swing") && hasPuck && shooting) {
            if (hasPuck) {
                ShootPuck();
            }
        }

        if (rewiredPlayer.GetButtonDown("Call Puck")) {
            PuckController p = Master.me.puck;
            p.Control(transform.GetChild(0).transform.GetChild(0).transform.GetChild(0));
            ControlPuck(p);
        }

        if (rewiredPlayer.GetButtonDown("LeftFlap")) {
            Master.me.leftFlap.StartFlap();
        } 

        if (rewiredPlayer.GetButtonDown("RightFlap")) {
            Master.me.rightFlap.StartFlap();
        }
    }

    void FixedUpdate() {

        if (justShot) {
            framesSinceShot ++;
        }
        framesSinceDash ++;

        MoveGoalie();

        accel += lStickDir.normalized * jerk;
        accel = Vector2.ClampMagnitude(accel, maxAccel);
        if (lStickDir == Vector2.zero) {
            accel *= damping;
            vel *= damping;
        } else {
            //transform.localEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Geo.ToAng(lStickDir) - 90);
            transform.localEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Geo.ToAng(accel) - 90);
        }

        if (shooting) {
            accel *= .89f;
            vel *= .89f;
        }

        vel += accel;
        vel = Vector2.ClampMagnitude(vel, maxSpeed);
        //Debug.Log(vel);


        rb.MovePosition((Vector2)transform.position + vel);
    }

    void Dash() {
        if (framesSinceDash > dashTimer) {
            Vector2 dir = lStickDir.normalized;
            float mag = vel.magnitude;
            //Debug.Log(mag);
            dir *= dashSpeed;
            vel *= .5f;
            vel += dir*(mag*10);
            framesSinceDash = 0;
        }
    }

    void ShootPuck() {
        if (puck) {
            SoundController.me.PlaySoundAtNormalPitch(sfx_shoot, 1f, transform.position.x);
            shooting = false;
            Vector2 dir = Geo.ToVect(transform.localEulerAngles.z + 90f);
            puck.Shoot(vel.magnitude + shootSpd, dir);
            shootSpd = 0;
            hasPuck = false;
            puck = null;
            justShot = true;
            framesSinceShot = 0;
            Master.me.playerLastTouched = this;
        }
    }

    public void ControlPuck(PuckController p) {
        if (framesSinceShot > shotTimer) {
            p.playerControllingPuck = this;
            hasPuck = true;
            puck = p;
            justShot = false;
        }
    }

    void MoveGoalie() {
        float y = lStickDir.y;
        float goalSpd = .1f;

        float desPos = goal.transform.position.y + goalSpd*y;
        if (desPos > Master.me.goalPos[playerId].y + .5f*Master.me.goalSize) {
            desPos = Master.me.goalPos[playerId].y + .5f*Master.me.goalSize;       
        } else if (desPos < Master.me.goalPos[playerId].y - .5f*Master.me.goalSize) {
            desPos = Master.me.goalPos[playerId].y - .5f*Master.me.goalSize;       
        }
        goal.transform.position = new Vector2(goal.transform.position.x, desPos);
    }

    void OnCollisionEnter2D(Collision2D coll) {

        if (coll.gameObject.tag == "Wall") {
            vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (vel.magnitude * 0.5f);
        }

        if (coll.gameObject.tag == "Player") {
            PlayerController p = coll.gameObject.GetComponent<PlayerController>();
            if (puck && p.vel.magnitude > 0.1f) {
                hasPuck = false;
                puck.DropPuck(p.vel);
                puck = null;
            }

            //vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (p.vel.magnitude*5);
            Vector2 collDir = transform.position - coll.transform.position;
            vel = collDir * p.vel.magnitude;

        }

        if (coll.gameObject.tag == "Bumper") {
            vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (vel.magnitude * Random.Range(.9f, 1.3f));
        }

        if (coll.gameObject.tag == "Puck") {
            PuckController p = coll.gameObject.GetComponent<PuckController>();
            Vector2 dir = (Vector2)p.transform.position - coll.contacts[0].point;
            p.vel = dir;
            p.spd = accel.magnitude * 100;
            Master.me.playerLastTouched = this;
            //Debug.Log("DIR: " + p.vel);
            //Debug.Log("SPEED: " + p.spd);
        }

        if (coll.gameObject.tag == "Flap") {
            FlapController f = coll.gameObject.GetComponent<FlapController>();
            if (!f.flapping) {
                vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (vel.magnitude * 0.5f);
            } else {
                vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (vel.magnitude);
                //accel*=2;
            }
        }

    }

    public void SetColor(Color c) {
        spr.color = c;
    }
}
