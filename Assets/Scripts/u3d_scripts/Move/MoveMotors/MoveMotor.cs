using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine.Const;
using KBEngine;
using SwordMaster;
using System;

//ȫ���ƶ�����
class MotorSettingParam
{

    public float Gravity = 9.8f;
}


//��������ƶ�����(�������������������ÿ�����)
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class MoveMotor : MonoBehaviour
{

    public float currentTimeStamp = 0;                 //��ǰִ�е���ʱ��
    //a->b�� ��a��֪����Ҫ�ߵĳ�����ƶ�״̬����b���֪����һ�ε��ƶ��ٶ�

    //�趨��Ҫ�ɵ����飬������У�����Ҫ�Ƴ���
    public bool inBattle = false;                                 //ս��״̬
    public bool setedInBattle = false;                                 //ս��״̬
    public int skillId = 0;                                 //ʹ�ü��ܵ�id

    public MoveConst nowMoveType = MoveConst.Idel;                   //��ǰ�ƶ�����
    public MoveConst setedMoveType = MoveConst.Idel;                   //���õ��ƶ�����
    public Vector3 moveDirection = Vector3.forward;               //�ƶ���������(ȫ�֣�����)
    public Vector3 faceDirection = Vector3.forward;                  //�泯����(ȫ��,ŷ����)

    //�趨��Ҫ�ɵ����� end

    //�����ֵ�����ƶ���״̬���Ĳ�����
    public float moveSpeed = 0.0f;                               //��ǰ��ˮƽ���������
    public float faceDiectionSpeed = 0.0f;                               //�泯����ת���ٶȣ�+���ң�
    public float moveVerticalSpeed = 0.0f;                         //��ǰ����ֱ���������

    public float stopMoveBlend = 0.0f;                               //ֹͣ��ϲ���




    //�����ֵ�����ƶ���״̬����end

    public Animator animator;                                        //���������ƶ�(ֻ��������λ��)
    public AnimatorController animatorController;

    CharacterController controller;

    #region �ƶ��Ŀ�����Ͳ���

    public MoveControlersBase currentMoveControler;
    protected NormalWalkControler walkMove;
    protected NormalRunControler runMove;
    protected NormalIdleControler idle;
    protected NormalJumpControler jump;
    protected NormalUseSkillControler useSkill;
    protected NormalServerMove serverMove;

    #endregion

    /*
     * p3ʹ�õ�
     * λ�õ�Ԥ���ɶ���ϵͳ���ˣ�ֻҪ�������
     * ת����Ԥ�������ʷ������ת���ٶ�
     */

    float faceDirectionDiff; //�������

    float moveDirectionSpeed = 0; // �ƶ��ķ���
    float faceDirectionSpeed = 0; // ����y���ת���ٶȣ�����Ԥ��ģ�



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



    //��ȡ��ת�Ƕȣ���ֱ��ʹ��transform.rotation����Ϊunity������0-360�ȣ���Ҫת����-pai ��+pai��
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

    #region ���ø�������

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


    #region ���ù����ƶ��ĸ������Խӿ�

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
    //�˴�Ԥ�⳯��
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
    //             *ǿ��ͬ�������
    //             */
    //            currentTimeStamp = serverTimer;//todo  p1���޸�
    //            setForcePosition(serverSample.position);
    //            positionDiff = new Vector3(0, 0, 0);
    //            faceDirectionSpeed = 0;

    //        }
    //        else
    //        {
    //            /*
    //             *�������ȣ����㱾�ص����position��diff,ƽ�������÷�������λ��
    //             */
    //            calculatePositionDiff();
    //            calculateFacedirectionSpeed();

    //        }
    //    }

    //    localOpQueue.popBeforePosition(recentReciveTimer - 3);

    //}




    public Vector3 getDelterMove()
    {
        Vector3 moveDis = currentMoveControler.calcuteDelterPosition();//����Ҫ�ƶ���ƫ��
        Vector3 old = transform.position;
        controller.Move(moveDis); //����λ��
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
        transform.rotation = Quaternion.Euler(faceDirection); //���ó���
        currentMoveControler.BeforeSwitchMoveControl();//�����л�״̬
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

        transform.rotation = Quaternion.Euler(faceDirection); //���ó���
        currentMoveControler.BeforeSwitchMoveControl();//�����л�״̬
        flushAnimatorParameter();
        if (nowMoveType != setedMoveType)
        {
            currentMoveControler = getMoveControl(setedMoveType);
            currentMoveControler.reset();

        }
        ////����ָ��
        //SampleBase newSample = getNewOpSample(setedMoveType, transform.position, renderRotation, moveDirection, setedInBattle); //���طŵ��ǻ��ȣ������Ǿ���������-pai��pai��
        //localOpQueue.push(newSample, currentTimeStamp);


        nowMoveType = setedMoveType;
        inBattle = setedInBattle;

    }

    public virtual void confirmMoveTimeStamp(float timeStamp, MoveConst moveType, Vector3 position, Vector3 faceDirection, Vector3 moveDirection, bool inBattle)
    {
    }



    //ˢ��״̬������
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
