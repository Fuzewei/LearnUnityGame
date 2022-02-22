namespace KBEngine
{
	using GameLogic;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Const;
	public partial class Avatar : AvatarBase, IServerEntity
	{
		private bool settedInBattle;

		private void __init__InputManage()
		{
			settedInBattle = Convert.ToBoolean(inBattle);
			Event.registerIn("inputSwitchBattle", this, "inputSwitchBattle");//切换战斗
			Event.registerIn("inputCommand", this, "inputCommand");//移动指令通知
			Event.registerIn("useSkill", this, "useSkill");//使用技能
		}

		//非移动状态
		public void inputSwitchBattle()
		{
			Dbg.DEBUG_MSG("inputSwitchBattle:" + settedInBattle + inBattle);
            if (settedInBattle == Convert.ToBoolean(inBattle))
            {
				settedInBattle = !settedInBattle;
				renderEntity.setEnitiyInbattle(settedInBattle);
			}
		}

		//移动状态
		public virtual void inputCommand(MoveConst commandType)
		{
			Dbg.DEBUG_MSG("inputCommand:" + commandType);
			renderEntity.setMoveType(commandType);
		}

		public virtual void useSkill(int skillid)
		{
			Dbg.DEBUG_MSG("useSkill:" + skillid);
			requestUseSkill(skillid);
        }
	}
}