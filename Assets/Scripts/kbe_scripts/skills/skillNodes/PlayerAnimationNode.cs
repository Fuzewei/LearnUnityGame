using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBEngine
{
    public class PlayerAnimationNode : SkillNodeBase
    {
        public string animName;
        public PlayerAnimationNode(float timeStamp, string name) : base(timeStamp)
        {
            animName = name;
        }

        public override void Run()
        {
            Avatar a = ((skillTimeLine)owneTimeLine).ownerEntity as Avatar;
            a.renderEntity.palyerAnimation(animName);
        }
    }
}
