using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBEngine
{
    public class StartNewTimeLine : SkillNodeBase
    {
        public float durationTime;
        public int newTimeLineId;
        private float beginRegisterTimer;

        public StartNewTimeLine(float timeStamp, float _durationTime, int timeLineId) : base(timeStamp)
        {
            durationTime = _durationTime;
            newTimeLineId = timeLineId;
        }

        public override void runP1()
        {
            Event.registerIn("useSkill", new Action<int>(useSkill));//使用技能
            beginRegisterTimer = Utils.localTime();
        }

        public override void runP3()
        {

        }

        public override void OnDestory()
        {
            Event.deregisterIn(this);
        }

        public void useSkill(int skillid)
        {
            Dbg.DEBUG_MSG("StartNewTimeLine:" + skillid);
            if (Utils.localTime() - beginRegisterTimer > durationTime)
            {
                return;
            }
            
             Event.deregisterIn(this);
             TABLE arg = new TABLE();
             arg.dictOrlist = 0;

             var uuid = avatarOwner.timeLineManager.getUUid();
             avatarOwner.preUseSkill.startTimeLine(newTimeLineId, uuid);
             arg.values.Add(newTimeLineId);
             arg.values.Add(uuid);
             ((skillTimeLine)owneTimeLine).callServer(2, arg);
            
        }

        public override void serverCall(TABLE args)
        {
            if (nodeType == SkillNodeType.P1)
            {
                return;
            }

            int newTimeLineId = (int)args.values[0];
            uint uuid = (uint)args.values[1];

            skillTimeLine line = SkillFactory.getTimeLineById(avatarOwner, newTimeLineId, SkillNodeType.P3);
            avatarOwner.timeLineManager.addTimeLine(uuid, line);
        }
    }
}

