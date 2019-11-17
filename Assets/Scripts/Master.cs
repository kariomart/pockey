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
    public TextMeshPro team1LivePointsUI;
    public TextMeshPro team2LivePointsUI;
    public Image middleCircle;

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
        puck = Instantiate(puckObj, Vector2.zero, Quaternion.identity).GetComponent<PuckController>();
        shake = Camera.main.GetComponent<Screenshake>();
        ResetPuck();
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


    public void GoalScored(PuckController p) {
        PlayerController player = p.lastPlayerTouched;
        int playerId = player.playerId;
        player.points += livePoints;


        SoundController.me.PlaySound(sfx_goal, 1f);
        UpdateUI();
        ResetPuck();
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

    public void ResetPuck() {

        puck.transform.position = puckSpawn.position;
        puck.spd = 0;

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

    void SpawnPlayers() {

        for (int i = 0; i < players.Length; i++)
        {
            GameObject playerObj = Instantiate(playerPrefab, spawnPos[i], Quaternion.identity);
            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            playerController.playerId = i;
            players[i] = playerController;

            GoalieController g = Instantiate(goalieObj, goalPos[i], Quaternion.identity).GetComponent<GoalieController>();
            g.gameObject.GetComponent<SpriteRenderer>().color = playerColors[i];
            g.transform.eulerAngles = new Vector3(0, 0, -90f);
            playerController.goal = g;

        }

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
