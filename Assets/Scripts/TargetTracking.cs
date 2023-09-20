using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTracking : MonoBehaviour
{
    // Start is called before the first frame update
    private bool gotTarget = false;
    private Vector3 targetPosition;
    public Camera mainCam;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gotTarget == true)
        {
            transform.position = mainCam.ScreenToWorldPoint(targetPosition);
        }
    }
    public void Target(Vector3 newTarget)
    {
        //gameObject.SetActive(true);
        gotTarget = true;
        targetPosition = newTarget;
    }
}
