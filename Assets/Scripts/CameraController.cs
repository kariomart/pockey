using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    Transform t1;
    Transform t2;
    public Transform t3;
    Camera cam;
    public int baseSize;
    public int maxSize;
    public Vector3 shakeChange;
    public float sizeFac;
    // Start is called before the first frame update
    void Start()
    {

        t1 = Master.me.players[0].transform;
        t2 = Master.me.players[1].transform;
        t3 = Master.me.puck.gameObject.transform;
        cam = Camera.main;
        cam.farClipPlane = 2000;
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // How many units should we keep from the players
    float zoomFactor = 5f;
    float followTimeDelta = 0.8f;
    Vector3 midpoint;
    midpoint = (t1.position + t2.position) / 2f;
    //midpoint = (t1.position + t2.position + t3.position) / 3f;

    // if (!Master.me.puck.controlled) {
    //     midpoint = (t1.position + t2.position + t3.position) / 3f;
    // } else {
    //     midpoint = (t1.position + t2.position) / 2f;
    // }
 
     // Distance between objects
    float distance = (t1.position - t2.position).magnitude;
    distance = Vector2.Distance(t1.position, t2.position);
    float puckDist = Master.me.GetLargestDist(midpoint);
    
     // Move camera a certain distance
     Vector3 cameraDestination = midpoint - cam.transform.forward * distance * zoomFactor;
     cameraDestination += shakeChange;
 
     // Adjust ortho size if we're using one of those
     if (cam.orthographic)
     {
         float newSize;
         // The camera's forward vector is irrelevant, only this size will matter
         //float newSize = baseSize + distance;
         if (puckDist > distance) {
            newSize = puckDist / sizeFac;
         } else {
            newSize = distance / sizeFac; 
         }

         newSize = Mathf.Clamp(newSize, baseSize, maxSize);
         cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, newSize, .1f*(newSize*.1f));
     }
     // You specified to use MoveTowards instead of Slerp
     cam.transform.position = Vector3.Slerp(cam.transform.position, cameraDestination, followTimeDelta);
         
     // Snap when close enough to prevent annoying slerp behavior
     if ((cameraDestination - cam.transform.position).magnitude <= 0.05f)
         cam.transform.position = cameraDestination;
        
    }
}
