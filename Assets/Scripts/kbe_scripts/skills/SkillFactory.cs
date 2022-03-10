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
                    StartNewTimeLine NodeBase3 = new StartNewTimeLine(1.0f, 0.4f, 2);
                    NodeBase3.nodeType = nodeType;
                    line.addNode(NodeBase3);
                    TimeLineEndNode NodeBase4 = new TimeLineEndNode(1.5f);
                    NodeBase4.nodeType = nodeType;
                    line.addNode(NodeBase4);
                    break;

                case 2:
                    PlayerAnimationNode NodeBase6 = new PlayerAnimationNode(0.0f, "Attack.GreatSword_Attack02");
                    NodeBase6.nodeType = nodeType;
                    line.addNode(NodeBase6);
                    CommonAttack NodeBase7 = new CommonAttack(0.3f);
                    NodeBase7.nodeType = nodeType;
                    line.addNode(NodeBase7);
                    StartNewTimeLine NodeBase8 = new StartNewTimeLine(1.0f, 0.4f, 2);
                    NodeBase8.nodeType = nodeType;
                    line.addNode(NodeBase8);
                    TimeLineEndNode NodeBase9 = new TimeLineEndNode(1.5f);
                    NodeBase9.nodeType = nodeType;
                    line.addNode(NodeBase9);
                    break;
                case 3:
                    break;

                default:
                    break;
            }
            
           
            return line;
        }




    }
}
