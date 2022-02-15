using System.Collections;
using System.Collections.Generic;
using KBEngine;


namespace GameLogic
{

    public class TimeLineBase
    {
        
        public float tickTimeStamp; // 上次tick时间
        public int nextIndex;    //下一个node的id
        public float delterTimeStamp ;//相对开始的时间
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
            while (!isFinish() && delterTimeStamp >= getNextTimeStamp())
            {
                doTick();
                nextIndex += 1;
            }
            tickTimeStamp = now;

        }

        public virtual void doTick()
        {
            nodesList[nextIndex].Run();
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
           return nodesList[nextIndex].runTimeStamp;
        }

        public float getNextDelterTime()
        {
            float _t = nodesList[nextIndex].runTimeStamp - delterTimeStamp;
            return _t / speed;
        }

        public virtual void onEnd()
        {
            //Dbg.DEBUG_MSG("timeLine:onEnd");
        }

        public void addNode(NodeBase node)
        {
            node.owneTimeLine = this;
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
            nodesList.Insert(i, node);
        }
        
        
    }


}