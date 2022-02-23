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


//程序控制移动的类
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class MoveMotor : MonoBehaviour
{
    private MotorSettingParam moveParam = new MotorSettingParam();
    //操作队列相关
    private SampleQueue opQueue;
    public float oldTimeStamp = 0;                 //上次执行的时间戳
    public float currentTimeStamp = 0;                 //当前执行到的时间
    public float latestTimeStamp = 0;                 //最新位置数据包的时间
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
    //设定将要干的事情 end

    //计算的值（控制动画状态机的参数）
    public float moveSpeed = 0.0f;                               //当前个水平方向的移速
    public float moveVerticalSpeed = 0.0f;                         //当前个竖直方向的移速

    public float stopMoveBlend = 0.0f;                               //停止混合参数




    //计算的值（控制动画状态机）end

    public Animator animator;                                        //动画驱动移动(只用来计算位置)
    public AnimatorController animatorController;

    CharacterController controller;

    public MoveControlersBase currentMoveControler;
    NormalWalkControler walkMove;
    NormalRunControler runMove;
    NormalIdleControler idle;
    NormalJumpControler jump;
    NormalUseSkillControler useSkill;

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
        opQueue = new SampleQueue();

        SampleBase newSample = getNewOpSample(setedMoveType, transform.position, faceDirection, moveDirection, inBattle);
        opQueue.push(newSample, currentTimeStamp);

        walkMove = new NormalWalkControler(this);
        runMove = new NormalRunControler(this);
        idle = new NormalIdleControler(this);
        jump = new NormalJumpControler(this);
        useSkill = new NormalUseSkillControler(this);
        currentMoveControler = idle;
    }

    public Vector3 renderRotation
    {
        get
        {
            Vector3 ans = transform.rotation.eulerAngles;
            var y = (double)(ans.y / 360 * (System.Math.PI * 2));
            if (y - System.Math.PI > 0.0)
                y -= System.Math.PI * 2;
            ans.y = (float)y;
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

    public void setInBattle(bool isInbattle)
    {
        this.setedInBattle = isInbattle;
    }

    public void setInUseSkill(int skillid)
    {
        if(setMoveType(MoveConst.Skill))
            this.skillId = skillid;
    }

    public bool setMoveType(MoveConst state)
    {
        if (!currentMoveControler.canSetNewMoveType(state)) //是否接受输入判断
        {
            return false;
        }
        setedMoveType = state;
        return true;
    }

    public void setMoveDirection(Vector3 dir)
    {
        moveDirection = dir;
    }

    public void setFaceDirection(Vector3 direction)
    {
        //float delta = rotation.y - faceRotation.y;
        //float dir = 1;
        //if (delta > 180 || delta <= 0)
        //{
        //    dir = -1;
        //}
        //else if (delta < -180 || delta > 0)
        //{
        //    dir = 1;
        //}


        //Debug.Log("setFaceRotation:" + rotation);
        //faceRotation.y += dir * Time.deltaTime * 360;

        if (!currentMoveControler.canSetFaceDirection(ref direction))
        {
            faceDirection = transform.rotation.eulerAngles;
            return;
        }
        faceDirection = direction;
    }
    private SampleBase getNewOpSample(MoveConst moveType, Vector3 position, Vector3 faceDirection, Vector3 moveDirection, bool inBattle)
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
        }
        return ans;
    }


    void OnAnimatorMove()
    {
        //Debug.Log("OnAnimatorMove:" + animator.speed + animator.velocity.magnitude);
        currentMoveControler.tick(Time.deltaTime);
        currentMoveControler.UpdateMoveSpeed();
        if (isSyncSource)
        {
            currentTimeStamp = Utils.localTime();
            latestTimeStamp = currentTimeStamp;
            flushAnimatorParameter();
            controller.Move(currentMoveControler.calcuteDelterPosition());
            transform.rotation = Quaternion.Euler(faceDirection);
            SampleBase newSample = getNewOpSample(setedMoveType, transform.position, renderRotation, moveDirection, setedInBattle);
            opQueue.push(newSample, currentTimeStamp);
            if (nowMoveType != setedMoveType || setedInBattle != inBattle)
            {
                KBEngine.Event.fireIn("uploadMovetypeAndPositionAndRotation", currentTimeStamp, newSample.moveType, newSample.position, newSample.faceDirection, newSample.moveDirection, newSample.inBattle);
                if (nowMoveType != setedMoveType)
                {
                    currentMoveControler = getMoveControl(newSample.moveType);
                    currentMoveControler.reset();
                }

                nowMoveType = newSample.moveType;
                inBattle = setedInBattle;
            }
        }
        else
        {
            SampleBase newSample = opQueue.getSampleByPosition(latestTimeStamp, moveSpeed);
            setFaceDirection(newSample.faceDirection);
            setMoveType(newSample.moveType);
            setMoveDirection(newSample.moveDirection);
            setInBattle(newSample.inBattle);
            if (latestTimeStamp != currentTimeStamp)
            {
                currentTimeStamp = latestTimeStamp;
            }
            if (nowMoveType != setedMoveType || setedInBattle != inBattle)
            {
                if (nowMoveType != setedMoveType)
                {
                    currentMoveControler = getMoveControl(newSample.moveType);
                    currentMoveControler.reset();
                }
                faceDirection = newSample.moveDirection;
                transform.rotation = Quaternion.Euler(faceDirection);
                transform.position = newSample.position;
                flushAnimatorParameter();
                nowMoveType = setedMoveType;
                inBattle = setedInBattle;
            }
            else
            {
                flushAnimatorParameter();
                controller.Move(currentMoveControler.calcuteDelterPosition());
                transform.rotation = Quaternion.Euler(faceDirection);

            }

        }
    }

    //收到确认位置信息
    public void confirmMoveTimeStamp(float timeStamp, MoveConst moveType, Vector3 position, Vector3 faceDirection, Vector3 moveDirection, bool inBattle)
    {
        if (isSyncSource)
        {

            /*todo
             * 除了调整队列，还要看是否有冲突并且更新队列中未被确认的位置
             */
            opQueue.popBeforePosition(timeStamp - 1000);//保留确认前一秒的位置
        }
        else
        {
            Debug.Log("confirmMoveTimeStamp:" + moveType + faceDirection);
            latestTimeStamp = timeStamp;
            SampleBase newSample = getNewOpSample(moveType, position, faceDirection, moveDirection, inBattle);
            opQueue.push(newSample, timeStamp);

            if (opQueue.lenth() >= 3)
            {
                opQueue.popBeforePosition(timeStamp - 2000); //保留确认前二秒的位置
            }

        }

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
        animator.SetFloat("stopMoveBlend", this.stopMoveBlend);

        animator.SetFloat("X", this.moveDirection.x);
        animator.SetFloat("Z", this.moveDirection.z);

        animator.SetBool("isGrounded", this.isOnGrounded);
        animator.SetBool("InBattle", this.inBattle);
        
        oldTimeStamp = currentTimeStamp;
    }

}
