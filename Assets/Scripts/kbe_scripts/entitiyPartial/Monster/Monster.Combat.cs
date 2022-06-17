namespace KBEngine
{
	using GameLogic;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using Const;
	public partial class Monster : MonsterBase, IServerEntity
	{
		public TimeLineManager _timeLineManager;

		public Dictionary<int, Skill> curUseSkills;//预记录一下

		//客户端主动放技能

		private void __init__Combat()
		{
			_timeLineManager = new TimeLineManager();
		}

		public TimeLineManager timeLineManager
		{
			get
			{
				return _timeLineManager;
			}
			set
			{
				_timeLineManager = value;
			}
		}
		public override void serverRequestUseSkill(uint UUid, Int32 skillId) //服务端通知p3放技能
		{
			var skill = new Skill(skillId, this, SkillNodeType.Monster);
			curUseSkills[skillId] = skill;
			skill.startTimeLine(skill.initTimeLineId, UUid);
			renderEntity.setEntityInUseSkill(skillId);
		}

		public override void serverSkillFinish(Int32 skillid)
		{
			Dbg.DEBUG_MSG("onSkillFinish:" + skillid);
			var skill = curUseSkills[skillid];
			curUseSkills[skillid] = null;
			skill.doFininsh();
			renderEntity.setEntityFinishSkill(skillid);
		}

	}
}
