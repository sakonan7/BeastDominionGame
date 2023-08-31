using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 followDirection;
    private Vector3 attackDirection;
    public float attackForce = 6;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetAttackForce()
    {

    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.fwd * 20);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
