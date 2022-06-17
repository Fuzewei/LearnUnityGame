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
		Int32? controlId = null;

		private void __init__Motion()
		{
		}

		public override void confirmMoveTimeStamp(float timeStamp)
		{
			Dbg.DEBUG_MSG("Monster：confirmMoveTimeStamp:"  + moveType);
			float rotateAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
			Vector3 faceDirection = new Vector3(0, rotateAngle, 0);
			renderEntity.confirmMoveTimeStamp(timeStamp, (MoveConst)moveType, position, faceDirection, direction, Convert.ToBoolean(inBattle));
		}

		public void uploadPositionAndRotation(params object[] args)
		{
			float localBeginTimer = (float)args[0];
			float serverStartTimer = (float)args[1];
			float delter = Utils.localTime() - localBeginTimer;
			Dbg.DEBUG_MSG("p3UpdatePosition:" + renderPosition + " ," + renderRotation);
			cellEntityCall.p3UpdatePosition(serverStartTimer + delter, renderPosition, renderRotation, renderMoveDirection);
		}


		public override void startP3ClientMove(float startTimer, Int32 _controlId)
		{
			controlId = _controlId;
			if (controlId == KBEngineApp.app.player().id)
			{
				p3MoveTimer = TimerUtils.addTimer(0.1f, 0.1f, new TimerCallback(uploadPositionAndRotation), Utils.localTime(), startTimer);
			}
		}

		public override void stopP3ClientMove(float endTimer)
		{
			if (p3MoveTimer > 0)
			{
				TimerUtils.cancelTimer(p3MoveTimer);
				p3MoveTimer = 0;
				controlId = null;
			}
		}


		public bool isBeControl()
		{
			return controlId == KBEngineApp.app.player().id;
		}

	}
}