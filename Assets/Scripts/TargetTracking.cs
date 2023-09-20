using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTracking : MonoBehaviour
{
    // Start is called before the first frame update
    private bool gotTarget = false;
    private Vector3 targetPosition;
    private Camera mainCam;
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (gotTarget == true)
        {
            transform.position = mainCam.WorldToScreenPoint(targetPosition + new Vector3(0,3,0));
        }
    }
    public void Target(Vector3 newTarget)
    {
        //gameObject.SetActive(true);
        gotTarget = true;
        targetPosition = newTarget;
    }
}
