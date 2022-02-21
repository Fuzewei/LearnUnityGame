using System;
using KBEngine;
using UnityEngine;
using KBEngine.Const;


namespace SwordMaster
{
    public class MoveControlersBase
    {
        public MoveMotor montor;
        public float beginTime;
        public float deltaTime;
        public float xzMoveSpeed
        {
            get {
                return montor.moveSpeed;
            }
            set {
                montor.moveSpeed = value;
            }
        }
        public float yMoveSpeed
        {
            get
            {
                return montor.moveVerticalSpeed;
            }
            set
            {
                montor.moveVerticalSpeed = value;
            }
        }
        public MoveControlersBase(MoveMotor _montor)
        {
            montor = _montor;
        }
    
        public virtual void reset() {
            beginTime = Utils.localTime();
            onReset();
        }
        public virtual void onReset(){}
        public void tick(float _deltaTime) {
            deltaTime = _deltaTime;
        }
        public virtual bool canSetNewMoveType(MoveConst state )
        {
            return true;
        }

        public virtual bool canSetFaceDirection(ref Vector3 rotation)
        {
            return true;
        }

        public virtual void UpdateMoveSpeed() { }
        public virtual Vector3 calcuteDelterPosition() {   
            Vector3 delta = montor.animator.deltaPosition;
            delta.y += yMoveSpeed * Time.deltaTime;
            return delta;
        }
    }

    class NormalIdleControler : MoveControlersBase
    {
        // idle的加速度
        public float acc = 5.5f;

        public NormalIdleControler(MoveMotor _montor) : base(_montor)
        {
        }

        public override void UpdateMoveSpeed() {
            this.xzMoveSpeed = this.xzMoveSpeed - acc * deltaTime;
            this.xzMoveSpeed = Mathf.Max(this.xzMoveSpeed, 0);
            this.yMoveSpeed = -9.8f;
        }
        public override bool canSetFaceDirection(ref Vector3 rotation)
        {
            return false;
        }
    }

    class NormalWalkControler: MoveControlersBase
    {
        public NormalWalkControler(MoveMotor _montor) : base(_montor)
        {

        }
        // The maximum horizontal speed when moving
        public float maxForwardSpeed = 2.21f;
        public float acc = 2.76f;

        override public void UpdateMoveSpeed()
        {
            this.xzMoveSpeed = this.xzMoveSpeed < maxForwardSpeed ? this.xzMoveSpeed + acc * deltaTime : this.xzMoveSpeed - acc * Time.deltaTime;
            this.yMoveSpeed = -100;
        }
    }

    class NormalRunControler : MoveControlersBase
    {
        public NormalRunControler(MoveMotor _montor) : base(_montor)
        {

        }
        // The maximum horizontal speed when moving
        public float maxForwardSpeed = 5.05f;
        public float acc = 10.8f;
        override public void UpdateMoveSpeed()
        {
            this.xzMoveSpeed = this.xzMoveSpeed +acc * deltaTime;
            this.xzMoveSpeed = Mathf.Min(this.xzMoveSpeed, maxForwardSpeed);
            this.yMoveSpeed = -100;
        }
    }


    class NormalJumpControler : MoveControlersBase
    {
        public NormalJumpControler(MoveMotor _montor) : base(_montor)
        {

        }
        // 向上的速度
        public float UpSpeed = 5.5f;
        // 水平空中的减速
        public float accAir = 0.1f;
        // 水平地面的减速
        public float accGround = 20.0f;
        // 阶段
        public int jumpStage = 0; //0是没有收到起跳指令，1准备起跳，2是起跳成功

        public float nextSteteTime; //进入下一阶段的时间
        private MoveConst nextMoveType = MoveConst.Idel;

        public override void onReset() {
            jumpStage = 0;
            nextMoveType = MoveConst.Idel;
        }
        public override bool canSetNewMoveType(MoveConst state)
        {
            if (jumpStage >= 4 && montor.isOnGrounded == true)
            {
                return true;
            }
            nextMoveType = state;
            return false;
        }

        public override bool canSetFaceDirection(ref Vector3 rotation)
        {
            return false;
        }

        public override void UpdateMoveSpeed()
        {  
            if (jumpStage == 0)
            {
                this.yMoveSpeed = 0.5f; //起跳速度
                this.xzMoveSpeed = this.xzMoveSpeed - accAir * deltaTime;
                this.xzMoveSpeed = Mathf.Max(this.xzMoveSpeed, 0);
                jumpStage = 1;
                nextSteteTime = Utils.localTime() + 0.3f;
            }
            else if (jumpStage == 1 && Utils.localTime() > nextSteteTime)
            {
                this.yMoveSpeed = 3.5f; //起跳速度
                this.xzMoveSpeed = this.xzMoveSpeed - accAir * deltaTime;
                this.xzMoveSpeed = Mathf.Max(this.xzMoveSpeed, 0);
                jumpStage = 2;
            }
            else if (jumpStage == 2 && montor.isOnGrounded == false)
            {
                this.yMoveSpeed -= 9.8f * deltaTime;
                this.xzMoveSpeed -= accAir * deltaTime;
                this.xzMoveSpeed = Mathf.Max(this.xzMoveSpeed, 0);
            }
            else if (jumpStage == 2 && montor.isOnGrounded == true)
            {
                jumpStage = 3;
                this.yMoveSpeed = -9.8f;
                this.xzMoveSpeed -= accGround * deltaTime;
                this.xzMoveSpeed = Mathf.Max(this.xzMoveSpeed, 0);
                nextSteteTime = Utils.localTime() + 0.25f;
            }
            else if (jumpStage == 3 && Utils.localTime() <= nextSteteTime)
            {
                this.yMoveSpeed = -9.8f;
                this.xzMoveSpeed -= accGround * deltaTime;
                this.xzMoveSpeed = Mathf.Max(this.xzMoveSpeed, 0);
            }
            else if (jumpStage == 3 && Utils.localTime() > nextSteteTime)
            {
                jumpStage = 4;
                montor.setedMoveType = nextMoveType;
            }
        }

        public override Vector3 calcuteDelterPosition()
        {
            Vector3 delta = this.xzMoveSpeed * deltaTime * montor.globalmoveDirection;
            delta.y += yMoveSpeed * Time.deltaTime;
            return delta;
        }
    }

    //使用技能移动
    class NormalUseSkillControler : MoveControlersBase
    {
        public NormalUseSkillControler(MoveMotor _montor) : base(_montor)
        {

        }
        // skill的加速度
        public float acc = 5.5f;
        public override void UpdateMoveSpeed()
        {
            this.xzMoveSpeed = this.xzMoveSpeed - acc * deltaTime;
            this.xzMoveSpeed = Mathf.Max(this.xzMoveSpeed, 0);
            this.yMoveSpeed = -9.8f;
        }

        public override Vector3 calcuteDelterPosition()
        {
            Vector3 delta = montor.animator.deltaPosition;
            delta.y += yMoveSpeed * Time.deltaTime;
            return delta;
        }
    }

    //被动移动的预表现
    class NormalPreMove : MoveControlersBase
    {
        public NormalPreMove(MoveMotor _montor) : base(_montor)
        {

        }
    }

    //被动移动的实际表现
    class NormalActuallyPreMove : MoveControlersBase
    {
        public NormalActuallyPreMove(MoveMotor _montor) : base(_montor)
        {

        }
    }

}
