namespace KBEngine
{
	using GameLogic;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Const;
	public partial class Avatar : AvatarBase, IServerEntity
	{
		private TimeLineManager timeLineManager;
		private void __init__Combat()
		{

			timeLineManager = new TimeLineManager();

		}

		public override void onInBattleChanged(Byte oldValue)
		{
			Dbg.DEBUG_MSG("onInBattleChanged:" + oldValue +"    " +inBattle);
		}

		public override void recvDamage(Int32 attackerID, Int32 skillID, Int32 damageType, Int32 damage)
		{
			// Dbg.DEBUG_MSG(className + "::recvDamage: attackerID=" + attackerID + ", skillID=" + skillID + ", damageType=" + damageType + ", damage=" + damage);
			Entity entity = KBEngineApp.app.findEntity(attackerID);

			Event.fireOut("recvDamage", new object[] { this, entity, skillID, damageType, damage });
		}

		public override void skillNodeCallClient(UInt32 uuid, Int32 nodeId, TABLE args)
		{
			var timeLine = timeLineManager.getTimeLine(uuid);
            if (timeLine != null)
            {
				((skillTimeLine)timeLine).callFromServer(nodeId, args);
			}
		}


		public void uploadInbattle(bool Inbattle)
		{
			Dbg.DEBUG_MSG("uploadInbattle:" + Inbattle);
			cellEntityCall.setInBattle(Utils.localTime(), Convert.ToByte(Inbattle));
			//requestUseSkill(preUseSkillId);
		}

		public void requestUseSkill(int skillid)
		{
			skillTimeLine line = new skillTimeLine(this);
			NodeBase NodeBase1 = new PlayerAnimationNode(0.0f, "Attack.GreatSword_Attack01");
			line.addNode(NodeBase1);
			NodeBase NodeBase2 = new CommonAttack(0.3f);
			line.addNode(NodeBase2);
			NodeBase NodeBase3 = new TimeLineEndNode(1.5f);
			line.addNode(NodeBase3);
			var uuid = timeLineManager.getUUid();
			timeLineManager.addTimeLine(uuid, line);
			cellEntityCall.clientRequestUseSkill(uuid, skillid);
		}

		public override void serverRequestUseSkill(uint UUid, Int32 skillId)
		{
			skillTimeLine line = new skillTimeLine(this);
			PlayerAnimationNode NodeBase1 = new PlayerAnimationNode(0.0f, "Attack.GreatSword_Attack01");
			NodeBase1.nodeType = SkillNodeType.P3;
			line.addNode(NodeBase1);
			CommonAttack NodeBase2 = new CommonAttack(0.3f);
			NodeBase2.nodeType = SkillNodeType.P3;
			line.addNode(NodeBase2);
			TimeLineEndNode NodeBase3 = new TimeLineEndNode(1.5f);
			NodeBase3.nodeType = SkillNodeType.P3;
			line.addNode(NodeBase3);
			timeLineManager.addTimeLine(UUid, line);
		}

	}
}