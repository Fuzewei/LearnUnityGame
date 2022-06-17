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
		public IServerEntity avatar;
		public List<UUID> timeLineUUIDs;

		public int initTimeLineId = 1;
		SkillNodeType nodeType;

		// ���һ��ʹ��ʱ�䣬 ��õ������Ǹ��ͻ���ʵ������cooldownϵͳ������demoֻ�Ǽ�չʾ����������ʵ�ֹ���
		public System.DateTime lastUsedTime = System.DateTime.Now;

        public Skill(Int32 _id, IServerEntity _avatar, SkillNodeType _nodeType = SkillNodeType.P1)
		{
			timeLineUUIDs = new List<UUID>();
			skillId = _id;
			initTimeLineId = skillId;
			avatar = _avatar;
			nodeType = _nodeType;
		}

		//�ڼ����ڿ���timeline
		public uint startTimeLine(int timeLineId, uint uuid )
		{
			skillTimeLine line = SkillFactory.getTimeLineById(avatar, timeLineId, nodeType);
			timeLineUUIDs.Add(uuid);
			line.setUUID(uuid);
			line.setSkillId(skillId);
			avatar.timeLineManager.addTimeLine(uuid, line);
			return uuid;
		}

		//���ܽ����ĵ���
		public void doFininsh()
		{
			foreach (var uuid in timeLineUUIDs)
			{
				avatar.timeLineManager.delTimeLine(uuid);
			}
			timeLineUUIDs = null;

		}

	}
} 
