namespace KBEngine
{
	using GameLogic;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Const;
	//和玩家控制相关的，包括玩家主动输入和程序输入(不一定移动是引擎的Event通知)
	public partial class Avatar : AvatarBase, IServerEntity
	{
		private bool _settedInBattle;
		private int preUseSkillId = 0;
		private bool preJump = false;

		private void __init__InputManage()
		{
			//settedInBattle = Convert.ToBoolean(inBattle);
			Event.registerIn("inputSwitchBattle", this, "inputSwitchBattle");//切换战斗状态
			Event.registerIn("useSkill", this, "useSkill");//使用技能
			Event.registerIn("playerJump", this, "playerJump");//玩家使用跳跃指令
			Event.registerIn("playerWalk", this, "playerWalk");//玩家走
			Event.registerIn("playerRun", this, "playerRun");//玩家跑
			Event.registerIn("playerIdle", this, "playerIdle");//玩家Idle
		}

		//非移动状态
		public void inputSwitchBattle(bool settedInBattle)
		{
			Dbg.DEBUG_MSG("inputSwitchBattle:" + settedInBattle + inBattle);
			if (settedInBattle == Convert.ToBoolean(inBattle))
				return;
			/*todo
             * 其他不能入战的情况判定
             */

			Event.fireOut("setEnitiyInbattle", new object[] { this, settedInBattle });  //通知表现层，p1设置进入战斗状态
			renderEntity.setEnitiyInbattle(settedInBattle);
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

		public virtual void playerJump()
		{
			if (preUseSkillId > 0 || moveType == ((uint)MoveConst.ServerMove)) //使用技能状态禁止移动, 服务端移动也禁止移动
			{
				return;
			}
            if (preJump)
            {
				return;
            }
			preJump = true;
			renderEntity.setMoveType(MoveConst.Jump);
		}

		public void playerJumpFinish()
		{
			preJump = false;
			renderEntity.setMoveType(MoveConst.Idel);
		}

		public virtual void playerWalk()
		{
			if (preUseSkillId > 0 || moveType == ((uint)MoveConst.ServerMove)) //使用技能状态禁止移动, 服务端移动也禁止移动
			{
				return;
			}
			if (preJump)
			{
				return;
			}
			renderEntity.setMoveType(MoveConst.Walk);
		}

		public virtual void playerRun()
		{
			if (preUseSkillId > 0 || moveType == ((uint)MoveConst.ServerMove)) //使用技能状态禁止移动, 服务端移动也禁止移动
			{
				return;
			}
			if (preJump)
			{
				return;
			}
			renderEntity.setMoveType(MoveConst.Run);
		}

		public virtual void playerIdle()
		{
			if (preUseSkillId > 0 || moveType == ((uint)MoveConst.ServerMove)) //使用技能状态禁止移动, 服务端移动也禁止移动
			{
				return;
			}
			if (preJump)
			{
				return;
			}
			renderEntity.setMoveType(MoveConst.Idel);
		}
	}
}