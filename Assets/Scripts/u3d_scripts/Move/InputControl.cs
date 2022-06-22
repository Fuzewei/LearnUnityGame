using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine.Const;
using System;
using KBEngine;



//��������ת��Ϊ��Ӧ�������֪ͨ��ȥ������ת���㣩
//TODO ʱ��ϵͳ���ϸ���ƽ���ָ�����ʾ��˳��������֡�Ϳ��Է�Ӧ����С�ӳ٣����ںܶ���fixupdate���ģ���ʵ����

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


        //�ǳ�������Ĵ���
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

        //��������Ĵ���

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
            KBEngine.Event.fireIn("setFaceDirection", (VECTOR3)inputRotation);//�泯����,ŷ����
            KBEngine.Event.fireIn("setMoveDirection", (VECTOR3)(Quaternion.Euler(inputRotation) * Vector3.forward).normalized);//�ƶ�����,����
        }
        else
        {
            Vector3 cameraFace = new Vector3(0, Camera.main.transform.rotation.eulerAngles.y, 0);

            KBEngine.Event.fireIn("setFaceDirection", (VECTOR3)cameraFace);//�泯����
            KBEngine.Event.fireIn("setMoveDirection", (VECTOR3)(Quaternion.Euler(cameraFace) * directionVector).normalized);//�ƶ�����
        }
    }

}
