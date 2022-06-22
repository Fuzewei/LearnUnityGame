namespace KBEngine
{
	using GameLogic;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Const;
	public partial class Monster : MonsterBase, IServerEntity
	{

		public override void randomWalk(PATH_POINTS pathPoints)
		{
			Dbg.DEBUG_MSG("randomWalk:" + pathPoints);
			renderEntity.setAiMovePath(pathPoints);
			renderEntity.setAiMovType((AiMoveConst)aiMovingType);
		}

		public override void chaseTarget(Int32 entityId)
		{
			Dbg.DEBUG_MSG("chaseTarget:" + entityId);
			renderEntity.setAiMoveTarget(entityId);
			renderEntity.setAiMovType((AiMoveConst)aiMovingType);
		}
		public override void useSkill(Int32 entityId, Int32 skillId)
        {
			Dbg.DEBUG_MSG("useSkill:" + entityId);
			
				renderEntity.setAiMoveTarget(entityId);
				renderEntity.setAiMovType((AiMoveConst)aiMovingType);
			
			
		}
		public override void stopMotion()
		{
			Dbg.DEBUG_MSG("stopMotion:");
			renderEntity.setAiMovType((AiMoveConst)aiMovingType);
		}




	}
}
