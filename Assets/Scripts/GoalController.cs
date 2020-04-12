using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour
{

    public int playerId;
    public SpriteRenderer sprite;
    public string name;
    public GameObject defender;
    public Transform puckSpawn;

    // Start is called before the first frame update
    void Start()
    {
        //sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeColor(Color c)
    {
        if (sprite)
        {
            sprite.color = c;
        }
    }

    public IEnumerator TurnOnDefender() {
        defender.SetActive(true);
        yield return new WaitForSeconds(15);
        defender.SetActive(false);
        Master.me.ResetBouncers();
    }

    public void EnableBlocker() {
        defender.SetActive(true);
    }

    public void DisableBlocker() {
        defender.SetActive(false);
    }

    
}
