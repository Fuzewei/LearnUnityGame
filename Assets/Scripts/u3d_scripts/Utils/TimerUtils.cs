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

		private static Dictionary<uint, timerHolder> id2Callback = new Dictionary<uint, timerHolder>();

		public static uint addTimer(float timeout, float interval, TimerCallback callback, params object[] args)
		{
			uint timeId = World.world._addTimer(timeout, interval);
			timerHolder holder = new timerHolder(interval >= 0.001, callback, args);
			id2Callback[timeId] = holder;
			return timeId;
		}
		public static void cancelTimer(uint timeId)
		{
			timerHolder holder = null;
			if (id2Callback.TryGetValue(timeId, out holder))
			{
				id2Callback.Remove(timeId);
				World.world._cancelTimer(timeId);
			}

		}

		public static void onTimer(uint timeId)
		{
			//Debug.Log(id2Callback.Count);
			timerHolder holder = id2Callback[timeId];
			holder.callback(holder.args);
			if (!holder.isForever)
				id2Callback.Remove(timeId);
		}
	}

}
