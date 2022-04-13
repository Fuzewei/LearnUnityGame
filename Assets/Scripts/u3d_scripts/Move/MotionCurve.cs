
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

    rootMotionInfo lastPosition;
    public MotionCurve(List<rootMotionInfo> info , float total, bool loop = false)
    {
        motionInfo = info;
        totalTime = total;
        isLoop = loop;
    }

    public Vector3 deltaPosition(float oldT, float newT)
    {
        var oldM = lerpMotion(oldT);
        var newM = lerpMotion(newT);

        return new Vector3(newM.x - oldM.x, 0, newM.z - oldM.z);
    }

    public rootMotionInfo lerpMotion(float timeStamp)
    {
        rootMotionInfo left = default;
        rootMotionInfo right = default;
        rootMotionInfo ans = new rootMotionInfo();
        rootMotionInfo accPosition = new rootMotionInfo();
        if (isLoop)
        {
            while (timeStamp >= totalTime)
            {
                accPosition = accPosition + motionInfo.Last();
                timeStamp -= totalTime;
            }
        }
        else
        {
            if (timeStamp >= totalTime)
            {
                ans = motionInfo.Last();
                ans.timeStamp = timeStamp;
                return ans;
            }
        }
        foreach (var item in motionInfo) //查找左右位置
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

        if (left == null)//时间点左边没有
        { 
            return accPosition ;
        }
        if(right.timeStamp == left.timeStamp) //时间点右边没了
        {
            ans = accPosition + left;
            ans.timeStamp = timeStamp;
            return ans;
        }
        else
        {
            float percent = (timeStamp - left.timeStamp) / (right.timeStamp - left.timeStamp);
            var _t = new rootMotionInfo(Mathf.Lerp(left.x, right.x, percent), Mathf.Lerp(left.y, right.y, percent), Mathf.Lerp(left.z, right.z, percent), timeStamp);
            ans = accPosition + _t;
            ans.timeStamp = timeStamp;
            return ans;
        }

        
    }

}

