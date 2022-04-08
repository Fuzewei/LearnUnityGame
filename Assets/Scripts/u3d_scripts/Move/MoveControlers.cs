﻿using System.Collections.Generic;
using KBEngine;
using UnityEngine;
using KBEngine.Const;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


namespace SwordMaster
{
    [System.Serializable]
    public class rootMotionInfo
    {
        public float x;
        public float y;
        public float z;
        public float timeStamp;
        public rootMotionInfo(float x, float y, float z, float timeStamp)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.timeStamp = timeStamp;

        }
        public rootMotionInfo()
        {
        }
    }
    public class MoveControlersBase
    {
        public static Dictionary<string, List<rootMotionInfo>> rootMotion;

        static MoveControlersBase()
        {
            FileStream file = File.OpenRead("Assets/GreatSword_Animset/Animation/MotionInfo" + "/data.da");
            BinaryFormatter bf = new BinaryFormatter();
            rootMotion =  bf.Deserialize(file) as Dictionary<string, List<rootMotionInfo>>;
            file.Close();
        }



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

        public virtual bool canSetFaceDirection(ref Vector3 rotation)
        {
            return true;
        }

        public virtual void UpdateMoveSpeed() { }

        public virtual void BeforeSwitchMoveControl() { }
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
        public Vector3 jumpDirection ;
        public NormalJumpControler(MoveMotor _montor) : base(_montor)
        {
            jumpDirection = _montor.globalmoveDirection;
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
       

        public override void onReset() {
            jumpStage = 0;
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
            }
        }

        public override void BeforeSwitchMoveControl(){
            if (jumpStage == 4)
            {
                KBEngine.Avatar avatar = montor.GetComponent<GameEntity>().logicEntity as KBEngine.Avatar;
                if (avatar.isPlayer())
                {
                    avatar.playerJumpFinish();
                }
            }
        }

        public override Vector3 calcuteDelterPosition()
        {
            Vector3 delta = this.xzMoveSpeed * deltaTime * jumpDirection;
            delta.y += yMoveSpeed * Time.deltaTime;
            return delta;
        }
    }

    //使用技能移动
    class NormalUseSkillControler : MoveControlersBase
    {
        string aniClipName = null;
        float timeStamp = 0;
        List<rootMotionInfo> clipInfo;
        Vector3 _recent;
        // skill的加速度
        public float acc = 5.5f;


        public NormalUseSkillControler(MoveMotor _montor) : base(_montor)
        {

        }
        public override void onReset()
        {
            aniClipName = null;
            clipInfo = null;
            timeStamp = 0;
        }
        public override void UpdateMoveSpeed()
        {
            this.xzMoveSpeed = this.xzMoveSpeed - acc * deltaTime;
            this.xzMoveSpeed = Mathf.Max(this.xzMoveSpeed, 0);
            this.yMoveSpeed = -9.8f;
        }

        public override Vector3 calcuteDelterPosition()
        {
            var clips = montor.animator.GetCurrentAnimatorClipInfo(0);
            foreach (var item in clips)
            {
                if (item.clip.name != aniClipName)
                {
                    aniClipName = item.clip.name;
                    clipInfo = rootMotion[aniClipName];
                    timeStamp = 0;
                    _recent = Vector3.zero;
                    Dbg.DEBUG_MSG("NormalUseSkillControler: " + item.clip.name + item.clip.length);
                }
                timeStamp = timeStamp > item.clip.length ?  timeStamp - item.clip.length : timeStamp;
               
            }
            rootMotionInfo left = null;
      
            rootMotionInfo right = null;
            foreach (var item in clipInfo)
            {
                if (timeStamp >= item.timeStamp)
                {
                    left = item;
                }

                if (timeStamp <= item.timeStamp)
                {
                    right = item;
                    break;
                }
            }
            //Vector3 delta = montor.animator.deltaPosition;
            //Dbg.DEBUG_MSG("NormalUseSkillControler: timeStamp" + timeStamp);
            Vector3 _new = new Vector3(left.x, 0, left.z);
            Vector3 delta = _new - _recent;
            _recent = _new;
            delta.y += yMoveSpeed * Time.deltaTime;
            timeStamp += Time.deltaTime;
            
            return delta;
        }
    }

    //服务端驱动的移动
    class NormalServerMove : MoveControlersBase
    {
        public NormalServerMove(MoveMotor _montor) : base(_montor)
        {

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

}
