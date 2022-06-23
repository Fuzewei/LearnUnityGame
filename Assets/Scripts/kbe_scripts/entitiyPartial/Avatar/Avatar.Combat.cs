namespace KBEngine
{
	using GameLogic;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Const;
	public partial class Avatar : AvatarBase, IServerEntity
	{
		public TimeLineManager _timeLineManager;

		public Dictionary<int, Skill> curUseSkills;//预记录一下

		private void __init__Combat()
		{
			_timeLineManager = new TimeLineManager();
			curUseSkills = new Dictionary<int, Skill>();
		}

		public TimeLineManager timeLineManager { get {
				return _timeLineManager;
			} set {
				_timeLineManager = value;
			} 
		}

		public override void recvDamage(Int32 attackerID, Int32 skillID, Int32 damageType, Int32 damage)
		{
			// Dbg.DEBUG_MSG(className + "::recvDamage: attackerID=" + attackerID + ", skillID=" + skillID + ", damageType=" + damageType + ", damage=" + damage);
			Entity entity = KBEngineApp.app.findEntity(attackerID);

			Event.fireOut("recvDamage", new object[] { this, entity, skillID, damageType, damage });
		}

		public override void skillNodeCallClient(UInt32 uuid, Int32 nodeId, TABLE args)
		{
			Dbg.DEBUG_MSG("skillNodeCallClient:" + uuid + nodeId);
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

		//客户端主动放技能(先行)
		public void requestUseSkill(int skillid)
		{
			if (curUseSkills.ContainsKey(skillid))
			{
				return;
			}
			Dbg.DEBUG_MSG("requestUseSkill:" + skillid);
			var skill = new Skill(skillid, this, SkillNodeType.P1);
			curUseSkills[skillid] = skill;
			var uuid = timeLineManager.getUUid();
			skill.startTimeLine(skill.initTimeLineId, uuid);
			cellEntityCall.clientRequestUseSkill(uuid, skillid);
			renderEntity.setEntityInUseSkill(skillid);
		}

		public override void serverSkillFinish(Int32 skillid)
		{
			var skill = curUseSkills[skillid];
			curUseSkills.Remove(skillid);
			skill.doFininsh();
            if (curUseSkills.Count == 0)
            {
				renderEntity.setEntityFinishSkill(skillid);
			}
			Dbg.DEBUG_MSG("onSkillFinish:" + skillid);
		}

		public override void serverRequestUseSkill(uint UUid, Int32 skillId) //服务端通知p3放技能
		{
			var skill = new Skill(skillId, this, SkillNodeType.P3);
			curUseSkills[skillId] = skill;
			skill.startTimeLine(skill.initTimeLineId, UUid);
			renderEntity.setEntityInUseSkill(skillId);
		}

	}
}