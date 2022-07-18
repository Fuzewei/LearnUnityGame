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

    public float currentTimeStamp = 0;                 //当前执行到的时间
    //a->b， 在a点知道将要走的朝向和移动状态，到b点才知道这一段的移动速度

    //设定将要干的事情，先入队列（后面要移除）
    public bool inBattle = false;                                 //战斗状态
    public bool setedInBattle = false;                                 //战斗状态
    public int skillId = 0;                                 //使用技能的id

    public MoveConst nowMoveType = MoveConst.Idel;                   //当前移动类型
    public MoveConst setedMoveType = MoveConst.Idel;                   //设置的移动类型
    public Vector3 moveDirection = Vector3.forward;               //移动方向向量(全局，向量)
    public Vector3 faceDirection = Vector3.forward;                  //面朝方向(全局,欧拉角)

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

    #region 移动的控制类和参数

    public MoveControlersBase currentMoveControler;
    protected NormalWalkControler walkMove;
    protected NormalRunControler runMove;
    protected NormalIdleControler idle;
    protected NormalJumpControler jump;
    protected NormalUseSkillControler useSkill;
    protected NormalServerMove serverMove;

    #endregion

    /*
     * p3使用的
     * 位置的预测由动画系统做了，只要调整误差
     * 转动的预测根据历史，计算转动速度
     */

    float faceDirectionDiff; //朝向误差

    float moveDirectionSpeed = 0; // 移动的方向
    float faceDirectionSpeed = 0; // 朝向y轴的转动速度（用来预测的）



    private void Awake()
    {
        onAwake();
    }
    public virtual void onAwake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        animatorController = animator.GetBehaviour<AnimatorController>();
        animator.SetInteger("moveType", (int)nowMoveType);
        animator.SetBool("inBattle", inBattle);
        animator.SetFloat("moveSpeed", moveSpeed);


        walkMove = new NormalWalkControler(this);
        runMove = new NormalRunControler(this);
        idle = new NormalIdleControler(this);
        jump = new NormalJumpControler(this);
        useSkill = new NormalUseSkillControler(this);
        serverMove = new NormalServerMove(this);
        currentMoveControler = idle;

    }



    //获取旋转角度（不直接使用transform.rotation是因为unity引擎是0-360度，需要转换成-pai 到+pai）
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
        if (setMoveType(MoveConst.Skill))
            this.skillId = skillid;
    }

    public void setFinishSkill(int skillid)
    {
        if (setMoveType(MoveConst.Idel))
            this.skillId = 0;
    }

    public virtual bool setMoveType(MoveConst state)
    {
        setedMoveType = state;
        return true;
    }


    public void setMoveDirection(Vector3 dir)
    {
        // Debug.Log("setMoveDirection" + dir + dir.magnitude);
        moveDirection = dir;
    }

    public void setFaceDirection(Vector3 direction)
    {
        //Debug.Log("setFaceDirection: " + direction );
        faceDiectionSpeed = Utils.cycleMin(0, 360, faceDirection.y, direction.y);
        //Debug.Log("setFaceDirection: " + direction + " ::" + faceDiectionSpeed);
        faceDirection = direction;
    }


    #endregion


    #region 设置怪物移动的各种属性接口

    public virtual bool setAiMovePath(PATH_POINTS _state, uint _pathIndex)
    {
        return true;
    }

    public virtual bool setAiMoveTarget(Transform tr)
    {
        return true;
    }

    public virtual bool setAiMovType(AiMoveConst aiMoveType)
    {
        return true;
    }

    public virtual bool setAiMovPoint(Vector3 movePostion)
    {
        return true;
    }

    #endregion

    protected virtual MoveControlersBase getMoveControl(MoveConst moveType)
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
            case MoveConst.beStrikefly:
                ans = serverMove;
                break;
        }
        return ans;
    }

    protected void Update()
    {
        currentTimeStamp += Time.deltaTime;
        onUpdate();

        // Debug.Log("moveTest1: "+currentTimeStamp + "delterMove" + animator.deltaPosition.magnitude);

        //if (isSyncSource)
        //{
        //    p1Update();
        //}
        //else
        //{
        //    p3Update();
        //}

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
    //此处预测朝向
    public virtual void onUpdate()
    {
    }


    //private void p1Update()
    //{
    //    if (serverOpQueue.lenth() != 0)
    //    {
    //        float recentPackageTime = serverOpQueue.recentPosition();
    //        confirmTimeStamp = recentPackageTime;
    //        localOpQueue.popBeforePosition(confirmTimeStamp - 3);

    //        var newSample = serverOpQueue.getSampleByPosition(recentPackageTime);
    //        serverOpQueue.popBeforePosition(float.MaxValue);
    //        if (newSample.Item2.moveType == MoveConst.beStrikefly)
    //        {
    //            p3Update();
    //            return;
    //        }
    //    }
    //}



    //private void p3Update()
    //{
    //    if (serverOpQueue.lenth() == 0)
    //    {
    //        return;
    //    }


    //    float recentReciveTimer = serverOpQueue.recentPosition();

    //    while (serverOpQueue.lenth() > 0)
    //    {
    //        Tuple<float, SampleBase> tuple = serverOpQueue.fromt();
    //        float serverTimer = tuple.Item1;
    //        SampleBase serverSample = tuple.Item2;
    //        serverOpQueue.pop();
    //        setMoveType(serverSample.moveType);
    //        setInBattle(serverSample.inBattle);
    //        setMoveDirection(serverSample.moveDirection);
    //        setFaceDirection(serverSample.faceDirection);
    //        if (serverTimer >= currentTimeStamp || nowMoveType != setedMoveType)
    //        {
    //            /*
    //             *强制同步的情况
    //             */
    //            currentTimeStamp = serverTimer;//todo  p1的修改
    //            setForcePosition(serverSample.position);
    //            positionDiff = new Vector3(0, 0, 0);
    //            faceDirectionSpeed = 0;

    //        }
    //        else
    //        {
    //            /*
    //             *本地领先，估算本地的误差position的diff,平滑处理不用服务器的位置
    //             */
    //            calculatePositionDiff();
    //            calculateFacedirectionSpeed();

    //        }
    //    }

    //    localOpQueue.popBeforePosition(recentReciveTimer - 3);

    //}




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

#if UNITY_EDITOR
#else
    void OnAnimatorMove()
    {
        beforeMoveUpdate();
        onMoveUpdate();
        afterMoveUpdate();
    }
#endif


    public virtual void beforeMoveUpdate()
    {

    }

    public virtual void onMoveUpdate()
    {
        currentMoveControler.tick();
        var delterMove = getDelterMove();
        currentMoveControler.UpdateMoveSpeed();
        transform.position += delterMove;
        transform.rotation = Quaternion.Euler(faceDirection); //设置朝向
        currentMoveControler.BeforeSwitchMoveControl();//设置切换状态
        flushAnimatorParameter();
        if (nowMoveType != setedMoveType)
        {
            currentMoveControler = getMoveControl(setedMoveType);
            currentMoveControler.reset();
        }
        nowMoveType = setedMoveType;
        inBattle = setedInBattle;
    }

    public virtual void afterMoveUpdate()
    {
    }

    public void _currentMoveTick()
    {
        currentMoveControler.tick();
        var delterMove = getDelterMove();
        currentMoveControler.UpdateMoveSpeed();
        //Debug.Log("moveTest2: " + currentTimeStamp + "delterMove" + animator.deltaPosition.magnitude);
        //if (positionDiff.magnitude >= 0.01)
        //{
        //    //positionDiff /= 2;
        //    delterMove += positionDiff * Mathf.Min(0.9f, Time.deltaTime);
        //    positionDiff -= positionDiff * Mathf.Min(0.9f, Time.deltaTime);
        //}

        //if (forcePosition.HasValue)
        //{
        //    transform.position = forcePosition.Value;
        //    forcePosition = null;
        //}
        //else
        //{
        //    transform.position += delterMove;
        //}



        faceDirection.y += faceDirectionSpeed * Time.deltaTime;

        if (faceDirectionDiff != 0)
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
        ////缓存指令
        //SampleBase newSample = getNewOpSample(setedMoveType, transform.position, renderRotation, moveDirection, setedInBattle); //本地放的是弧度，而且是经过调整到-pai到pai的
        //localOpQueue.push(newSample, currentTimeStamp);


        nowMoveType = setedMoveType;
        inBattle = setedInBattle;

    }

    public virtual void confirmMoveTimeStamp(float timeStamp, MoveConst moveType, Vector3 position, Vector3 faceDirection, Vector3 moveDirection, bool inBattle)
    {
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

        Vector3 localDirection = Quaternion.Inverse(Quaternion.Euler(faceDirection)) * moveDirection;
        animator.SetFloat("X", localDirection.x);
        animator.SetFloat("Z", localDirection.z);

        animator.SetBool("isGrounded", this.isOnGrounded);
        animator.SetBool("InBattle", this.inBattle);
    }

}
