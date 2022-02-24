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
		private int preUseSkillId = 0;

		private void __init__InputManage()
		{
			settedInBattle = Convert.ToBoolean(inBattle);
			Event.registerIn("inputSwitchBattle", this, "inputSwitchBattle");//切换战斗
			Event.registerIn("inputCommand", this, "inputCommand");//移动指令通知
			Event.registerIn("useSkill", this, "useSkill");//使用技能
			Event.registerIn("skillFinish", this, "skillFinish");//技能结束
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

		//移动状态(使用技能的情况下禁止新的移动)
		public virtual void inputCommand(MoveConst commandType)
		{
			Dbg.DEBUG_MSG("inputCommand:" + commandType);
            if (preUseSkillId > 0 || moveType == ((uint)MoveConst.ServerMove)) //使用技能状态禁止移动, 服务端移动也禁止移动
            {
				return;
            }
			renderEntity.setMoveType(commandType);
		}

		public virtual void useSkill(int skillid)
		{
			Dbg.DEBUG_MSG("useSkill:" + skillid);
			preUseSkillId = skillid;
			renderEntity.setEnitiyInUseSkill(skillid);
			//requestUseSkill(skillid);
        }

		public virtual void skillFinish(int skillid)
		{
			Dbg.DEBUG_MSG("skillFinish:" + skillid);
			preUseSkillId = 0;
			renderEntity.setEnitiyFinishSkill(skillid);
			//requestUseSkill(skillid);
		}
	}
}