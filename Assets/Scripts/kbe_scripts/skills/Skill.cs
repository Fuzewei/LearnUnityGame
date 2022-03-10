namespace KBEngine
{
  	using UnityEngine; 
	using System; 
	using System.Collections; 
	using System.Collections.Generic;
	
    public class Skill 
    {
    	public string name;
    	public string descr;
    	public Int32 skillId;
		public Avatar avatar;
		public List<UUID> timeLineUUIDs;

		public int initTimeLineId = 1;
		SkillNodeType nodeType;

		// 最后一次使用时间， 最好的做法是给客户端实体增加cooldown系统，由于demo只是简单展示，就这样简单实现功能
		public System.DateTime lastUsedTime = System.DateTime.Now;

        public Skill(Int32 _id, Avatar _avatar, SkillNodeType _nodeType = SkillNodeType.P1)
		{
			timeLineUUIDs = new List<UUID>();
			skillId = _id;
			avatar = _avatar;
			nodeType = _nodeType;
		}

		public uint use()
		{
			return startTimeLine(initTimeLineId, avatar.timeLineManager.getUUid());
		}

		public uint startTimeLine(int timeLineId, uint uuid )
		{
			skillTimeLine line = SkillFactory.getTimeLineById(avatar, timeLineId, nodeType);
			timeLineUUIDs.Add(uuid);
			line.setUUID(uuid);
			avatar.timeLineManager.addTimeLine(uuid, line);
			return uuid;
		}

		public void onTimeLineFinish(UUID uuid)
		{
			var index = timeLineUUIDs.FindIndex((x)=>x == uuid);
            if (index >= 0 )
            {
				timeLineUUIDs.RemoveAt(index);
			}
            if (timeLineUUIDs.Count == 0)
            {
				onFininsh();
			}
		}

		public void onFininsh()
		{
			avatar.onSkillFinish(skillId);
		}



	}
} 
