using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanMask : MonoBehaviour
{

    public SpriteRenderer sprite;
    public float spd;
    public float maxSize;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.localScale.x <= maxSize) {
            transform.localScale = new Vector3(transform.localScale.x+spd, transform.localScale.y, transform.localScale.z);
        } else {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - Time.deltaTime);
            if (sprite.color.a <= 0) {
                Destroy(this.gameObject);
            }
        }

    }
}
