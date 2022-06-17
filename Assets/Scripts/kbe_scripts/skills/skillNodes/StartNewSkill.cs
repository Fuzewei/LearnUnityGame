using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBEngine
{
    public class StartNewSkill : SkillNodeBase
    {
        public float durationTime;
        public int newSkillId;
        private float beginRegisterTimer;

        public StartNewSkill(float timeStamp, float _durationTime, int newSkillId) : base(timeStamp)
        {
            durationTime = _durationTime;
            this.newSkillId = newSkillId;
        }

        public override void runP1()
        {
            Event.registerIn("useSkill", new Action<int>(useSkill));//使用技能
            Dbg.DEBUG_MSG("StartNewSkill:runP1");
            beginRegisterTimer = Utils.localTime();
        }

        public override void runP3()
        {

        }

        public override void OnDestory()
        {
            Dbg.DEBUG_MSG("StartNewSkill:OnDestory");
            Event.deregisterIn(this);
        }

        public void useSkill(int skillid)
        {
            Dbg.DEBUG_MSG("StartNewSkill:useSkill" + skillid);
            if (Utils.localTime() - beginRegisterTimer > durationTime)
            {
                return;
            }
            
             Event.deregisterIn(this);
             TABLE arg = new TABLE();
             arg.dictOrlist = 0;
            //客户端先行放技能
             ((Avatar)avatarOwner).requestUseSkill(newSkillId);
        }

        public override void serverCall(TABLE args)
        {
            
        }
    }
}

