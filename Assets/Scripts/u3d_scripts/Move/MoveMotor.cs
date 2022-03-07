using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine.Const;
using KBEngine;
using SwordMaster;
using System;

//全局移动属性
class MotorSettingParam
{

    public float Gravity = 9.8f;
}


//程序控制移动的类(必须先设置属性在设置控制类)
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class MoveMotor : MonoBehaviour
{
    private MotorSettingParam moveParam = new MotorSettingParam();
    //操作队列相关
    public SampleQueue localOpQueue; //本地的操作队列
    public SampleQueue serverOpQueue; //收到的服务端操作队列
    public float currentTimeStamp = 0;                 //当前执行到的时间

    public float confirmTimeStamp = 0;                 //服务器确认的时间

    public bool isSyncSource = true;
    //操作队列相关 end

    //a->b， 在a点知道将要走的朝向和移动状态，到b点才知道这一段的移动速度

    //设定将要干的事情，先入队列（后面要移除）
    public bool inBattle = false;                                 //战斗状态
    public bool setedInBattle = false;                                 //战斗状态
    public int skillId = 0;                                 //使用技能的id

    public MoveConst nowMoveType = MoveConst.Idel;                   //当前移动类型
    public MoveConst setedMoveType = MoveConst.Idel;                   //设置的移动类型
    public Vector3 moveDirection = Vector3.forward;               //移动方向(局部,向量)
    public Vector3 faceDirection = Vector3.zero;                  //面朝方向(全局,欧拉角)

    public Vector3? forcePosition;                  //面朝方向(全局,欧拉角)
    //设定将要干的事情 end

    //计算的值（控制动画状态机的参数）
    public float moveSpeed = 0.0f;                               //当前个水平方向的移速
    public float faceDiectionSpeed = 0.0f;                               //面朝方向转向速度（+向右）
    public float moveVerticalSpeed = 0.0f;                         //当前个竖直方向的移速

    public float stopMoveBlend = 0.0f;                               //停止混合参数




    //计算的值（控制动画状态机）end

    public Animator animator;                                        //动画驱动移动(只用来计算位置)
    public AnimatorController animatorController;

    CharacterController controller;

    #region 移动的控制类
    public MoveControlersBase currentMoveControler;
    NormalWalkControler walkMove;
    NormalRunControler runMove;
    NormalIdleControler idle;
    NormalJumpControler jump;
    NormalUseSkillControler useSkill;
    NormalServerMove serverMove;
    #endregion

    /*
     * p3使用的
     * 位置的预测由动画系统做了，只要调整误差
     * 转动的预测根据历史，计算转动速度
     */
    Vector3 positionDiff; //移动误差（位置慢慢调整）
    float faceDirectionDiff; //朝向误差

    float moveDirectionSpeed = 0; // 移动的方向
    float faceDirectionSpeed = 0; // 朝向y轴的转动速度（用来预测的）
    public SampleQueue forecastOpQueue; //预测的队列

    public Vector3 globalmoveDirection
    {
        get
        {
            return (gameObject.transform.rotation * moveDirection).normalized;
        }
    }

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        animatorController = animator.GetBehaviour<AnimatorController>();
        animator.SetInteger("moveType", (int)nowMoveType);
        animator.SetBool("inBattle", inBattle);
        animator.SetFloat("moveSpeed", moveSpeed);
        localOpQueue = new SampleQueue();
        serverOpQueue = new SampleQueue();
        forecastOpQueue = new SampleQueue();

        SampleBase newSample = getNewOpSample(setedMoveType, transform.position, faceDirection, moveDirection, inBattle);
        localOpQueue.push(newSample, currentTimeStamp);

        walkMove = new NormalWalkControler(this);
        runMove = new NormalRunControler(this);
        idle = new NormalIdleControler(this);
        jump = new NormalJumpControler(this);
        useSkill = new NormalUseSkillControler(this);
        serverMove = new NormalServerMove(this);
        currentMoveControler = idle;

        positionDiff = new Vector3(0, 0, 0);
    }


    //unity引擎是0-360度，需要转换成-pai 到+pai
    public Vector3 renderRotation
    {
        get
        {
            Vector3 ans = transform.rotation.eulerAngles;
           // Debug.Log("setFaceDirection: renderRotation1: " + transform.rotation.eulerAngles);
            var y = (double)(ans.y / 360 * (System.Math.PI * 2));
            if (y - System.Math.PI > 0.0)
                y -= System.Math.PI * 2;
            ans.y = (float)y;
           // Debug.Log("setFaceDirection: renderRotation2: " + ans);
            return ans;
        }
    }

    public bool isOnGrounded
    {
        get
        {
            return controller.isGrounded;
        }
    }

    #region 设置各种属性

    public void setInBattle(bool isInbattle)
    {
        this.setedInBattle = isInbattle;
    }

    public void setInUseSkill(int skillid)
    {
        if(setMoveType(MoveConst.Skill))
            this.skillId = skillid;
    }

    public void setFinishSkill(int skillid)
    {
        if (setMoveType(MoveConst.Idel))
            this.skillId = 0;
    }

    public bool setMoveType(MoveConst state)
    {
        setedMoveType = state;
        return true;
    }

    public void setMoveDirection(Vector3 dir)
    {
        moveDirection = dir;
    }

    public void setFaceDirection(Vector3 direction)
    {
        Debug.Log("setFaceDirection: " + direction );
        faceDiectionSpeed = Utils.cycleMin(0, 360, faceDirection.y, direction.y);
        Debug.Log("setFaceDirection: " + direction + " ::" + faceDiectionSpeed);
        faceDirection = direction;
    }

    public void setForcePosition(Vector3 position)
    {
        forcePosition = position;
    }

    #endregion
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

    private MoveControlersBase getMoveControl(MoveConst moveType)
    {
        MoveControlersBase ans = null;
        switch (moveType)
        {
            case MoveConst.Idel:
                ans = idle;
                break;
            case MoveConst.Walk:
                ans = walkMove;
                break;
            case MoveConst.Run:
                ans = runMove;
                break;
            case MoveConst.Jump:
                ans = jump;
                break;
            case MoveConst.Skill:
                ans = useSkill;
                break;
            case MoveConst.ServerMove:
                ans = serverMove;
                break;
        }
        return ans;
    }

    private void Update()
    {
        currentTimeStamp += Time.deltaTime;

        // Debug.Log("moveTest1: "+currentTimeStamp + "delterMove" + animator.deltaPosition.magnitude);
        for (int i = 1; i < forecastOpQueue.lenth(); i++)
        {
            var b = forecastOpQueue.getSampleByIndex(i);
            var e = forecastOpQueue.getSampleByIndex(i -1 );
            var p = b.Item2.position;
            var p112 = e.Item2.position;
            p.y += 1;
            p112.y += 1;
            Debug.DrawLine(p, p112, Color.yellow);
        }
       

        if (isSyncSource)
        {
            p1Update();
        }
        else
        {
            p3Update();
        }

        #if UNITY_EDITOR
        var p1 = animator.GetFloat("Param1");
        var p2 = animator.GetFloat("Param2");

        Vector3 v = new Vector3(p1, 0, p2);
        if (v.magnitude <= 0.5f)
        {
            return;
        }
        v = transform.rotation * v;
        Debug.DrawLine(transform.position + new Vector3(0, 1, 0), transform.position + v * 5 + new Vector3(0, 1, 0), Color.red);
        #endif
    }

    private void p1Update()
    {
        if (serverOpQueue.lenth() != 0)
        {
            float recentPackageTime = serverOpQueue.recentPosition();
            confirmTimeStamp = recentPackageTime;
            localOpQueue.popBeforePosition(confirmTimeStamp - 3);

            var newSample = serverOpQueue.getSampleByPosition(recentPackageTime);
            serverOpQueue.popBeforePosition(float.MaxValue);
            if (newSample.Item2.moveType == MoveConst.ServerMove)
            {
                p3Update();
                return;
            }
        }
    }



    private void p3Update()
    {
        if (serverOpQueue.lenth() == 0)
        {
            return;
        }

       
        float recentReciveTimer = serverOpQueue.recentPosition();

        while (serverOpQueue.lenth() > 0)
        {
            Tuple<float, SampleBase> tuple = serverOpQueue.fromt();
            float serverTimer = tuple.Item1;
            SampleBase serverSample = tuple.Item2;
            serverOpQueue.pop();
            setMoveType(serverSample.moveType);
            setInBattle(serverSample.inBattle);
            setMoveDirection(serverSample.moveDirection);
            setFaceDirection(serverSample.faceDirection);
            if (serverTimer >= currentTimeStamp || nowMoveType != setedMoveType)
            {
                /*
                 *强制同步的情况
                 */
                currentTimeStamp = serverTimer;//todo  p1的修改
                setForcePosition(serverSample.position);
                positionDiff = new Vector3(0, 0, 0);
                faceDirectionSpeed = 0;

            }
            else
            {
                /*
                 *本地领先，估算本地的误差position的diff,平滑处理不用服务器的位置
                 */
                calculatePositionDiff();
                calculateFacedirectionSpeed();

            }
        }

        localOpQueue.popBeforePosition(recentReciveTimer - 3);

    }

    

    void calculateFacedirectionSpeed()
    {
        if (forecastOpQueue.lenth() < 3)
        {
            return;
        }
        var r = forecastOpQueue.end();

        faceDirectionSpeed = 0;
        //计算旋转的预测
        var secend = forecastOpQueue.getSampleByIndex(forecastOpQueue.lenth() - 2);
        float delterTimer = r.Item1 - secend.Item1;
        var y_recent = r.Item2.faceDirection.y;
        var y_secend = secend.Item2.faceDirection.y;
        //if (y_recent - y_secend < -180)
        //{
        //    y_recent = 180 + y_recent + 180;
        //}
        //else if (y_recent - y_secend >= 180)
        //{
        //    y_recent = y_recent - 180 - 180;
        //}
        //float y_delter = y_recent - y_secend;
        float y_delter = Utils.cycleMin(-180, 180, y_secend, y_recent);

        if (delterTimer < 0.008)
        {
            faceDirectionSpeed = 0;
        }
        else
        {
            faceDirectionSpeed = y_delter / delterTimer;
        }
        if (faceDirectionSpeed < 20)
        {
            faceDirectionSpeed = 0;
        }
    }

    void calculatePositionDiff()
    {
        if (forecastOpQueue.lenth() < 3)
        {
            return;
        }
        //移动计算误差，进行调整
        positionDiff = new Vector3(0, 0, 0);

        var r = forecastOpQueue.end();
        SampleBase recentSample = r.Item2;
        var l = localOpQueue.getSampleByPosition(r.Item1);
        SampleBase localSample = l.Item2;
        float timeDiff = r.Item1 - l.Item1;
        var moveDirection = (Quaternion.Euler(localSample.faceDirection) * localSample.moveDirection).normalized;
        positionDiff = recentSample.position - (localSample.position + timeDiff * moveSpeed * moveDirection);
        Debug.Log("positionDiff" + positionDiff.magnitude + "delter_time:" + timeDiff + ":" + recentSample.position + "-----" + (localSample.position + timeDiff * moveSpeed * moveDirection));


        faceDirectionDiff = 0;
        faceDirectionDiff = recentSample.faceDirection.y - (localSample.faceDirection.y + timeDiff * faceDirectionSpeed);
    }

    public Vector3 getDelterMove()
    {
        Vector3 moveDis = currentMoveControler.calcuteDelterPosition();//马上要移动的偏移
        Vector3 old = transform.position;
        controller.Move(moveDis); //设置位移
        Vector3 _new = transform.position;
        transform.position = old;
        Vector3 delter = _new - old;
        return delter;
    }
    void OnAnimatorMove()
    {
        currentMoveControler.tick(Time.deltaTime);
        currentMoveControler.UpdateMoveSpeed();
        var delterMove = getDelterMove();
        //Debug.Log("moveTest2: " + currentTimeStamp + "delterMove" + animator.deltaPosition.magnitude);
        if (positionDiff.magnitude >= 0.01)
        {
            //positionDiff /= 2;
            delterMove += positionDiff * Time.deltaTime;
            positionDiff -= positionDiff * Time.deltaTime;
        }

        if (forcePosition.HasValue)
        {
            transform.position = forcePosition.Value;
            forcePosition = null;
        }
        else {
            transform.position += delterMove;
        }


        //Vector3 moveDis = currentMoveControler.calcuteDelterPosition();//计算位移偏移(最好提前)
        //controller.Move(moveDis); //设置位移


        faceDirection.y += faceDirectionSpeed * Time.deltaTime;

        if (faceDirectionDiff!=0)
        {
           // faceDirection.y += faceDirectionDiff * Time.deltaTime;
            //faceDirectionDiff-= faceDirectionDiff * Time.deltaTime;
        }

        transform.rotation = Quaternion.Euler(faceDirection); //设置朝向
        currentMoveControler.BeforeSwitchMoveControl();//设置切换状态
        flushAnimatorParameter();
        if (nowMoveType != setedMoveType)
        {
            currentMoveControler = getMoveControl(setedMoveType);
            currentMoveControler.reset();

        }
        //缓存指令
        SampleBase newSample = getNewOpSample(setedMoveType, transform.position, renderRotation, moveDirection, setedInBattle); //本地放的是弧度，而且是经过调整到-pai到pai的
        localOpQueue.push(newSample, currentTimeStamp);

        if (isSyncSource)
        {
            KBEngine.Avatar avatar = (KBEngine.Avatar)KBEngineApp.app.player();
            if (nowMoveType != setedMoveType)
            {  
                avatar.uploadMovetype(setedMoveType);
            }

            if (setedInBattle != inBattle)
            { 
                avatar.uploadInbattle(setedInBattle);
            }
        }
        nowMoveType = setedMoveType;
        inBattle = setedInBattle;
    }


    //收到确认位置信息
    public void confirmMoveTimeStamp(float timeStamp, MoveConst moveType, Vector3 position, Vector3 faceDirection, Vector3 moveDirection, bool inBattle)
    {
        SampleBase newSample = getNewOpSample(moveType, position, faceDirection, moveDirection, inBattle);
        serverOpQueue.push(newSample, timeStamp);
        forecastOpQueue.push(newSample, timeStamp);
        forecastOpQueue.popBeforePosition(timeStamp - 3);
        Debug.Log("confirmMoveTimeStamp" + faceDirection);
    }

    //刷新状态机参数
    private void flushAnimatorParameter()
    {
        MoveConst animMoveType = (MoveConst)animator.GetInteger("moveType");
        bool animInBattle = animator.GetBool("InBattle");
        if (setedMoveType == MoveConst.Idel)
        {

            if (animMoveType != MoveConst.Idel)
            {
                this.stopMoveBlend = (this.moveSpeed - walkMove.maxForwardSpeed) / (runMove.maxForwardSpeed - walkMove.maxForwardSpeed);
            }
        }
        if (setedMoveType == MoveConst.Jump && animMoveType != MoveConst.Jump)
        {
            animator.SetTrigger("TriggerJump");
        }

        if (animInBattle != inBattle)
        {
            animator.SetTrigger("TriggerEquip");
        }

        animator.SetInteger("moveType", (int)setedMoveType);
        animator.SetFloat("moveSpeed", this.moveSpeed);
        animator.SetFloat("rotationAngle", faceDiectionSpeed * 10);
        animator.SetFloat("stopMoveBlend", this.stopMoveBlend);

        animator.SetFloat("X", this.moveDirection.x);
        animator.SetFloat("Z", this.moveDirection.z);

        animator.SetBool("isGrounded", this.isOnGrounded);
        animator.SetBool("InBattle", this.inBattle);
    }

}
