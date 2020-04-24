using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BouncerController : MonoBehaviour
{

    SpriteRenderer sprite;
    Color defColor;
    public int controlledID;
    public Light2D hitLight;
    
    
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

    public void Hit() {
        
    }

    public void Hit(int id) {
        ChangeColor(Master.me.playerColors[id]);
        StartCoroutine(lightBurst(Master.me.playerColors[id]));
        controlledID = id;
        Master.me.CheckBouncers();
    }

    public void Reset() {
        ChangeColor(defColor);
        controlledID = -1;
    }

    public void ChangeColor(Color c) {
        sprite.color = c;
    }

    IEnumerator lightBurst(Color c) {
        float duration = 30f;
        float maxIntensity = 3f;
        hitLight.color = c;

        // for(float i = 0f; i<duration/3f; i++) {
        //     float desIntensity = Mathf.Lerp(0, maxIntensity, i/duration);
        //     hitLight.intensity = desIntensity;
        //     yield return new WaitForSeconds(.001f);
        // }
        hitLight.intensity = maxIntensity;

        for(float i = 0; i<duration; i++) {
            float desIntensity = Mathf.Lerp(maxIntensity, 0, i/duration);
            hitLight.intensity = desIntensity;
            yield return new WaitForSeconds(.001f);
        }
    }
}
