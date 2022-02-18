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
        public TimeLineBase owneTimeLine;
        public NodeBase(float timeStamp)
        {
            this.runTimeStamp = timeStamp;
        }

        public virtual void Run()
        {
            Debug.Log("NodeBase: Run" + runTimeStamp +  "   "  + Utils.localTime());
        }

        public virtual void OnDestory()
        {
            Debug.Log("NodeBase:OnDestory " + runTimeStamp + "   " + Utils.localTime());
        }

        public virtual void OnSetTimeLine()
        {
            Debug.Log("NodeBase:OnSetTimeLine " + runTimeStamp + "   " + Utils.localTime());
        }
    }

}