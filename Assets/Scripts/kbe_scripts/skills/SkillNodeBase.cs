using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLogic;

namespace KBEngine
{
    public class SkillNodeBase : NodeBase
    {
        protected Avatar avatarOwner;
        public SkillNodeBase(float timeStamp) : base(timeStamp)
        {
        }

        public override void OnSetTimeLine()
        {
            avatarOwner = ((skillTimeLine)owneTimeLine).ownerEntity as Avatar; ;
        }
    }
}
