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
        Monster,
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

        //服务端发来的信息
        public virtual void serverCall(TABLE args)
        {

        }

        public virtual void runP1()
        {

        }

        public virtual void runP3()
        {

        }

        public virtual void runMonster()
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

                case SkillNodeType.Monster:
                    runMonster();
                    break;
                default:
                    Dbg.DEBUG_MSG("error");
                    break;
            }
        }

    }
}
