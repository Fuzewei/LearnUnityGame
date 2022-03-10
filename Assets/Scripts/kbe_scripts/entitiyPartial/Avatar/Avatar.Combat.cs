namespace KBEngine
{
	using GameLogic;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Const;
	public partial class Avatar : AvatarBase, IServerEntity
	{
		public TimeLineManager timeLineManager;

		public Skill preUseSkill;

		private void __init__Combat()
		{
			timeLineManager = new TimeLineManager();
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
		}

		public void requestUseSkill(int skillid)
		{
			if (preUseSkill != null)
			{
				return;
			}
			preUseSkill = new Skill(skillid, this);
			var uuid = preUseSkill.use();
			cellEntityCall.clientRequestUseSkill(uuid, skillid);
			renderEntity.setEntityInUseSkill(skillid);
		}

		public void skillFinish(int skillid)
		{
			Dbg.DEBUG_MSG("skillFinish:" + skillid);
			preUseSkill = null;
			renderEntity.setEntityFinishSkill(skillid);
		}

		public void onTimeLineFinish(UUID uuid)
		{
			preUseSkill?.onTimeLineFinish(uuid);
		}

		public override void serverRequestUseSkill(uint UUid, Int32 timeLineId)
		{
			skillTimeLine line = SkillFactory.getTimeLineById(this, timeLineId, SkillNodeType.P3);
			timeLineManager.addTimeLine(UUid, line);
		}

	}
}