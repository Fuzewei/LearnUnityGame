using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;

namespace GameLogic
{
    public class NodeBase
    {
        public float runTimeStamp;
        public float priority = 0;
        public int nodeId;
        public NodeBase(float timeStamp)
        {
            this.runTimeStamp = timeStamp;
        }

        public virtual void Run()
        {
            Debug.Log("NodeBase: " + runTimeStamp +  "   "  + Utils.localTime());
        }
    }

}