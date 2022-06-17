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

        public override void runP1()
        {
            avatarOwner.renderEntity.palyerAnimation(animName);
        }

        public override void runP3()
        {
            runP1();
        }

        public override void runMonster()
        {
            runP1();
        }
    }
}
