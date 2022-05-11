using System.Collections.Generic;
using UnityEngine;
using KBEngine.Const;
using KBEngine;
using SwordMaster;
using System;

class MoveMotorMonster : MoveMotorAvatarP3
{
    public int pathIndex = 0;
    public List<Vector3> path = new List<Vector3>(); //怪物移动需要路点

    public Transform moveTarget = null; //怪物移动目标对象

    public AiMoveConst aiMovingType = 0;   //服务端移动类型 



    public override bool setAiMovePath(PATH_POINTS _path)
    {
        pathIndex = 0;
        path = _path as List<Vector3>;
        return true;
    }

    public override bool setAiMoveTarget(Transform tr)
    {
        moveTarget = tr;
        return true;
    }

    public override bool setAiMovType(AiMoveConst _aiMoveType)
    {
        aiMovingType = _aiMoveType;
        return true;
    }


    public override void onUpdate()
    {
        base.onUpdate();

    }

    public override void beforeMoveUpdate()
    {
        if (aiMovingType == AiMoveConst.CHAST_RUN)
        {
            Vector3 _direction = moveTarget.position - transform.position;
            float _dis = _direction.magnitude;
            _direction = _direction.normalized;
            setMoveDirection(_direction);

            float rotateAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
            Vector3 faceDirection = new Vector3(0, rotateAngle, 0);
            setFaceDirection(faceDirection);

        }
    }

    public override void afterMoveUpdate()
    {
        if (aiMovingType == AiMoveConst.RANDOM_MOVE)
        {
            Vector3 pos = transform.position;
            Vector3 nextPoint = path[pathIndex];
            Vector3 _direction = nextPoint - pos;
            float _dis = _direction.magnitude;
            _direction = _direction.normalized;
            setMoveDirection(_direction);

            float rotateAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
            Vector3 faceDirection = new Vector3(0, rotateAngle, 0);
            setFaceDirection(faceDirection);

        }

        if (aiMovingType == AiMoveConst.CHAST_RUN)
        {
            Vector3 _direction = moveTarget.position - transform.position;
            float _dis = _direction.magnitude;
            _direction = _direction.normalized;
            setMoveDirection(_direction);
            float rotateAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
            Vector3 faceDirection = new Vector3(0, rotateAngle, 0);
            setFaceDirection(faceDirection);

        }
    }

    protected override void clientUpdatePositionAfterServerUpdate()
    {
        base.clientUpdatePositionAfterServerUpdate();
    }


    public override void confirmMoveTimeStamp(float timeStamp, MoveConst moveType, Vector3 position, Vector3 faceDirection, Vector3 moveDirection, bool inBattle)
    {
        base.confirmMoveTimeStamp(timeStamp, moveType, position, faceDirection, moveDirection, inBattle);
        Debug.Log("monster.confirmMoveTimeStamp" + timeStamp);
    }
}

