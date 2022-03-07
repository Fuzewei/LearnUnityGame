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
    private MotorSettingParam moveParam = new MotorSettingParam();
    //�����������
    public SampleQueue localOpQueue; //���صĲ�������
    public SampleQueue serverOpQueue; //�յ��ķ���˲�������
    public float currentTimeStamp = 0;                 //��ǰִ�е���ʱ��

    public float confirmTimeStamp = 0;                 //������ȷ�ϵ�ʱ��

    public bool isSyncSource = true;
    //����������� end

    //a->b�� ��a��֪����Ҫ�ߵĳ�����ƶ�״̬����b���֪����һ�ε��ƶ��ٶ�

    //�趨��Ҫ�ɵ����飬������У�����Ҫ�Ƴ���
    public bool inBattle = false;                                 //ս��״̬
    public bool setedInBattle = false;                                 //ս��״̬
    public int skillId = 0;                                 //ʹ�ü��ܵ�id

    public MoveConst nowMoveType = MoveConst.Idel;                   //��ǰ�ƶ�����
    public MoveConst setedMoveType = MoveConst.Idel;                   //���õ��ƶ�����
    public Vector3 moveDirection = Vector3.forward;               //�ƶ�����(�ֲ�,����)
    public Vector3 faceDirection = Vector3.zero;                  //�泯����(ȫ��,ŷ����)

    public Vector3? forcePosition;                  //�泯����(ȫ��,ŷ����)
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

    #region �ƶ��Ŀ�����
    public MoveControlersBase currentMoveControler;
    NormalWalkControler walkMove;
    NormalRunControler runMove;
    NormalIdleControler idle;
    NormalJumpControler jump;
    NormalUseSkillControler useSkill;
    NormalServerMove serverMove;
    #endregion

    /*
     * p3ʹ�õ�
     * λ�õ�Ԥ���ɶ���ϵͳ���ˣ�ֻҪ�������
     * ת����Ԥ�������ʷ������ת���ٶ�
     */
    Vector3 positionDiff; //�ƶ���λ������������
    float faceDirectionDiff; //�������

    float moveDirectionSpeed = 0; // �ƶ��ķ���
    float faceDirectionSpeed = 0; // ����y���ת���ٶȣ�����Ԥ��ģ�
    public SampleQueue forecastOpQueue; //Ԥ��Ķ���

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


    //unity������0-360�ȣ���Ҫת����-pai ��+pai
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
                 *ǿ��ͬ�������
                 */
                currentTimeStamp = serverTimer;//todo  p1���޸�
                setForcePosition(serverSample.position);
                positionDiff = new Vector3(0, 0, 0);
                faceDirectionSpeed = 0;

            }
            else
            {
                /*
                 *�������ȣ����㱾�ص����position��diff,ƽ�������÷�������λ��
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
        //������ת��Ԥ��
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
        //�ƶ����������е���
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
        Vector3 moveDis = currentMoveControler.calcuteDelterPosition();//����Ҫ�ƶ���ƫ��
        Vector3 old = transform.position;
        controller.Move(moveDis); //����λ��
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


        //Vector3 moveDis = currentMoveControler.calcuteDelterPosition();//����λ��ƫ��(�����ǰ)
        //controller.Move(moveDis); //����λ��


        faceDirection.y += faceDirectionSpeed * Time.deltaTime;

        if (faceDirectionDiff!=0)
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
        //����ָ��
        SampleBase newSample = getNewOpSample(setedMoveType, transform.position, renderRotation, moveDirection, setedInBattle); //���طŵ��ǻ��ȣ������Ǿ���������-pai��pai��
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


    //�յ�ȷ��λ����Ϣ
    public void confirmMoveTimeStamp(float timeStamp, MoveConst moveType, Vector3 position, Vector3 faceDirection, Vector3 moveDirection, bool inBattle)
    {
        SampleBase newSample = getNewOpSample(moveType, position, faceDirection, moveDirection, inBattle);
        serverOpQueue.push(newSample, timeStamp);
        forecastOpQueue.push(newSample, timeStamp);
        forecastOpQueue.popBeforePosition(timeStamp - 3);
        Debug.Log("confirmMoveTimeStamp" + faceDirection);
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

        animator.SetFloat("X", this.moveDirection.x);
        animator.SetFloat("Z", this.moveDirection.z);

        animator.SetBool("isGrounded", this.isOnGrounded);
        animator.SetBool("InBattle", this.inBattle);
    }

}
