using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperController : MonoBehaviour
{
    SpriteRenderer sprite;
    Color defColor;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        defColor = sprite.color;
    }

    // Update is called once per frame
    void Update()
    {
        
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


    }
}
