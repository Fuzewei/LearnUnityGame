using System.Collections.Generic;
using KBEngine;
using UnityEngine;
using KBEngine.Const;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;


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

        public static rootMotionInfo operator - (rootMotionInfo a, rootMotionInfo b)
        {
            return new rootMotionInfo(a.x - b.x, a.y - b.y, a.z - b.z, a.timeStamp - b.timeStamp);
        }

        public static rootMotionInfo operator +(rootMotionInfo a, rootMotionInfo b)
        {
            return new rootMotionInfo(a.x + b.x, a.y + b.y, a.z + b.z, a.timeStamp + b.timeStamp);
        }
    }
    public class MoveControlersBase
    {
        public static Dictionary<string, List<rootMotionInfo>> rootMotion;

        static MoveControlersBase()
        {
            rootMotion = new Dictionary<string, List<rootMotionInfo>>();
            using (StreamReader streamReader = new StreamReader(Application.streamingAssetsPath + "/data.json"))
            {
                while (true)
                {
                    string name = streamReader.ReadLine();
                    if (name == null ||name.Length == 0)
                    {
                        break;
                    }
                    string[] clipInfo = streamReader.ReadLine().Split(' ');
                    List<rootMotionInfo> lInfo = new List<rootMotionInfo>();
                    for (int i = 0; i < clipInfo.Length - 1; i += 4)
                    {
                        float x = Convert.ToSingle(clipInfo[i]);
                        float y = Convert.ToSingle(clipInfo[i + 1]);
                        float z = Convert.ToSingle(clipInfo[i + 2]);
                        float timeStamp = Convert.ToSingle(clipInfo[i + 3]);
                        rootMotionInfo item = new rootMotionInfo(x, y, z, timeStamp);
                        lInfo.Add(item);
                    }
                    rootMotion[name] = lInfo;
                }
               
               
            }

        }



        public MoveMotor montor;
        public float beginTime;
        public float deltaTime;
        public float _lastTickTime;
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
            _lastTickTime = Utils.localTime();
            onReset();
        }

        public virtual void onReset(){}

        public void tick() {
            deltaTime = Utils.localTime() - _lastTickTime;
            _lastTickTime = Utils.localTime();
        }

        public virtual bool canSetFaceDirection(ref Vector3 rotation)
        {
            return true;
        }

        public virtual void UpdateMoveSpeed() { }

        public virtual void BeforeSwitchMoveControl() { }
        public virtual Vector3 calcuteDelterPosition() {   
            Vector3 delta = montor.animator.deltaPosition;
            delta.y += yMoveSpeed * deltaTime;
            return delta;
        }
    }

    public class NormalIdleControler : MoveControlersBase
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

        //public override Vector3 calcuteDelterPosition()
        //{
        //    float accTime = Mathf.Abs(this.xzMoveSpeed) / acc;
        //    accTime = Mathf.Min(accTime, deltaTime);
        //    float l = this.xzMoveSpeed * deltaTime - 0.5f * acc * accTime * accTime;

        //    Vector3 delta = new Vector3(0, 0, l);
        //    delta.y += yMoveSpeed * deltaTime;


        //    return Quaternion.LookRotation(montor.globalMoveDirection) * delta;
        //}
    }

    public class NormalWalkControler: MoveControlersBase
    {
        
        public NormalWalkControler(MoveMotor _montor) : base(_montor)
        {
           
        }
        // The maximum horizontal speed when moving
        public float maxForwardSpeed = 2.21f;
        public float acc = 3.76f;

        override public void UpdateMoveSpeed()
        {
            if (this.xzMoveSpeed <= maxForwardSpeed)
            {
                this.xzMoveSpeed = Mathf.Min(this.xzMoveSpeed + acc * deltaTime, maxForwardSpeed);
            }
            else
            {
                this.xzMoveSpeed = Mathf.Max(this.xzMoveSpeed - acc * deltaTime, maxForwardSpeed);
            }
            this.yMoveSpeed = -100;
        }

        //public override Vector3 calcuteDelterPosition()
        //{
        //    float accTime = Mathf.Abs(maxForwardSpeed - this.xzMoveSpeed) / acc;
        //    accTime = Mathf.Min(accTime , deltaTime);
        //    float l = this.xzMoveSpeed * deltaTime + 0.5f * acc * accTime * accTime;
            
        //    Vector3 delta = new Vector3(0, 0, l);
        //    delta.y += yMoveSpeed * deltaTime;
        //    return Quaternion.LookRotation(montor.globalMoveDirection) * delta;
        //}
    }

    public class NormalRunControler : MoveControlersBase
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

        public override Vector3 calcuteDelterPosition()
        {
            float accTime = Mathf.Abs(maxForwardSpeed - this.xzMoveSpeed) / acc;
            accTime = Mathf.Min(accTime, deltaTime);
            float l = this.xzMoveSpeed * deltaTime + 0.5f * acc * accTime * accTime;

            Vector3 delta = new Vector3(0, 0, l);
            delta.y += yMoveSpeed * deltaTime;
            return Quaternion.LookRotation(montor.moveDirection) * delta;
        }
    }


    public class NormalJumpControler : MoveControlersBase
    {
        public Vector3 jumpDirection ;
        // 向上的速度
        public float UpSpeed = 5.5f;
        // 水平空中的减速
        public float accAir = 0.1f;
        // 水平地面的减速
        public float accGround = 20.0f;
        // 阶段
        public int jumpStage = 0; //0是没有收到起跳指令，1准备起跳，2是起跳成功

        public float nextSteteTime; //进入下一阶段的时间

        public NormalJumpControler(MoveMotor _montor) : base(_montor)
        {
            jumpDirection = montor.moveDirection;

        }
        public override void onReset() {
            jumpStage = 0;
            jumpDirection = montor.moveDirection;
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
            delta.y += yMoveSpeed * deltaTime;
            return delta;
        }
    }

    //使用技能的移动
    public class NormalUseSkillControler : MoveControlersBase
    {
        string aniClipName = null;
        float timeStamp = 0;
        MotionCurve currentCurve;
        // skill的加速度
        public float acc = 5.5f;


        public NormalUseSkillControler(MoveMotor _montor) : base(_montor)
        {

        }
        public override void onReset()
        {
            aniClipName = null;
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
                    Dbg.DEBUG_MSG("NormalUseSkillControler: " + aniClipName);
                    currentCurve = new MotionCurve(rootMotion[aniClipName], item.clip.length);
                    timeStamp = 0;
                }
            }

            Vector3 delta = currentCurve.deltaPosition(timeStamp, timeStamp + deltaTime);
            delta.y += yMoveSpeed * deltaTime;

            timeStamp += deltaTime;
            return Quaternion.Euler(montor.faceDirection) * delta;
        }
    }

    //服务端驱动的移动
    public class NormalServerMove : MoveControlersBase
    {
        public NormalServerMove(MoveMotor _montor) : base(_montor)
        {

        }
    }


}
