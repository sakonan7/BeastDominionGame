using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHPBar : MonoBehaviour
{
    private GameObject cameraRef;
    private Quaternion lookRotation;
    private Enemy enemyScript;
    //Need to reference Target here so i can put HP Bar relative to Tar

    // Start is called before the first frame update
    void Start()
    {
        cameraRef = GameObject.Find("Main Camera");
        enemyScript = GetComponentInParent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        //lookRotation = Quaternion.LookRotation(cameraRef.transform.position - transform.position);
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5);
        //I can also determine how much the HP Bar floats above the enemy
        // I'm pretty sure that I want to do stuff like making the HP Bar invisible and deducting HP here 

    }
}
