using System.Collections;
using System.Collections.Generic;
using KBEngine;
namespace GameLogic
{
    public class TimeLineManager
    {
        private uint nodeUUid = 0;
        private Dictionary<uint, TimeLineBase> timeLines = new Dictionary<uint, TimeLineBase>();
        private Dictionary<uint, Dictionary<int, object>> timeLinesData = new Dictionary<uint, Dictionary<int, object>>();

        private float nextDelterTime = float.MaxValue;
        private uint updateTimerId = 0;

        public TimeLineManager()
        { }

        public uint addTimeLine(TimeLineBase timeLine)
        {
            uint _id = getUUid();
            timeLine.uuid = _id;
            timeLine.setManager(this);
            timeLine.start();
            timeLines.Add(_id, timeLine);
            if (timeLine.getNextDelterTime() < nextDelterTime)
            {
                if (updateTimerId > 0)
                {
                    TimerUtils.cancelTimer(updateTimerId);
                }

                nextDelterTime = timeLine.getNextDelterTime(); 
                updateTimerId = TimerUtils.addTimer(nextDelterTime, 0, new TimerCallback(onTime));
            }
            
            return _id;
        }

        public TimeLineBase getTimeLine(uint uuid)
        {
            TimeLineBase ans;
            if (timeLines.TryGetValue(uuid, out ans))
            {
                return ans;
            }
            else
                return null;       
        }

        public void onTime(params object[] args)
        {
            updateTimerId = 0;
            List<uint> delete = new List<uint>();
            foreach (var item in timeLines)
            {
                item.Value.tick();
                if (item.Value.isFinish())
                {
                    delete.Add(item.Key);
                }
            }
            foreach (var item in delete)
            {
                timeLines[item].onEnd();
                timeLines.Remove(item);
            }
            nextDelterTime = getNextTimer();
            if (nextDelterTime < float.MaxValue)
            {
                updateTimerId = TimerUtils.addTimer(nextDelterTime, 0, new TimerCallback(onTime));
            }
        }

        public uint getUUid()
        {
            return nodeUUid++;
        }

        public float getNextTimer()
        {
            float minDelter = float.MaxValue;
            foreach (var item in timeLines)
            {
                if (item.Value.getNextDelterTime() < minDelter)
                {
                    minDelter = item.Value.getNextDelterTime();
                }
            }
            return minDelter;
        }


    }
}