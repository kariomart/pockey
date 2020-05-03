using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CursorController : MonoBehaviour
{
   GameObject selectedObject;
    
    // Start is called before the first frame update
    void Start()
    {
        selectedObject = EventSystem.current.currentSelectedGameObject;
    }

    // Update is called once per frame
    void Update()
    {
        selectedObject = EventSystem.current.currentSelectedGameObject;
        //transform.position = new Vector3(transform.position.x, selectedObject.transform.position.y, 0);
        if (selectedObject) {
            transform.localPosition = new Vector3(selectedObject.transform.localPosition.x+15, selectedObject.transform.localPosition.y, 0);
        }
    }
}
