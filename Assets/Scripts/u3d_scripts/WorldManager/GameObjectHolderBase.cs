using UnityEngine;
using KBEngine;
using KBEngine.Const;
using System.Collections;
using System;
using System.Xml;
using System.Collections.Generic;


namespace KBEngine.GameobjectHolder
{
    public class ObjectArgeBase
    {
    }

    public delegate void noticeCallback(ObjectArgeBase arge);


    public class GameobjectHolderBase: MonoBehaviour
    {
        public uint objId;
        public float destoryTimestamp = float.MaxValue;


        private event noticeCallback registerCallBack;
        public void init(uint _id, float _destoryTimestamp)
        {
            objId = _id;
            destoryTimestamp = _destoryTimestamp;
        }

        public void addNoticeCallback(noticeCallback cb)
        {
            registerCallBack += cb;
        }

        public void removeNoticeCallback(noticeCallback cb)
        {
            registerCallBack -= cb;
        }

        public void notice(ObjectArgeBase args)
        {
            if (registerCallBack != null)
            {
                registerCallBack(args);
            }
        }



    }

}
