using System.Collections;
using System.Collections.Generic;
using KBEngine;


namespace GameLogic
{

    public class TimeLineBase
    {
        
        public float tickTimeStamp; // �ϴ�tickʱ��
        public int nextIndex;    //��һ��node��id
        public float delterTimeStamp ;//��Կ�ʼ��ʱ��
        List<NodeBase> nodesList ;

        public float speed ; //�����ٶ�

        TimeLineManager manager;
        public TimeLineBase(TimeLineManager manager)
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

        public void addNode(NodeBase node)
        {
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