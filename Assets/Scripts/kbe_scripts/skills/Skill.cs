namespace KBEngine
{
  	using UnityEngine; 
	using System; 
	using System.Collections; 
	using System.Collections.Generic;

	
    public class Skill 
    {
		// skillId���������� Ӧ������
		static Dictionary<int, string> skillId2StartTimeLine;
		static Skill()
		{
			skillId2StartTimeLine = new Dictionary<int, string>();
			skillId2StartTimeLine[1] = "commonAttack1";
			skillId2StartTimeLine[2] = "commonAttack2";
			skillId2StartTimeLine[3] = "commonAttack3";
		}



		public string name;
    	public string descr;
    	public Int32 skillId;
		public IServerEntity avatar;
		public List<UUID> timeLineUUIDs;

		public string initTimeLineName;
		SkillNodeType nodeType;

		// ���һ��ʹ��ʱ�䣬 ��õ������Ǹ��ͻ���ʵ������cooldownϵͳ������demoֻ�Ǽ�չʾ����������ʵ�ֹ���
		public System.DateTime lastUsedTime = System.DateTime.Now;

		


		public Skill(Int32 _id, IServerEntity _avatar, SkillNodeType _nodeType = SkillNodeType.P1)
		{
			timeLineUUIDs = new List<UUID>();
			skillId = _id;
			initTimeLineName = skillId2StartTimeLine[skillId];
			avatar = _avatar;
			nodeType = _nodeType;
		}

		//�ڼ����ڿ���timeline
		public uint startTimeLine(string timeLineName, uint uuid )
		{
			skillTimeLine line = SkillFactory.getTimeLineById(avatar, timeLineName, nodeType);
			timeLineUUIDs.Add(uuid);
			line.setUUID(uuid);
			line.setSkillId(skillId);
			avatar.timeLineManager.addTimeLine(uuid, line);
			avatar.renderEntity.palyerTimeLine(timeLineName);
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
