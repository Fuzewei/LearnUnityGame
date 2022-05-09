using System.Collections.Generic;
using UnityEngine;
using KBEngine.Const;
using KBEngine;
using SwordMaster;
using System;

//根据服务端的结果修正移动位置
public class MoveMotorServerFix : MoveMotor
{

    //操作队列相关
    public SampleQueue localOpQueue; //本地的操作队列
    public SampleQueue forecastOpQueue; //预测的队列
    public float confirmTimeStamp = 0;                 //服务器确认的最近的时间

    public bool isSyncSource = true;
    //操作队列相关 end


    Vector3 positionDiff; //移动误差（位置慢慢调整）

    public override void onAwake()
    {
        base.onAwake();
        localOpQueue = new SampleQueue();
        
        forecastOpQueue = new SampleQueue();
        SampleBase newSample = getNewOpSample(setedMoveType, transform.position, faceDirection, moveDirection, inBattle);
        localOpQueue.push(newSample, currentTimeStamp);
        positionDiff = new Vector3(0, 0, 0);
    }


    public override void onUpdate() 
    {
        base.onUpdate();
        for (int i = 1; i < forecastOpQueue.lenth(); i++)
        {
            var b = forecastOpQueue[i];
            var e = forecastOpQueue[i - 1];
            var p = b.Item2.position;
            var p112 = e.Item2.position;
            p.y += 1;
            p112.y += 1;
            Debug.DrawLine(p, p112, Color.yellow);
        }
    }

    public override void onMoveUpdate()
    {
        base.onMoveUpdate();//表现层移动（位置的预测的所以需要修正）
        clientUpdatePositionAfterServerUpdate();//根据服务器信息修正位置
        //缓存指令
        SampleBase newSample = getNewOpSample(nowMoveType, transform.position, renderRotation, moveDirection, setedInBattle); //本地放的是弧度，而且是经过调整到-pai到pai的
        localOpQueue.push(newSample, currentTimeStamp);
        localOpQueue.popBeforePosition(currentTimeStamp - 5);

    }

    //移动完成后根据服务端发来的数据修正位置
    protected virtual void clientUpdatePositionAfterServerUpdate()
    {
        if (forecastOpQueue.lenth() == 0)
        {
            return;
        }

        float recentServerTime = forecastOpQueue[-1].Item1;
        SampleBase recentSample = forecastOpQueue[-1].Item2;

        if (recentServerTime > currentTimeStamp) //客户端比服务端慢，强制设置位置和朝向
        {
            currentTimeStamp = recentServerTime;
            transform.position = recentSample.position;
            transform.rotation = Quaternion.Euler(recentSample.faceDirection);
            return;
        }
        else   //客户端比服务端快
        {
            if (recentServerTime - confirmTimeStamp > float.Epsilon) //服务端新的数据到了才更新误差
            {
                confirmTimeStamp = recentServerTime;
                positionDiff = calculatePositionDiff();
            }
           
            if (positionDiff.magnitude >= 0.01)
            {
                transform.position += positionDiff * Mathf.Min(0.9f, Time.deltaTime);
                positionDiff -= positionDiff * Mathf.Min(0.9f, Time.deltaTime);
            }

        }

    }

    float  calculateFacedirectionSpeed()
    {
        if (forecastOpQueue.lenth() < 3)
        {
            return 0;
        }
        var last = forecastOpQueue[-1];

        float faceDirectionSpeed = 0;
        //计算旋转的预测
        var secend = forecastOpQueue[-2];
        float delterTimer = last.Item1 - secend.Item1;
        var y_recent = last.Item2.faceDirection.y;
        var y_secend = secend.Item2.faceDirection.y;
        float y_delter = Utils.cycleMin(-180, 180, y_secend, y_recent);

        faceDirectionSpeed = y_delter / delterTimer;
       
        if (faceDirectionSpeed < 40)
        {
            faceDirectionSpeed = 0;
        }
        return faceDirectionSpeed;
    }

    Vector3 calculatePositionDiff()
    {
        Vector3 _positionDiff = new Vector3(0, 0, 0);
        if (forecastOpQueue.lenth() < 3)
        {
            return _positionDiff;
        }

        //移动计算误差，进行调整
      

        float recentServerTime = forecastOpQueue[-1].Item1;
        SampleBase recentSample = forecastOpQueue[-1].Item2;
        var leftAndRight = localOpQueue.getLeftAndRight(recentServerTime);

        var localSampleL = localOpQueue[leftAndRight.Item1];
        var localSampleR = localOpQueue[leftAndRight.Item2];


        float timeDiff = recentServerTime - localSampleL.Item1;
        Vector3 moveDirection = localSampleL.Item2.moveDirection.normalized;
        Vector3 positionDiff_1 = recentSample.position - (localSampleL.Item2.position + timeDiff * moveSpeed * moveDirection);


        Vector3 lerpPoint = Vector3.Lerp(localSampleL.Item2.position, localSampleR.Item2.position, timeDiff / (localSampleR.Item1 - localSampleL.Item1));
        Vector3 positionDiff_2 = recentSample.position - lerpPoint;

        _positionDiff = positionDiff_2.magnitude > positionDiff_1.magnitude ? positionDiff_1 : positionDiff_2;

        return _positionDiff;

        // Debug.Log("positionDiff" + positionDiff.magnitude + "delter_time:" + timeDiff + ":" + forecastSample.position + "-----" + (localSample.position + timeDiff * moveSpeed * moveDirection));
        //faceDirectionDiff = 0;
        //faceDirectionDiff = forecastSample.faceDirection.y - (localSample.faceDirection.y + timeDiff * faceDirectionSpeed);
    }



    public SampleBase getNewOpSample(MoveConst moveType, Vector3 position, Vector3 faceDirection, Vector3 moveDirection, bool inBattle)
    {
        SampleBase ans = null;

        switch (moveType)
        {
            case MoveConst.Idel:
                ans = new IdelSamples(position, faceDirection, moveDirection, inBattle);
                break;
            case MoveConst.Walk:
                ans = new WalkSamples(position, faceDirection, moveDirection, inBattle);
                break;
            case MoveConst.Run:
                ans = new RunSamples(position, faceDirection, moveDirection, inBattle);
                break;
            case MoveConst.Jump:
                ans = new JumpSamples(position, faceDirection, moveDirection, inBattle);
                break;
            case MoveConst.Skill:
                ans = new SkillSamples(position, faceDirection, moveDirection, inBattle);
                break;
            case MoveConst.ServerMove:
                ans = new ServerMoveSamples(position, faceDirection, moveDirection, inBattle);
                break;
        }
        return ans;
    }

    //收到服务器确认位置信息
    public override void confirmMoveTimeStamp(float timeStamp, MoveConst moveType, Vector3 position, Vector3 faceDirection, Vector3 moveDirection, bool inBattle)
    {
        SampleBase newSample = getNewOpSample(moveType, position, faceDirection, moveDirection, inBattle);
        forecastOpQueue.push(newSample, timeStamp);
        forecastOpQueue.popBeforePosition(timeStamp - 3);
        //Debug.Log("confirmMoveTimeStamp" + faceDirection);
    }

}


