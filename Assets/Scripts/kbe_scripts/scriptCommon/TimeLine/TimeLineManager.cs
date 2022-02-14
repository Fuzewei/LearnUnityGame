using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameLogic
{
    public class TimeLineManager
    {
        private int nodeUUid = 0;
        private Dictionary<int, TimeLineBase> timeLines = new Dictionary<int, TimeLineBase>();
        TimeLineManager()
        { }
    }
}