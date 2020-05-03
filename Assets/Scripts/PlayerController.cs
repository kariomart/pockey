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
    public Color color;

    public Vector2 lStickDir;
    Vector2 lastStickDir;
    Vector2 vel;
    public float accel;
    public float deaccel;
    public float speed;
    public float maxSpeed;
    public float damping;
    public float dashSpeed;

    public bool hasPuck;
    public bool flicking;
    public bool startedFlick;
    bool flicked;
    Vector2 flickDir = Vector2.zero;
    public float flickSpd;
    public PuckController puck;
    public bool shooting;
    int framesSinceShot = 11;
    int shotTimer = 15;
    public float shootSpd;
    public float maxShootSpd;
    float framesSinceDash;
    public float dashTimer;

    public int points;

    public AudioClip sfx_shoot;
    public AudioClip sfx_orbitShoot;
    public AudioSource skateAudio;

    public GameObject vfx_ice;

    public GoalieController goal;
    public GameObject reticle;
    public GameObject head;

    public FlapController leftFlap;
    public FlapController rightFlap;

    public CircleCollider2D hitbox;

    public GameObject FX_ShotPuck;
    public GameObject FX_MuzzleFlash;

    public List<PuckController> orbittingPucks = new List<PuckController>();


    // Start is called before the first frame update
    public void Start()
    {
        rewiredPlayer = ReInput.players.GetPlayer(playerId);
        rb = GetComponent<Rigidbody2D>();
        spr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        SetColor(Master.me.playerColors[playerId]);
        coll = transform.GetChild(0).GetComponent<CircleCollider2D>();
        skateAudio = GetComponentInChildren<AudioSource>();
        SpawnOrbitPuck();

        // PlayerTuning tuning = Resources.Load<PlayerTuning>("MyTune");
        // jerk = tuning.jerk;
        // deaccel = tuning.deaccel;
        // speed = tuning.speed;
        // maxAccel = tuning.maxAccel;
        // maxSpeed = tuning.maxSpeed;
        // damping = tuning.damping;
        
    }

    // Update is called once per frame
    void Update()
    {

        if (rewiredPlayer.GetButtonDown("Restart")) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            Time.timeScale = 1;
        }
        lStickDir = new Vector2(rewiredPlayer.GetAxis("Move Horizontal"), rewiredPlayer.GetAxis("Move Vertical"));

        if (rewiredPlayer.GetButton("Swing")) {
            if (!shooting) {
                shootSpd+=.5f;
            }
            if (hasPuck) {
                shooting = true;
                shootSpd += maxShootSpd/80f;
                shootSpd = Mathf.Clamp(shootSpd, 0, maxShootSpd);
            }

            if (flicking) {
                flicked = true;
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

        if (rewiredPlayer.GetButtonDown("ReleasePuck")) {
            ReleasePuck();
        }

        if (rewiredPlayer.GetButtonUp("Select")) {
            //PauseGame();
            //bring up menu
        }

        if (rewiredPlayer.GetButtonDown("Call Puck")) {
            // PuckController p = Master.me.puck;
            // p.Control(transform.GetChild(0).transform.GetChild(0).transform.GetChild(0));
            // p.playerControllingPuck.DropPuck();
            // ControlPuck(p);
        }

        // if (rewiredPlayer.GetButton("LeftFlap")) {
        //     Debug.Log("left flap!");
        //     leftFlap.flapping = true;
        // } else {
        //     leftFlap.flapping = false;
        // }

        // if (rewiredPlayer.GetButton("RightFlap")) {
        //     Debug.Log("right flap!");
        //     rightFlap.flapping = true;
        // } else {
        //     rightFlap.flapping = false;
        // }

        if (rewiredPlayer.GetButtonDown("LeftFlap")) {
            leftFlap.StartFlap();
        }
        if (rewiredPlayer.GetButtonDown("RightFlap")) {
            rightFlap.StartFlap();
        }
    }

    void FixedUpdate() {


        framesSinceShot ++;
        framesSinceDash ++;
        SkateAudio();
        FlickPuck();

        float dot = Vector2.Dot(lStickDir.normalized, lastStickDir);
        if (dot < .97f && dot != 0) {
            Instantiate(vfx_ice, transform.position, Quaternion.identity);
        }

        Vector2 moveDir = lStickDir.normalized; 
        
        //reticle.transform.position = Vector2.Lerp(transform.position, (Vector2)transform.position + moveDir*1.25f, 1 - speed / maxSpeed);

        if (moveDir != Vector2.zero) {
            speed+=accel;
            lastStickDir = moveDir;
            reticle.transform.position = (Vector2)transform.position+moveDir*(transform.lossyScale.x * .8f);
            head.transform.position = (Vector2)transform.position+moveDir*.9f;
            //transform.localEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Geo.ToAng(lStickDir) - 90);
            transform.localEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Geo.ToAng(vel) - 90);
        } else {
            speed -= deaccel;
            speed = Mathf.Clamp(speed, 0, maxSpeed);
            reticle.transform.position = (Vector2)transform.position + lastStickDir*(transform.lossyScale.x * .8f);
            head.transform.position = (Vector2)transform.position + lastStickDir*.9f;

        }


        if (speed>=maxSpeed) {
            speed = maxSpeed;
        }

        vel += moveDir*speed;  
        if (!shooting) {   
            vel *= damping;   
        } else {
            vel *= .85f;
        }

        if (!flicking) {
            rb.MovePosition((Vector2)transform.position + vel);
        }
        if (Master.me.goaliesEnabled) {
            MoveGoalie();
        }
    }

    void Dash() {
        shooting = false;
        if (framesSinceDash > dashTimer) {
            Vector2 dir = lStickDir.normalized;
            //Debug.Log(dir);
            vel+=dir*1.5f;
            framesSinceDash = 0;
            StartCoroutine(scaleHitbox());
        }
    }

    void SpawnOrbitPuck() {

        PuckController p = Instantiate(Master.me.puckObj, (Vector2)transform.position, /*+ sDir*/ Quaternion.identity).GetComponent<PuckController>();
        p.Start();
        AddToOrbit(p);

    }

    public void AddToOrbit(PuckController p) {
        p.UpdateAlpha(1-(.3f*orbittingPucks.Count));
        p.coll.enabled = false;
        orbittingPucks.Add(p);
        p.StartOrbit(this);
    }

    void ReleasePuck() {
        if (orbittingPucks.Count > 0) {
            PuckController p = orbittingPucks[0];
            orbittingPucks.Remove(p);
            SoundController.me.PlaySoundAtNormalPitch(sfx_orbitShoot, .5f);
            p.Shoot(p.transform.position - transform.position, Random.Range(1.5f,3f), true);
        }

        int a = 0;
        foreach(PuckController p in orbittingPucks) {
           p.UpdateAlpha(1-(.3f*a));
           a++;
        }
        
        //Debug.Log(orbittingPucks.Count);
    }

    void ShootPuck() {
        if (puck) {
            //SoundController.me.PlaySoundAtNormalPitch(sfx_shoot, 1f, transform.position.x);
            SoundController.me.PlayRandomSound(SoundController.me.sfx_shots, .5f);
            shooting = false;
            Vector2 dir;
            if (lStickDir == Vector2.zero) {
                dir = Geo.ToVect(transform.localEulerAngles.z + 90f);
            } else {
                dir = lastStickDir;
            }
            puck.Shoot(dir + vel, shootSpd);
            shootSpd = 0;
            hasPuck = false;
            framesSinceShot = 0;
            puck.lastPlayerTouched = this;
            puck.playerLastShot = this;
            puck = null;
            reticle.SetActive(false);
            SetColor(new Color(spr.color.r, spr.color.g, spr.color.b, 1f));
            //float rotDir = Geo.ToAng(dir);
            float ang = Geo.ToAng(dir) + 180;
            Transform t = Instantiate(FX_ShotPuck, (Vector2)transform.position + (dir * 4f), Quaternion.Euler(new Vector3(360 - ang, 90, 0))).transform;
            ParticleSystem.MainModule p = t.gameObject.GetComponent<ParticleSystem>().main;
            p.startColor = new ParticleSystem.MinMaxGradient(Color.red, color);

            Transform t1 = Instantiate(FX_MuzzleFlash, (Vector2)transform.position + (dir * 2f), Quaternion.Euler(new Vector3(360 - ang, 90, 0))).transform;
            //t.localEulerAngles = new Vector3(rotDir-180,0,0);

        }
    }

    public void ControlPuck(PuckController p) {
        if (framesSinceShot > shotTimer) {
            p.playerControllingPuck = this;
            hasPuck = true;
            puck = p;
            reticle.SetActive(true);
        }
    }

    public void DropPuck() {
        hasPuck = false;
        puck.controlled = false;
        puck.lastPlayerTouched = this;
        puck = null;
    }

    public bool CanShoot() {
        return framesSinceShot > shotTimer;
    }

    public void FlickPuck() {
        if (flicking) {
            Vector2 pos = -lStickDir * Mathf.Lerp(0, flickSpd, lastStickDir.magnitude);
            Master.me.puck.reticle.transform.position = Vector2.MoveTowards(Master.me.puck.reticle.transform.position, (Vector2)Master.me.puck.transform.position + pos, .7f);
            Master.me.puck.UpdateLine();

            if (lStickDir.magnitude > flickDir.magnitude) {
                flickDir = lStickDir;
            }

            if (lStickDir != Vector2.zero) {
                startedFlick = true;
            }

            if (startedFlick && lStickDir == Vector2.zero) {
                Master.me.puck.reticle.SetActive(false);
                coll.enabled = true;
                SoundController.me.PlayRandomSound(SoundController.me.sfx_shots);
                flicked = false;
                flicking = false;
                startedFlick = false;
                Vector2 dir;
                dir = -flickDir;
                float fSpeed = Mathf.Lerp(0, flickSpd, flickDir.magnitude);
                Master.me.puck.Shoot(dir, fSpeed);
                Master.me.puck.coll.enabled = true;
                flickDir = Vector2.zero;
            }
        }
    }

    void MoveGoalie() {
        float y = lStickDir.y;
        float goalSpd = .25f;

        float desPos = goal.transform.position.y + goalSpd*y;
        if (desPos > Master.me.goalPos[playerId].y + .5f*Master.me.goalSize) {
            desPos = Master.me.goalPos[playerId].y + .5f*Master.me.goalSize;       
        } else if (desPos < Master.me.goalPos[playerId].y - .5f*Master.me.goalSize) {
            desPos = Master.me.goalPos[playerId].y - .5f*Master.me.goalSize;       
        }
        goal.transform.position = new Vector2(goal.transform.position.x, desPos);
    }

    void SkateAudio() {
        float min = 0f;
        float max = .4f;
        float vol = Mathf.Lerp(min, max, speed/maxSpeed);
        float pan = 
        skateAudio.volume = vol;
    }

    void OnCollisionEnter2D(Collision2D coll) {

        if (coll.gameObject.tag == "Wall") {
            vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (vel.magnitude * 0.5f);
        }

        else if (coll.gameObject.tag == "Player") {
            PlayerController p = coll.gameObject.GetComponent<PlayerController>();
            if (puck && p.vel.magnitude > 0.08f) {
                framesSinceShot = 0;
                Debug.Log("smashed");
                puck.DropPuck(p.vel);
                DropPuck();
            }

            //vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (p.vel.magnitude*5);
            Vector2 collDir = transform.position - coll.transform.position;
            if (!Master.me.collided) {
                Master.me.collided = true;
                //vel = collDir * p.vel.magnitude;
                Debug.Log(p.speed);
                p.vel = (coll.transform.position - transform.position).normalized;
                p.speed += .85f * speed;
                vel = collDir * p.speed * 4f;
            } else
            {
                Master.me.collided = false;
            }

            //Master.me.shake.SetScreenshake(p.vel.magnitude*6, .25f, collDir);

        }

        else if (coll.gameObject.tag == "Bumper" || coll.gameObject.tag == "Bouncer") {
            vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * Random.Range(1f, 1.25f);
        }

        else if (coll.gameObject.tag == "Puck") {
            PuckController p = coll.gameObject.GetComponent<PuckController>();
            p.lastPlayerTouched = this;
            p.UpdatePuckColor();
            //Vector2 dir = (Vector2)p.transform.position - coll.contacts[0].point;
            //p.vel = dir;
            //Debug.Log("DIR: " + p.vel);
            //Debug.Log("SPEED: " + p.spd);
        }

        else if (coll.gameObject.tag == "Flap") {
            FlapController f = coll.gameObject.GetComponent<FlapController>();
            if (!f.flapping) {
                vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (vel.magnitude * 0.5f);
            } else {
                vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (vel.magnitude);
                //accel*=2;
            }
        }

    }

    public void OnTriggerEnter2D(Collider2D coll) {
        if (coll.gameObject.tag == "ZoomGate") {
            //vel*=Random.Range(2f,4f);
        }
    }

    public void SetColor(Color c) {
        spr.color = c;
        color = c;
        head.GetComponent<SpriteRenderer>().color = new Color(c.r+.1f, c.g+.1f, c.b+.1f);
        reticle.GetComponent<SpriteRenderer>().color = Color.white;
    }

    IEnumerator scaleHitbox() {
        float defSize = 2;
        float maxSize = 4;
        float duration = 30;

        for(float i = 0; i<duration; i++) {
            float desSize = Mathf.Lerp(defSize, maxSize, i/duration);
            hitbox.radius = desSize;
            yield return new WaitForSeconds(0.01f);
        }

        for(float i = 0; i<duration; i++) {
            float desSize = Mathf.Lerp(maxSize, defSize, i/duration);
            hitbox.radius = desSize;
            yield return new WaitForSeconds(0.01f);
        }

    }
}
