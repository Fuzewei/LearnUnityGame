
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwordMaster;

public class MotionCurve
{
    float totalTime = 0;  //片段总时间
    List<rootMotionInfo> motionInfo;
    bool isLoop; //是否循环
    float endTimeStamp;
    float lastTimeStamp;
    rootMotionInfo lastPosition;
    public MotionCurve(List<rootMotionInfo> info , float total, bool loop = false)
    {
        lastTimeStamp = 0;
        motionInfo = info;
        totalTime = total;
        isLoop = loop;
        var lastmotion = motionInfo.Last();
        endTimeStamp = lastmotion.timeStamp;
    }

    public Vector3 deltaPosition(float oldT, float newT)
    {
        while (oldT >= totalTime)
        {
            oldT -= totalTime;
        }
        while (newT >= totalTime)
        {
            newT -= totalTime;
        }

        var oldM = lerpMotion(oldT);
        var newM = lerpMotion(newT);

        return new Vector3(newM.x - oldM.x, 0, newM.z - oldM.z);
    }

    public rootMotionInfo lerpMotion(float timeStamp)
    {
        rootMotionInfo left = default;
        rootMotionInfo right = default;
        if (timeStamp >= endTimeStamp)
        {
            return motionInfo.Last();
        }

        foreach (var item in motionInfo)
        {
            if (timeStamp >= item.timeStamp)
            {
                left = item;
                right = item;
            }

            if (timeStamp <= item.timeStamp)
            {
                right = item;
                break;
            }
        }
        if (left == null)//比最小的还小
        {
            return new rootMotionInfo(0, 0, 0, timeStamp);
        }
        float percent = (timeStamp - left.timeStamp) / (right.timeStamp - left.timeStamp);
        return new rootMotionInfo(Mathf.Lerp(left.x, right.x, percent), Mathf.Lerp(left.y, right.y, percent), Mathf.Lerp(left.z, right.z, percent), timeStamp);
    }

}

