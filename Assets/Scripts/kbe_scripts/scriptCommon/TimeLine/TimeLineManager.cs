using System.Collections;
using System.Collections.Generic;
using KBEngine;
namespace GameLogic
{
    public class TimeLineManager
    {
        private int nodeUUid = 0;
        private Dictionary<int, TimeLineBase> timeLines = new Dictionary<int, TimeLineBase>();
        private Dictionary<int, Dictionary<int, object>> timeLinesData = new Dictionary<int, Dictionary<int, object>>();

        private float nextDelterTime = float.MaxValue;
        private uint updateTimerId = 0;

        public TimeLineManager()
        { }

        public int addTimeLine(TimeLineBase timeLine)
        {
            int _id = getUUid();
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

        public void onTime(params object[] args)
        {
            updateTimerId = 0;
            List<int> delete = new List<int>();
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

        public int getUUid()
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