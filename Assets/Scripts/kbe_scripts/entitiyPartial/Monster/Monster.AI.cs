namespace KBEngine
{
	using GameLogic;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Const;
	public partial class Monster : MonsterBase, IServerEntity
	{
		

		public override void randomWalk(PATH_POINTS arg1)
		{
			Dbg.DEBUG_MSG("randomWalk:" + arg1);
			renderEntity.setMoveType(MoveConst.Walk);
		}


	}
}