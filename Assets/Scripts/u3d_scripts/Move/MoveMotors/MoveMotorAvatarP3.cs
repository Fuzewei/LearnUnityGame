using System.Collections.Generic;
using UnityEngine;
using KBEngine.Const;
using KBEngine;
using SwordMaster;
using System;


public class MoveMotorAvatarP3 : MoveMotorServerFix
{
    public SampleQueue serverOpQueue; //收到的p1操作队列


    public override void onAwake()
    {
        base.onAwake();
        serverOpQueue = new SampleQueue();
    }
    public override void onUpdate()
    {
        base.onUpdate();

    }

    public override void onMoveUpdate()
    {
        base.onMoveUpdate();
    }

    public override void confirmMoveTimeStamp(float timeStamp, MoveConst moveType, Vector3 position, Vector3 faceDirection, Vector3 moveDirection, bool inBattle)
    {
        base.confirmMoveTimeStamp(timeStamp, moveType, position, faceDirection, moveDirection, inBattle);
        SampleBase newSample = getNewOpSample(moveType, position, faceDirection, moveDirection, inBattle);
        serverOpQueue.push(newSample, timeStamp);
        //Debug.Log("confirmMoveTimeStamp" + faceDirection);
    }
}

