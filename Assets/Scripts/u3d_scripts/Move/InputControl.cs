using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine.Const;
using System;
using KBEngine;



//根据输入转换为相应的命令，并通知出去（输入转化层）
[RequireComponent(typeof(MoveMotor))]
public class InputControl : MonoBehaviour
{
    // Start is called before the first frame update
    private MoveMotor motor;
    private GameEntity gameEntity;

    void Start()
    {
        motor = GetComponent<MoveMotor>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        bool acc = Input.GetKey(KeyCode.LeftShift);
        bool jump = Input.GetKeyDown(KeyCode.Space);
        bool equip = Input.GetKeyDown(KeyCode.E);

        if (equip)
        {
            KBEngine.Event.fireIn("inputSwitchBattle");
        }

        MoveConst oldMoveType = motor.setedMoveType;
        MoveConst newMoveType = MoveConst.Idel;
        if (jump)
        {
            newMoveType = MoveConst.Jump;
        }
        else if (directionVector.magnitude >= 0.2 && acc == false)
        {
            newMoveType = MoveConst.Walk;    
        }

        else if (directionVector.magnitude >= 0.2 && acc == true)
        {
            newMoveType = MoveConst.Run; 
        }
        if (!motor.inBattle)
        {
            float rotateAngle = Mathf.Atan2(directionVector.x, directionVector.z) * Mathf.Rad2Deg;
            Vector3 inputRotation = new Vector3(0, Camera.main.transform.rotation.eulerAngles.y + rotateAngle, 0);
            if (inputRotation.y < 0)
            {
                inputRotation.y += 360;
            }
            else if (inputRotation.y >= 360)
            {
                inputRotation.y -= 360;
            }
            motor.setFaceDirection(inputRotation);
            motor.setMoveDirection(Vector3.forward);
        }
        else
        {
            Vector3 cameraFace = new Vector3(0, Camera.main.transform.rotation.eulerAngles.y, 0);
            motor.setFaceDirection(cameraFace);
            motor.setMoveDirection(directionVector);
        }


        if (newMoveType != oldMoveType)
        {
            KBEngine.Event.fireIn("inputCommand", newMoveType);
        }

        bool testKey = Input.GetKeyDown(KeyCode.T);
        if (testKey)
        {
            KBEngine.Event.fireIn("useSkill", 1);

            motor.animatorController.playerSkillAttackAnimatior(motor.animator, "GreatSword_Attack01");
        }
    }


    private void LateUpdate()
    {
       
    }

}
