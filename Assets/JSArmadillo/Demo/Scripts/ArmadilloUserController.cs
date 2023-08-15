using UnityEngine;
using System.Collections;

public class ArmadilloUserController : MonoBehaviour
{
    ArmadilloCharacter armadilloCharacter;

    void Start()
    {
        armadilloCharacter = GetComponent<ArmadilloCharacter>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            armadilloCharacter.Attack();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            armadilloCharacter.Hit();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            armadilloCharacter.Gallop();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            armadilloCharacter.Walk();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            armadilloCharacter.BallStart();
        }

        if (Input.GetKeyUp(KeyCode.B))
        {
            armadilloCharacter.BallEnd();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            armadilloCharacter.EatStart();
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            armadilloCharacter.EatEnd();
        }



        armadilloCharacter.forwardSpeed = armadilloCharacter.maxWalkSpeed * Input.GetAxis("Vertical");
        armadilloCharacter.turnSpeed = Input.GetAxis("Horizontal");
    }

}
