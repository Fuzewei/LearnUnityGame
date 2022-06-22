using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine.Const;
using System;
using KBEngine;



//根据输入转换为相应的命令，并通知出去（输入转化层）
//TODO 时序系统，严格控制接收指令和显示的顺序，这样当帧就可以反应，减小延迟，现在很多是fixupdate做的，其实不对

public class InputControl : MonoBehaviour
{
    // Start is called before the first frame update
    private MoveMotor motor;

    void Start()
    {
        KBEngine.Event.registerOut("onRenderObjectCreate", this, "onRenderObjectCreate");
    }
    public void onRenderObjectCreate(KBEngine.Entity entity)
    {
        if (entity.isPlayer())
        {
            KBEngine.Avatar avatar = entity as KBEngine.Avatar;
            motor = avatar.renderEntity.GetComponent<MoveMotor>();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (motor == null)
        {
            return;
        }
        Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        bool acc = Input.GetKey(KeyCode.LeftShift);
        bool jump = Input.GetKeyDown(KeyCode.Space);
        bool equip = Input.GetKeyDown(KeyCode.E);


        //非持续输入的处理
        if (equip)
        {
            KBEngine.Event.fireIn("inputSwitchBattle", !motor.inBattle);
        }
        if (jump)
        {
            KBEngine.Event.fireIn("playerJump");
        }

        bool useSkill = Input.GetKeyDown(KeyCode.T);
        if (useSkill)
        {
            KBEngine.Event.fireIn("useSkill", 1);
        }

        //持续输入的处理

        if (directionVector.magnitude >= 0.2 && acc == false)
        {
            KBEngine.Event.fireIn("playerWalk");
            Dbg.DEBUG_MSG("playerWalk:");
        }
        else if (directionVector.magnitude >= 0.2 && acc == true)
        {
            KBEngine.Event.fireIn("playerRun");
        }
        else
        {
            KBEngine.Event.fireIn("playerIdle");
        }

        if (!motor.inBattle)
        {
            float rotateAngle = Mathf.Atan2(directionVector.x, directionVector.z) * Mathf.Rad2Deg;
            Vector3 inputRotation = new Vector3(0, Camera.main.transform.rotation.eulerAngles.y + rotateAngle, 0);
            inputRotation = Quaternion.Euler(inputRotation).eulerAngles;
            KBEngine.Event.fireIn("setFaceDirection", (VECTOR3)inputRotation);//面朝方向,欧拉角
            KBEngine.Event.fireIn("setMoveDirection", (VECTOR3)(Quaternion.Euler(inputRotation) * Vector3.forward).normalized);//移动方向,向量
        }
        else
        {
            Vector3 cameraFace = new Vector3(0, Camera.main.transform.rotation.eulerAngles.y, 0);

            KBEngine.Event.fireIn("setFaceDirection", (VECTOR3)cameraFace);//面朝方向
            KBEngine.Event.fireIn("setMoveDirection", (VECTOR3)(Quaternion.Euler(cameraFace) * directionVector).normalized);//移动方向
        }
    }

}
