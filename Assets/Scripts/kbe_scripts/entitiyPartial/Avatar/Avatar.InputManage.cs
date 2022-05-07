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
		
		private MoveConst preMoveState = MoveConst.Idel;
		private bool preInbattle = false;

		private void __init__InputManage()
		{
			//settedInBattle = Convert.ToBoolean(inBattle);
			Event.registerIn("inputSwitchBattle", new Action<bool>(inputSwitchBattle));//切换战斗状态
			Event.registerIn("useSkill", new Action<int>(useSkill));//使用技能
			Event.registerIn("playerJump", new Action(playerJump));//玩家使用跳跃指令
			Event.registerIn("playerWalk", new Action(playerWalk));//玩家走
			Event.registerIn("playerRun", new Action(playerRun));//玩家跑
			Event.registerIn("playerIdle", new Action(playerIdle));//玩家Idle
			Event.registerIn("setFaceDirection", new Action<VECTOR3>(setFaceDirection));//玩家朝向
			Event.registerIn("setMoveDirection", new Action<VECTOR3>(setMoveDirection));//玩家移动方向
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
			uploadInbattle(settedInBattle);
		}

		public virtual void useSkill(int skillid)
		{
			Dbg.DEBUG_MSG("useSkill:" + skillid );
			requestUseSkill(skillid);
		}

		public virtual void playerJump()
		{
			if (preUseSkill != null || preMoveState == MoveConst.ServerMove) //使用技能状态禁止移动, 服务端移动也禁止移动
			{
				return;
			}
			if (preMoveState == MoveConst.Jump)
			{
				return;
			}
			preMoveState = MoveConst.Jump;
			uploadMovetype(preMoveState);
			renderEntity.setMoveType(MoveConst.Jump);
		}

		public void playerJumpFinish()
		{
			preMoveState = MoveConst.Idel;
			renderEntity.setMoveType(MoveConst.Idel);
		}

		public virtual void playerWalk()
		{
			if (preUseSkill != null || preMoveState == MoveConst.ServerMove) //使用技能状态禁止移动, 服务端移动也禁止移动
			{
				return;
			}
			if (preMoveState == MoveConst.Jump)
			{
				return;
			}
			preMoveState = MoveConst.Walk;
			uploadMovetype(preMoveState);
			renderEntity.setMoveType(MoveConst.Walk);
		}

		public virtual void playerRun()
		{
			if (preUseSkill != null || preMoveState == MoveConst.ServerMove) //使用技能状态禁止移动, 服务端移动也禁止移动
			{
				return;
			}
			if (preMoveState == MoveConst.Jump)
			{
				return;
			}
			preMoveState = MoveConst.Run;
			renderEntity.setMoveType(MoveConst.Run);
		}

		public virtual void playerIdle()
		{
			if (preUseSkill != null || preMoveState == MoveConst.ServerMove) //使用技能状态禁止移动, 服务端移动也禁止移动
			{
				return;
			}
			if (preMoveState == MoveConst.Jump)
			{
				return;
			}
			preMoveState = MoveConst.Idel;
			uploadMovetype(preMoveState);
			renderEntity.setMoveType(MoveConst.Idel);
		}

		public virtual void setFaceDirection(VECTOR3 faceDirection)
		{
			//Dbg.DEBUG_MSG("setFaceDirection:" + faceDirection);
			if (preMoveState != MoveConst.Idel)
			{
				renderEntity.setEntityFaceDirection(faceDirection);
			}
		}

		public virtual void setMoveDirection(VECTOR3 moveDirection)
		{
            
			renderEntity.setEntityMoveDirection(moveDirection);
			
		}

		public override void onMoveTypeChanged(UInt32 oldValue)
		{
			preMoveState = (MoveConst)moveType;
		}

		public override void onInBattleChanged(Byte oldValue)
		{
			preInbattle = Convert.ToBoolean(inBattle);
		}
	}
}