using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KBEngine
{
	public delegate void TimerCallback(params object[] args);
	class TimerUtils
    {
		private class timerHolder
		{
			public timerHolder(bool forever, TimerCallback _callback, object[] _args)
			{
				callback = _callback;
				args = _args;
				isForever = forever;
			}
			public TimerCallback callback;
			public object[] args;
			public bool isForever;
		}

		private static Dictionary<UInt32, timerHolder> id2Callback = new Dictionary<UInt32, timerHolder>();

		public static UInt32 addTimer(float timeout, float interval, TimerCallback callback, params object[] args)
		{
			UInt32 timeId = World.world._addTimer(timeout, interval);
			timerHolder holder = new timerHolder(interval >= 0.001, callback, args);
			id2Callback[timeId] = holder;
			return timeId;
		}
		public static void cancelTimer(UInt32 timeId)
		{
			timerHolder holder = null;
			if (id2Callback.TryGetValue(timeId, out holder))
			{
				id2Callback.Remove(timeId);
				World.world._cancelTimer(timeId);
			}

		}

		public static void onTimer(UInt32 timeId)
		{
			//Debug.Log(id2Callback.Count);
			timerHolder holder = id2Callback[timeId];
			holder.callback(holder.args);
			if (!holder.isForever)
				id2Callback.Remove(timeId);
		}
	}

}
