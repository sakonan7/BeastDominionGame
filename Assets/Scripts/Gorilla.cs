using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gorilla : MonoBehaviour
{
    private bool ultimateAttackStart = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //For the arena lighting up, warning the player that the whole arena will be consumed in fire
    //I think I will adjust the light source itself to create the light lighting up and down
    //Do another version for the single hand smash shock
    IEnumerator UltimateAttackChargeUp()
    {
        while(ultimateAttackStart)
        {
            yield return new WaitForSeconds(0.5f);
        }
    }
}
