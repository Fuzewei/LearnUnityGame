using System.Collections.Generic;
using UnityEngine;
using KBEngine.Const;
using KBEngine;
using SwordMaster;
using System;

class MoveMotorMonster : MoveMotorAvatarP3
{
    int pathIndex = 0;
    List<Vector3> path = new List<Vector3>();

    public override bool setMoveType(MoveConst state)
    {
        base.setMoveType(state);
        if (setedMoveType == MoveConst.Walk)
        {
            pathIndex = 0;
            path = moveParam as List<Vector3>;
        }
       
        return true;
    }

    private void LateUpdate()
    {
        if (setedMoveType == MoveConst.Walk)
        {
            if (pathIndex >= path.Count)
            {
                return;
            }

            Vector3 pos = transform.position;
            Vector3 nextPoint = path[pathIndex];
            Vector3 _direction = nextPoint - pos;
            float _dis = _direction.magnitude;
            _direction = _direction.normalized;
        }
    }

    //收到确认位置信息
    public override void confirmMoveTimeStamp(float timeStamp, MoveConst moveType, Vector3 position, Vector3 faceDirection, Vector3 moveDirection, bool inBattle)
    {
        SampleBase newSample = getNewOpSample(moveType, position, faceDirection, moveDirection, inBattle);
        serverOpQueue.push(newSample, timeStamp);
        forecastOpQueue.push(newSample, timeStamp);
        forecastOpQueue.popBeforePosition(timeStamp - 3);
       
    }
}

