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
		private TimeLineManager timeLineManager;

		private void __init__InputManage()
		{
			settedInBattle = Convert.ToBoolean(inBattle);
			timeLineManager = new TimeLineManager();
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
			skillTimeLine line = new skillTimeLine(this);
			NodeBase NodeBase1 = new PlayerAnimationNode(0.0f, "Attack.GreatSword_Attack01");
			line.addNode(NodeBase1);
            NodeBase NodeBase2 = new CommonAttack(0.3f);
            line.addNode(NodeBase2);
            NodeBase NodeBase3 = new TimeLineEndNode(2.5f);
            line.addNode(NodeBase3);
            var uuid = timeLineManager.addTimeLine(line);
        }
	}
}