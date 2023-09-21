using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TargetTracking : MonoBehaviour
{
    // Start is called before the first frame update
    private bool gotTarget = false;
    private Vector3 targetPosition;
    private Camera mainCam;
    public GameObject HPBarHolder;
    public Image HPBar;
    float maxHPBarFill = 1;
    int enemyOriginalHP;
    int enemyCurrentHP;
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (gotTarget == true)
        {
            transform.position = mainCam.WorldToScreenPoint(targetPosition + new Vector3(0,2,0));
            //DisplayHP();
        }
    }
    public void Target(Vector3 newTarget)
    {
        //gameObject.SetActive(true);
        gotTarget = true;
        targetPosition = newTarget;
    }
    //Decrease and Display the Enemy'sH.
    public void SetHP(int originalHP, int HP)
    {
        //I need to keep setting
        //I'm thinking that enemyScript will update this with a 
        enemyOriginalHP = originalHP;
        enemyCurrentHP = HP;
        HPBar.fillAmount = (maxHPBarFill / enemyOriginalHP) * enemyCurrentHP;
    }
    public void DecreaseHP(int HP)
    {
        HPBar.fillAmount = (maxHPBarFill / enemyOriginalHP) * HP;
    }
    public void DisplayHP()
    {
        
    }
}
