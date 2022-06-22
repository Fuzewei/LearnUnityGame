namespace KBEngine
{
	using GameLogic;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Const;
	using UnityEngine;
	public partial class Monster : MonsterBase, IServerEntity
	{
		uint p3MoveTimer = 0;

		private void __init__Motion()
		{
		}

		public override void confirmMoveTimeStamp(float timeStamp)
		{
			if (renderEntity)
            {
				float rotateAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
				Dbg.DEBUG_MSG("Monster:confirmMoveTimeStamp:" + timeStamp + moveType + position + direction + rotateAngle);
				//renderEntity.confirmMoveTimeStamp(timeStamp, (MoveConst)moveType, position, direction, moveDirection, Convert.ToBoolean(inBattle));
				renderEntity.confirmMoveTimeStamp(timeStamp, (MoveConst)moveType, position, direction, moveDirection, Convert.ToBoolean(inBattle));
			}

		}

		public void uploadPositionAndRotation(params object[] args)
		{
			float localBeginTimer = (float)args[0];
			float serverStartTimer = (float)args[1];
			float delter = Utils.localTime() - localBeginTimer;
			Dbg.DEBUG_MSG("Monster:p3UpdatePosition:" + renderPosition + " ," + renderRotation + renderMoveDirection);
			cellEntityCall.p3UpdatePosition(serverStartTimer + delter, renderPosition, renderRotation, renderMoveDirection);
		}


		public override void startP3ClientMove(float startTimer)
		{
			if (isBeControl() && p3MoveTimer == 0)
			{
				Dbg.DEBUG_MSG("Monster:startP3ClientMove:" + renderPosition + " ," + renderRotation);
				p3MoveTimer = TimerUtils.addTimer(0.1f, 0.1f, new TimerCallback(uploadPositionAndRotation), Utils.localTime(), startTimer);
			}
		}

		public override void stopP3ClientMove(float endTimer)
		{
			if (p3MoveTimer > 0)
			{
				TimerUtils.cancelTimer(p3MoveTimer);
				p3MoveTimer = 0;
			}
		}


		public bool isBeControl()
		{
			return controlId == KBEngineApp.app.player().id;
		}

	}
}