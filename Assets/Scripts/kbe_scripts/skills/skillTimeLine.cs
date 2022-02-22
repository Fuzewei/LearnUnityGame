using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLogic;

namespace KBEngine
{
    public class skillTimeLine: TimeLineBase
    {
        public Entity ownerEntity;
        
        public skillTimeLine(Entity ownerEntity) :base()
        {
            this.ownerEntity = ownerEntity;
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
        public void callServer(uint nodeId, TABLE arg)
        {
            Dbg.DEBUG_MSG("skillTimeLine:callServer");
            Avatar avatar = ownerEntity as Avatar;
            if (avatar!= null)
            {
                avatar.cellEntityCall.skillNodeCallServer(uuid, (int)nodeId, arg);
            }
        }

        public void callFromServer(int nodeId, TABLE arg)
        {
            SkillNodeBase node = nodesList[nodeId] as SkillNodeBase;
            node.serverCall(arg);
        }

    }
}
