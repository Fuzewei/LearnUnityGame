using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLogic;

namespace KBEngine
{
    public enum SkillNodeType
    {
        P1,
        P3,
    }
    public class SkillNodeBase : NodeBase
    {
        protected Avatar avatarOwner;
        public SkillNodeType nodeType = SkillNodeType.P1;
        public SkillNodeBase(float timeStamp) : base(timeStamp)
        {
        }

        public override void OnSetTimeLine()
        {
            avatarOwner = ((skillTimeLine)owneTimeLine).ownerEntity as Avatar; ;
        }

        public virtual void serverCall(object args)
        {

        }

        public virtual void runP1()
        {

        }
        public virtual void runP3()
        {

        }

        public override void Run()
        {
            Dbg.DEBUG_MSG("SkillNodeBase.Run" + this.GetType().Name);

            switch (nodeType)
           {
               case SkillNodeType.P1:
                    runP1();
                    break;
                case SkillNodeType.P3:
                    runP3();
                    break;
               default:
                    Dbg.DEBUG_MSG("error");
                    break;
           }
        }

    }
}
