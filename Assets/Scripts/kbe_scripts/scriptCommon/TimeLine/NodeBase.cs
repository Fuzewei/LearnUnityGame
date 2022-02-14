using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public abstract class NodeBase
    {
        public float runTimeStamp;
        public float priority = 0;
        public int nodeId;
        public NodeBase(int timeStamp)
        {
            this.runTimeStamp = timeStamp;
        }

        public abstract void Run();
    }

}