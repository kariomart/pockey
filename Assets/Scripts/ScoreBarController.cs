using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBarController : MonoBehaviour
{

    float minScale = 0.01f;
    float maxScale = 9f;
    public int playerId;
    PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        player = Master.me.players[playerId];
    }

    // Update is called once per frame
    void Update()
    {
        float scale = Mathf.Lerp(minScale, maxScale, (float)player.points/(float)Master.me.pointsToWin);
        transform.localScale = new Vector3(scale, transform.localScale.y, transform.localScale.z);
    }
}
