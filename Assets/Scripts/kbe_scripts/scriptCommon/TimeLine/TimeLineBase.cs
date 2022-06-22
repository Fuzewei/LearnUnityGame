using System.Collections;
using System.Collections.Generic;
using KBEngine;
using System;


namespace GameLogic
{

    public class TimeLineBase
    {
        public uint uuid;
        public float tickTimeStamp; // 上次tick时间
        private int nextIndex;    //下一个node的id
        public float delterTimeStamp ;//相对开始的时间（不考虑加速）
        protected List<NodeBase> nodesList ;

        public float speed ; //播放速度

        TimeLineManager manager;
        public TimeLineBase()
        {
            init(); 
        }
        public virtual void init()
        {
            tickTimeStamp = 0;
            nextIndex = 0;
            delterTimeStamp = 0;
            nodesList = new List<NodeBase>();
            speed = 1.0f;
        }

        public void setUUID(uint uuid)
        {
            this.uuid = uuid;
        }
        public void setManager(TimeLineManager m)
        {
            manager = m;
        }
        public virtual void start()
        {
            tickTimeStamp = Utils.localTime();
            setSpeed(1);
            tick();
            
        }
        public void tick()
        {
            float now = Utils.localTime();
            delterTimeStamp += (now - tickTimeStamp) * speed;
            tickTimeStamp = now;
            while (tickCheck())
            {
                doTick(nextIndex);
                nextIndex += 1;
            }
        }
        private bool tickCheck()
        {
            return !isFinish() && delterTimeStamp - getNextTimeStamp() + 0.01 >= 0;
        }

        public virtual void doTick(int index)
        {
            nodesList[index].Run();
        }

        public virtual bool isFinish()
        {
            return nodesList.Count <= nextIndex;
        }

        public void setSpeed(float newSpeed)
        {
            speed = newSpeed;
        }

        public float getNextTimeStamp()
        {
            var realTime = nodesList[nextIndex].runTimeStamp - delterTimeStamp + tickTimeStamp;
            float now = Utils.localTime();
            return Math.Max(realTime - now, 0);
        }

        public float getNextDelterTime()
        {
            float _t = getNextTimeStamp() - delterTimeStamp;
            return _t / speed;
        }

        public virtual void onEnd()
        {
            //Dbg.DEBUG_MSG("timeLine:onEnd");
        }

        public void addNode(NodeBase node)
        {
            node.owneTimeLine = this;
            node.OnSetTimeLine();
            int i = 0;
            for (; i < nodesList.Count; i++)
            {
                var _t = nodesList[i];
                if (_t.runTimeStamp >= node.runTimeStamp)
                {
                    if (_t.runTimeStamp == node.runTimeStamp )
                    {
                        if ( _t.priority < node.priority)
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }

                    }
                    break;
                }
            }
            node.nodeId = i;
            nodesList.Insert(i, node);
        }
        
        
    }


}