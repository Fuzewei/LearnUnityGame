﻿using System;
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
            Avatar a = ((skillTimeLine)owneTimeLine).ownerEntity as Avatar;
            a.renderEntity.palyerAnimation(animName);

        }

        public override void runP3()
        {
            runP1();
        }
    }
}
