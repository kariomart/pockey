using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Master : MonoBehaviour
{

    public static Master me;
    public GameObject playerPrefab;
    public GameObject goalieObj;
    public GameObject puckObj;
    public FlapController leftFlap;
    public FlapController rightFlap;
    public bool goaliesEnabled;
    public bool collided;

    public int numPlayers;
    public PlayerController[] players;
    public Color[] playerColors = new Color[4];
    public Vector2[] spawnPos = new Vector2[2];
    public Vector2[] goalPos = new Vector2[2];
    public float goalSize;
    public PuckController puck;
    public Transform puckSpawn;

    public int livePoints;
    public int pointsToWin;
    public TextMeshPro livePointsUI;
    public TextMeshPro team1PointsUI;
    public TextMeshPro team2PointsUI;
    public Image middleCircle;
    public SpriteRenderer bg;

    public GameObject collisionParticle;
    public Screenshake shake;

    public AudioClip sfx_goal;
    public GameObject coinFX;


    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        players = new PlayerController[numPlayers];
        SpawnPlayers();
        SpawnPuck();
        livePoints = 5;
        shake = Camera.main.GetComponent<Screenshake>();
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space)) {
            //UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }
        
    }

    void FixedUpdate() {


    }


    public void GoalScored(PuckController p, GoalController g) {
        PlayerController player = p.lastPlayerTouched;

        if (g.playerId == 3)
        {
            player.points += livePoints;
        } else if (player.playerId != g.playerId) {
            player.points += livePoints;
        } else {

        }

        livePoints = 5;
        StartCoroutine(FlashBackground(playerColors[player.playerId]));
        SoundController.me.PlaySound(sfx_goal, 1f);
        UpdateUI();
        SpawnPuck();
    }

    public void AddPoints(int points) {
        livePoints += points;
        UpdateUI();
    }
    
    public void AddPoints(int points, Vector2 pos) {
        livePoints += points;
        UpdateUI();
        //StartCoroutine(ShootCoinFX(points, pos));
    }

    IEnumerator ShootCoinFX(int amt, Vector2 pos) {

        for (int i = 0; i < amt; i++)
        {
            CoinFX coin = Instantiate(coinFX, pos, Quaternion.identity).GetComponent<CoinFX>();   
            coin.destination = Vector2.zero;
            yield return new WaitForSeconds(.1f);
        }
    }

    IEnumerator FlashBackground(Color color) {
        Color defColor = bg.color;
        bg.color = color;
        float time = 100f;

        for (int i=0;i<=time;i++) {
            Color desColor = Color.Lerp(color, defColor, i/time);
            bg.color = desColor;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public IEnumerator HitPos(float t = 0.01f) {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(t);
        Time.timeScale = 1;
    }



    public void SpawnParticle(GameObject p, Vector2 pos) {
        Instantiate(p, pos, Quaternion.identity);
    }

    public void UpdateUI() {
        if (livePoints == 0) {
            livePointsUI.enabled = false;
        } else {
            livePointsUI.enabled = true;
        }
        livePointsUI.text = "" + livePoints;
        team1PointsUI.text = "" + players[0].points;
        team2PointsUI.text = "" + players[1].points;
    }

    PlayerController GetWinningPlayer() {
        PlayerController ply = null;
        int s = 0;
        for (int i = 0; i < numPlayers; i++)
        {
            PlayerController p = players[i];
            if (p.points > s) {
                ply = p;
                s = p.points;
            }
        }

        return ply;
    }

    void SpawnPuck() {

        puck = Instantiate(puckObj, new Vector2(0, 10), Quaternion.identity).GetComponent<PuckController>();
        StartCoroutine(PuckInvuln());
        Camera.main.GetComponent<CameraController>().t3 = puck.transform;

    }

    void SpawnPlayers() {

        for (int i = 0; i < players.Length; i++)
        {
            GameObject playerObj = Instantiate(playerPrefab, spawnPos[i], Quaternion.identity);
            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            playerController.playerId = i;
            players[i] = playerController;

            if (goaliesEnabled)
            {
                GoalieController g = Instantiate(goalieObj, goalPos[i], Quaternion.identity).GetComponent<GoalieController>();
                g.gameObject.GetComponent<SpriteRenderer>().color = playerColors[i];
                g.transform.eulerAngles = new Vector3(0, 0, -90f);
                playerController.goal = g;
            }

        }

    }

    IEnumerator PuckInvuln() {

        float maxScale = 5;
        float defScale = puck.transform.localScale.x;
        Collider2D coll = puck.GetComponent<Collider2D>();
        coll.enabled = false;
        puck.transform.localScale = new Vector3(maxScale, maxScale, maxScale);
        float len = 100f;
        for (int i = 0; i < len; i++)
        {
            float desScale = Mathf.Lerp(puck.transform.localScale.x, defScale, i/len);
            puck.transform.localScale = new Vector3(desScale, desScale, desScale);
            yield return new WaitForSeconds(.01f);        
        }

        coll.enabled = true;

    }

    void Initialize() {
        if (me == null) {
		    me = this;	
		} else {
			Destroy (this.gameObject);
		}

       // GameObject.Find("Canvas").GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
