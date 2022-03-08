using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBEngine
{
    static class Utils
    {
        public static long serverTime()
        {
            DateTime now = System.DateTime.Now;
            long timeStamp =(now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
            return timeStamp;
        }

        //本地时间，同步的时间戳用的这个
        public static float localTime()
        {
            return Time.realtimeSinceStartup;
        }
        public static Vector3 directAngleLerp(Vector3 v_1, Vector3 v_2, float persentage)
        {
            Quaternion t_1 = Quaternion.Euler(v_1);
            Quaternion t_2 = Quaternion.Euler(v_2);
            Quaternion ans = Quaternion.Lerp(t_1, t_2, persentage);
            return ans.eulerAngles;
        }
        //循环范围计算最两者的距离
        public static float cycleMin(float min, float max, float old, float _new){
            float len = max - min;
            float halfLen = len / 2.0f;
            if (_new - old < -halfLen)
            {
                _new = len + _new ;
            }
            else if (_new - old >= halfLen)
            {
                _new = _new - len;
            }
            float y_delter = _new - old;
            return y_delter;
        }

    }



}
