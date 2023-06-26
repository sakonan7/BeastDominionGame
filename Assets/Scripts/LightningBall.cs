using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBall : MonoBehaviour
{
    private Rigidbody ballRb;
    private GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        ballRb = GetComponent<Rigidbody>();
        target = GameObject.Find("Target");
    }

    // Update is called once per frame
    void Update()
    {
        ballRb.AddForce((target.transform.position - transform.position) * 15);
    }
}
