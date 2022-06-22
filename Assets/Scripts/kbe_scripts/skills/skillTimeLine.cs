using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLogic;

namespace KBEngine
{
    public class skillTimeLine: TimeLineBase
    {
        public IServerEntity ownerEntity;
        public int skillId;

        public skillTimeLine(IServerEntity ownerEntity) :base()
        {
            this.ownerEntity = ownerEntity;
        }

        public void setSkillId(int skillId)
        {
            this.skillId = skillId;
        }

        public override void start()
        {
            base.start();

        }

        public override void doTick(int index)
        {
            nodesList[index].Run();
        }

        public override void onEnd()
        {
            Dbg.DEBUG_MSG("skillTimeLine:onEnd");
            foreach (var item in nodesList)
            {
                item.OnDestory();
            }
        }

        public void callServer(int nodeId, TABLE arg)
        {
            Dbg.DEBUG_MSG("skillTimeLine:callServer");
            Avatar avatar = ownerEntity as Avatar;
            if (avatar!= null)
            {
                avatar.cellEntityCall.skillNodeCallServer(uuid, nodeId, arg);
            }
        }

        public void callFromServer(int nodeId, TABLE arg)
        {
            SkillNodeBase node = nodesList[nodeId] as SkillNodeBase;
            node.serverCall(arg);
        }

    }
}
