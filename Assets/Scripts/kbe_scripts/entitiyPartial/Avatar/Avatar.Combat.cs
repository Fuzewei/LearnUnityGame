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

		//客户端主动放技能
		public void requestUseSkill(int skillid)
		{
			if (preUseSkill != null)
			{
				return;
			}
			preUseSkill = new Skill(skillid, this);
			var uuid = timeLineManager.getUUid();
			preUseSkill.startTimeLine(preUseSkill.initTimeLineId, uuid);
			cellEntityCall.clientRequestUseSkill(uuid, skillid);
			renderEntity.setEntityInUseSkill(skillid);
		}


		public void onSkillFinish(int skillid)
		{
			cellEntityCall.clientSkillFinish(skillid);
		}

		public override void serverSkillFinish(Int32 skillid)
		{
			Dbg.DEBUG_MSG("onSkillFinish:" + skillid);
			preUseSkill = null;
			renderEntity.setEntityFinishSkill(skillid);
		}

		public void onTimeLineFinish(UUID uuid)
		{
			Dbg.DEBUG_MSG("onSkillFinish123123:" + uuid);
			cellEntityCall.clientTimeLineFinish(uuid);
		}

		public override void serverTimeLineFinish(UInt32 uuid)
		{
			preUseSkill?.onTimeLineFinish(uuid);
			timeLineManager.delTimeLine(uuid);
		}

		public override void serverRequestUseSkill(uint UUid, Int32 skillId) //服务端通知p3放技能
		{
			preUseSkill = new Skill(skillId, this, SkillNodeType.P3);
			preUseSkill.startTimeLine(preUseSkill.initTimeLineId, UUid);

			//skillTimeLine line = SkillFactory.getTimeLineById(this, timeLineId, SkillNodeType.P3);
			//timeLineManager.addTimeLine(UUid, line);
		}

	}
}