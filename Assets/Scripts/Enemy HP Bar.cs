using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHPBar : MonoBehaviour
{
    private GameObject cameraRef;
    private Quaternion lookRotation;
    private Enemy enemyScript;
    private GameObject target;
    public GameObject HP;
    private float originalHPLength;
    //Need to reference Target here so i can put HP Bar relative to Tar

    // Start is called before the first frame update
    void Start()
    {
        cameraRef = GameObject.Find("Main Camera");
        enemyScript = GetComponentInParent<Enemy>();
        target = GameObject.Find("Target");
        originalHPLength = HP.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        //lookRotation = Quaternion.LookRotation(cameraRef.transform.position - transform.position);
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5);
        //I can also determine how much the HP Bar floats above the enemy
        // I'm pretty sure that I want to do stuff like making the HP Bar invisible and deducting HP here 
        //transform.LookAt(transform.position - (cameraRef.transform.position - transform.position));
        //transform.rotation = new Quaternion();
        //transform.rotation = target.transform.rotation;
        //transform.rotation = new Quaternion(transform.rotation.x -90, transform.rotation.y, transform.rotation.z, 0);
        transform.position = new Vector3(target.transform.position.x, target.transform.position.y + 2.5f, target.transform.position.z);
        if (enemyScript.lockedOn == true)
        {
            gameObject.SetActive(true);
        }
        else if (enemyScript.lockedOn == false)
        {
            gameObject.SetActive(false);
        }
    }
    public void HPDecrease(float newDamage, float originalHP)
    {
        float damage = (originalHPLength / originalHP) * newDamage;
        HP.transform.localScale -= new Vector3(damage, 0, 0);
        HP.transform.position -= new Vector3(damage/2, 0, 0);
        //Debug.Log("Damage Dealt Is " +damage);
    }
}
