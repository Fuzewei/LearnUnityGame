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

        public override void doTick()
        {
            nodesList[nextIndex].Run();
        }

        public override void onEnd()
        {
            Dbg.DEBUG_MSG("skillTimeLine:onEnd");
        }
    }
}
