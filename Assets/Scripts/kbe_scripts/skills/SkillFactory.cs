namespace KBEngine
{
    using GameLogic;
    using UnityEngine;
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public static class SkillFactory
    {
        public static skillTimeLine getTimeLineById(Entity entity, int timeLineId, SkillNodeType nodeType = SkillNodeType.P1)
        {
            skillTimeLine line = new skillTimeLine(entity);
            switch (timeLineId)
            {
                case 1:
                    PlayerAnimationNode NodeBase1 = new PlayerAnimationNode(0.0f, "Attack.GreatSword_Attack01");
                    NodeBase1.nodeType = nodeType;
                    line.addNode(NodeBase1);
                    CommonAttack NodeBase2 = new CommonAttack(0.3f);
                    NodeBase2.nodeType = nodeType;
                    line.addNode(NodeBase2);
                    TimeLineEndNode NodeBase3 = new TimeLineEndNode(1.5f);
                    NodeBase3.nodeType = nodeType;
                    line.addNode(NodeBase3);
                    break;

                case 2:

                default:
                    break;
            }
            
           
            return line;
        }




    }
}
