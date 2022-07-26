using KBEngine;
using UnityEngine;
using System; 
using System.IO;  
using System.Collections; 
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public partial class World : MonoBehaviour 
{
	public static World world = null;
	private UnityEngine.GameObject terrain = null;
	public UnityEngine.GameObject terrainPerfab;
	
	private UnityEngine.GameObject player = null;
	public UnityEngine.GameObject entityPerfab;
	public UnityEngine.GameObject avatarPerfab;

	public UnityEngine.GameObject monsterPerfab;
	public UnityEngine.GameObject cameraPerfab;

	private uint curSpaceId = 0;




	void Awake() 
	 {
		DontDestroyOnLoad(transform.gameObject);
		world = this;
		AwakeGameObjectManager();
		WorldInit();
	}
	 
	// Use this for initialization
	void Start () 
	{
		installEvents();
		timers = new SortedDictionary<uint, TimerHandle>();
	}

	void installEvents()
	{
		// in world
		KBEngine.Event.registerOut("addSpaceGeometryMapping", this, "addSpaceGeometryMapping");
		KBEngine.Event.registerOut("onEnterWorld", this, "onEnterWorld");
		KBEngine.Event.registerOut("onLeaveWorld", this, "onLeaveWorld");
		//KBEngine.Event.registerOut("set_position", this, "set_position");
		//KBEngine.Event.registerOut("set_direction", this, "set_direction");
		KBEngine.Event.registerOut("updatePosition", this, "updatePosition");
		KBEngine.Event.registerOut("onControlled", this, "onControlled");
		
		// in world(register by scripts)
		KBEngine.Event.registerOut("onAvatarEnterWorld", this, "onAvatarEnterWorld");
		KBEngine.Event.registerOut("set_HP", this, "set_HP");
		KBEngine.Event.registerOut("set_MP", this, "set_MP");
		KBEngine.Event.registerOut("set_HP_Max", this, "set_HP_Max");
		KBEngine.Event.registerOut("set_MP_Max", this, "set_MP_Max");
		KBEngine.Event.registerOut("set_level", this, "set_level");
		KBEngine.Event.registerOut("set_name", this, "set_entityName");
		//KBEngine.Event.registerOut("set_state", this, "set_state");
		KBEngine.Event.registerOut("set_moveSpeed", this, "set_moveSpeed");
		KBEngine.Event.registerOut("set_modelScale", this, "set_modelScale");
		KBEngine.Event.registerOut("set_modelID", this, "set_modelID");
		KBEngine.Event.registerOut("recvDamage", this, "recvDamage");
		KBEngine.Event.registerOut("otherAvatarOnJump", this, "otherAvatarOnJump");
		KBEngine.Event.registerOut("onAddSkill", this, "onAddSkill");
	}

	void OnDestroy()
	{
		KBEngine.Event.deregisterOut(this);
	}
	
	// Update is called once per frame
	void Update () 
	{
		createPlayer();
		updateTimes();
		tickGameObjectManager();
	}
	
	public void addSpaceGeometryMapping(string respath)
	{
		// 这个事件可以理解为服务器通知客户端加载指定的场景资源
		// 通过服务器的api KBEngine.addSpaceGeometryMapping设置到spaceData中，进入space的玩家就会被同步spaceData里面的内容
		Debug.Log("loading scene(" + respath + ")...");

		UI.inst.info("scene(" + respath + "), spaceID=" + KBEngineApp.app.spaceID);
        if (KBEngineApp.app.spaceID == 4)
        {
			
			SceneManager.LoadScene("Scenes/fight1", LoadSceneMode.Single);
            if (curSpaceId == 3)
            {
				SceneManager.UnloadSceneAsync("Scenes/world");
				Destroy(terrain);
				terrain = null;
			}
			
		}

		if (KBEngineApp.app.spaceID == 3)
		{
			terrain = Instantiate(terrainPerfab) as UnityEngine.GameObject;
            if (curSpaceId == 4)
            {
				SceneManager.UnloadSceneAsync("Scenes/fight1");

			}
		}

		curSpaceId = KBEngineApp.app.spaceID;

		//if (terrain == null)
		//	terrain = Instantiate(terrainPerfab) as UnityEngine.GameObject;

		if (player && !player.GetComponent<GameEntity>().entityEnabled)
			player.GetComponent<GameEntity>().entityEnable((KBEngine.Avatar)KBEngineApp.app.player());
	}	
	
	public void onAvatarEnterWorld(UInt64 rndUUID, Int32 eid, KBEngine.Avatar avatar)
	{
		if(!avatar.isPlayer())
		{
			return;
		}

		UI.inst.info("loading scene...(加载场景中...)");
		Debug.Log("loading scene...");
	}

	public void createPlayer()
	{

		if (KBEngineApp.app.entity_type != "Avatar") //可能是account
		{
			return;
		}

		KBEngine.Avatar avatar = (KBEngine.Avatar)KBEngineApp.app.player();
		if (avatar == null)
		{
			Debug.Log("wait create(palyer)!");
			return;
		}


		// 需要等场景加载完毕再显示玩家
		if (player != null)
		{
			if (curSpaceId > 0 && !player.GetComponent<GameEntity>().entityEnabled)
			{
				return;
			}
				
			return;
		}
		
		float y = avatar.position.y;
		if(avatar.isOnGround)
			y = 2.3f;

		player = Instantiate(avatarPerfab, new Vector3(avatar.position.x, y, avatar.position.z), Quaternion.Euler(avatar.direction)) as UnityEngine.GameObject;
		GameEntity gameEntity = player.GetComponent<GameEntity>();
		avatar.renderObj = player;
		gameEntity.isPlayer = true;
		gameEntity.entityEnable(avatar);



		var cameraFollow = player.transform.Find("CameraTarget");
		GameObject camera = Instantiate(cameraPerfab);
		Cinemachine.CinemachineFreeLook freelook = camera.GetComponent<Cinemachine.CinemachineFreeLook>();
		freelook.Follow = cameraFollow;
		freelook.LookAt = cameraFollow;

		// 有必要设置一下，由于该接口由Update异步调用，有可能set_position等初始化信息已经先触发了
		// 那么如果不设置renderObj的位置和方向将为0，人物会陷入地下
		set_position(avatar);
		set_direction(avatar);
        set_entityName(avatar, avatar.name);
    }
	
	public void onEnterWorld(KBEngine.Entity entity)
	{
		Debug.Log("onEnterWorld");
		if (entity.isPlayer())
			return;
		
		float y = entity.position.y;
		if(entity.isOnGround)
			y = 1.3f;
		

        if (entity is KBEngine.Avatar)
        {
			var p3 = entity as KBEngine.Avatar;
			entity.renderObj = Instantiate(entityPerfab, new Vector3(entity.position.x, y, entity.position.z), Quaternion.Euler(entity.direction)) as UnityEngine.GameObject;
			var renderEntity = entity.renderObj as UnityEngine.GameObject;
			GameEntity gameEntity = renderEntity.GetComponent<GameEntity>();
			gameEntity.entityEnable((KBEngine.Avatar)entity);
			
			//p3.renderEntity = gameEntity;

			((UnityEngine.GameObject)entity.renderObj).name = entity.className + "_id:" + entity.id;
		}
        if (entity is KBEngine.Monster)
        {
			var monster = entity as KBEngine.Monster;
			monster.renderObj = Instantiate(monsterPerfab, new Vector3(entity.position.x, y, entity.position.z), Quaternion.Euler(entity.direction)) as UnityEngine.GameObject;
			var renderEntity = entity.renderObj as UnityEngine.GameObject;
			GameEntity gameEntity = renderEntity.GetComponent<GameEntity>();
			gameEntity.entityEnable((KBEngine.Monster)entity);
			
			((UnityEngine.GameObject)entity.renderObj).name = entity.className + "_id:" + entity.id;
		}
		
	}

	public void onAddSkill(KBEngine.Entity entity)
	{
		Debug.Log("onAddSkill");
	}
	public void onLeaveWorld(KBEngine.Entity entity)
	{
		Debug.Log("onLeaveWorld");
		if (entity.renderObj == null)
			return;
		
		UnityEngine.GameObject.Destroy((UnityEngine.GameObject)entity.renderObj);
		entity.renderObj = null;
	}

	public void set_position(KBEngine.Entity entity)
	{
		Debug.Log("set_position"+ entity.position);
		if (entity.renderObj == null)
			return;

		GameEntity gameEntity = ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>();
		gameEntity.destPosition = entity.position;
		gameEntity.position = entity.position;
		gameEntity.spaceID = KBEngineApp.app.spaceID;
	}

	public void updatePosition(KBEngine.Entity entity)//设置了怪物的位置的回调
	{
		Debug.Log("updatePosition:"+ entity.className);
		if(entity.renderObj == null)
			return;
		
		GameEntity gameEntity = ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>();
		//gameEntity.destPosition = entity.position;
		//gameEntity.position = entity.position;
		gameEntity.isOnGround = entity.isOnGround;
		gameEntity.spaceID = KBEngineApp.app.spaceID;
	}
	
	public void onControlled(KBEngine.Entity entity, bool isControlled)
	{
		if(entity.renderObj == null)
			return;
		
		GameEntity gameEntity = ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>();
		gameEntity.isControlled = isControlled;
	}
	
	public void set_direction(KBEngine.Entity entity)
	{
		if(entity.renderObj == null)
			return;
		
		GameEntity gameEntity = ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>();
		gameEntity.destDirection = new Vector3(entity.direction.y, entity.direction.z, entity.direction.x); 
		gameEntity.spaceID = KBEngineApp.app.spaceID;
	}

	public void set_HP(KBEngine.Entity entity, Int32 v, Int32 HP_Max)
	{
		if(entity.renderObj != null)
		{
			((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().hp = "" + v + "/" + HP_Max;
		}
	}
	
	public void set_MP(KBEngine.Entity entity, Int32 v, Int32 MP_Max)
	{
	}
	
	public void set_HP_Max(KBEngine.Entity entity, Int32 v, Int32 HP)
	{
		if(entity.renderObj != null)
		{
			((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().hp = HP + "/" + v;
		}
	}
	
	public void set_MP_Max(KBEngine.Entity entity, Int32 v, Int32 MP)
	{
	}
	
	public void set_level(KBEngine.Entity entity, UInt16 v)
	{
	}
	
	public void set_entityName(KBEngine.Entity entity, string v)
	{
		if(entity.renderObj != null)
		{
			((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().entity_name = v;
		}
	}
	
	public void set_state(KBEngine.Entity entity, SByte v)
	{
		if(entity.renderObj != null)
		{
			((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().set_state(v);
		}
		
		if(entity.isPlayer())
		{
			Debug.Log("player->set_state: " + v);
			
			if(v == 1)
				UI.inst.showReliveGUI = true;
			else
				UI.inst.showReliveGUI = false;
			
			return;
		}
	}

	public void set_moveSpeed(KBEngine.Entity entity, float v)
	{
        if (entity.renderObj != null)
        {
            ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().speed = v;
        }
    }
	
	public void set_modelScale(KBEngine.Entity entity, Byte v)
	{
	}
	
	public void set_modelID(KBEngine.Entity entity, UInt32 v)
	{
	}
	
	public void recvDamage(KBEngine.Entity entity, KBEngine.Entity attacker, Int32 skillID, Int32 damageType, Int32 damage)
	{
	}
	
	public void otherAvatarOnJump(KBEngine.Entity entity)
	{
		Debug.Log("otherAvatarOnJump: " + entity.id);
		if(entity.renderObj != null)
		{
			((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().OnJump();
		}
	}






	//timer相关,暂时这么用

	struct TimerHandle
	{
		public uint timerId;
		public float interval;
		public float beginTimeStamp; //设置时间
		public float nextTimeStamp; //下次时间
		public uint times;
	}
	private SortedDictionary<uint, TimerHandle> timers;

	private UInt32 g_timerId = 1;
	public UInt32 _addTimer(float timeout, float interval)
	{
		float now = KBEngine.Utils.localTime();
		TimerHandle timerHandle = new TimerHandle();
		timerHandle.timerId = g_timerId++;
		timerHandle.interval = interval;
		timerHandle.beginTimeStamp = now;
		timerHandle.nextTimeStamp = now + timeout;
		timers.Add(timerHandle.timerId, timerHandle);
		return timerHandle.timerId;
	}

	public void _cancelTimer(UInt32 timeId)
	{
		timers.Remove(timeId);
	}
	private void updateTimes()
	{
		float now = KBEngine.Utils.localTime();
		List<uint> passKeys = new List<uint>();
		foreach (var item in timers)
		{
			if (item.Value.nextTimeStamp <= now + 0.01f)
			{
				passKeys.Add(item.Key);
			}
		}
		foreach (var key in passKeys)
		{
			TimerHandle timeHandle = timers[key];
			TimerUtils.onTimer(timeHandle.timerId);
			timers.Remove(key);
			if (timeHandle.interval >= 0.001f)
            {
				timeHandle.times++;
				timeHandle.nextTimeStamp = now + timeHandle.interval;
				timers.Add(timeHandle.timerId, timeHandle);
			}	
		}

	}
}
