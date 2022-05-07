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

		public override void confirmMoveTimeStamp(float timeStamp)
		{
			Dbg.DEBUG_MSG("confirmMoveTimeStamp:" + timeStamp);
			float rotateAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
			Vector3 faceDirection = new Vector3(0, rotateAngle, 0);
			renderEntity.confirmMoveTimeStamp(timeStamp, (MoveConst)moveType, position, faceDirection, direction, Convert.ToBoolean(inBattle));
		}
	}
}