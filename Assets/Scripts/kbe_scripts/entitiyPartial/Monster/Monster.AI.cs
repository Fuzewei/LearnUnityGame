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

		private void __init__Ai()
		{
			
		}

		private void __onRenderObjectCreate__Ai()
		{
			renderEntity.setAiMovType((AiMoveConst)aiMovingType);
			renderEntity.setAiMovePath(aiMoviePath, aiMoviePathIndex);
			renderEntity.setAiMoveTarget(targetID);
			renderEntity.setAiMovPoint(aiMovieToPoint);
		}

		public override void randomWalk()
		{
			Dbg.DEBUG_MSG("[monsterAI]randomWalk:" + aiMoviePath);
			renderEntity.setAiMovePath(aiMoviePath, aiMoviePathIndex);
			renderEntity.setAiMovType((AiMoveConst)aiMovingType);
		}

		public override void chaseTarget(Int32 entityId)
		{
			Dbg.DEBUG_MSG("[monsterAI]chaseTarget:" + entityId);
			renderEntity.setAiMoveTarget(targetID);
			renderEntity.setAiMovType((AiMoveConst)aiMovingType);
		}
		public override void useSkill(Int32 entityId, Int32 skillId)
        {
			Dbg.DEBUG_MSG("[monsterAI]useSkill:" + entityId);
			
			renderEntity.setAiMoveTarget(entityId);
			renderEntity.setAiMovType((AiMoveConst)aiMovingType);
	
		}
		public override void stopMotion()
		{
			Dbg.DEBUG_MSG("[monsterAI]stopMotion:");
			renderEntity.setAiMovType((AiMoveConst)aiMovingType);
		}

		public override void fightMove(SByte moveId, Vector3 movePostion)
		{
			Dbg.DEBUG_MSG("[monsterAI]fightMove:" + moveId + movePostion);
			renderEntity.setAiMovType((AiMoveConst)aiMovingType);
			renderEntity.setAiMoveTarget(targetID);
			renderEntity.setAiMovPoint(aiMovieToPoint);
		}


	}
}
