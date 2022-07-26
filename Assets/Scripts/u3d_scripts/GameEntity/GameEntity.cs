
using UnityEngine;
using KBEngine;
using KBEngine.Const;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

using System;


public class GameEntity : MonoBehaviour 
{
	public bool isPlayer = false;
	public IServerEntity _logicEntity;
	
	private Vector3 _position = Vector3.zero;
	private Vector3 _eulerAngles = Vector3.zero;
	private Vector3 _scale = Vector3.zero;
	private UInt32 _spaceID = 0;
	
	public Vector3 destPosition = Vector3.zero;
	public Vector3 destDirection = Vector3.zero;

	public float _speed = 0f;
	private byte jumpState = 0;
	private float currY = 1.0f;
	
	private Camera playerCamera = null;
	
	public string entity_name;
	
	public string hp = "100/100";
	
	float npcHeight = 2.0f;
	
	public CharacterController characterController;
	public MoveMotor moveMotor;

	public MoveConst moveType;

	PlayableDirector m_playabledirector;


	public bool isOnGround = true;

	public bool isControlled = false;
	
	public bool entityEnabled = true;

	//位置引用
	public GameObject hand_r;


	void Awake ()   
	{
		characterController = ((UnityEngine.GameObject)gameObject).GetComponent<CharacterController>();
		moveMotor = GetComponent<MoveMotor>();
		m_playabledirector = GetComponent<PlayableDirector>();
	}

	
	void Start() 
	{
        if (logicEntity != null)
        {
			logicEntity.onRenderObjectCreate(this);
		}
	}
	
	void OnGUI()
	{
		return;
		
		Vector3 worldPosition = new Vector3 (transform.position.x , transform.position.y + npcHeight, transform.position.z);
		
		if(playerCamera == null)
			playerCamera = Camera.main;
		
		//根据NPC头顶的3D坐标换算成它在2D屏幕中的坐标
		Vector2 uiposition = playerCamera.WorldToScreenPoint(worldPosition);
		
		//得到真实NPC头顶的2D坐标
		uiposition = new Vector2 (uiposition.x, Screen.height - uiposition.y);
		
		//计算NPC名称的宽高
		Vector2 nameSize = GUI.skin.label.CalcSize (new GUIContent(entity_name));
		
		//设置显示颜色为黄色
		GUI.color  = Color.yellow;
		
		//绘制NPC名称
		GUI.Label(new Rect(uiposition.x - (nameSize.x / 2), uiposition.y - nameSize.y - 5.0f, nameSize.x, nameSize.y), entity_name);
		
		//计算NPC名称的宽高
		Vector2 hpSize = GUI.skin.label.CalcSize (new GUIContent(hp));

		//设置显示颜色为红
		GUI.color = Color.red;
		
		//绘制HP
		GUI.Label(new Rect(uiposition.x - (hpSize.x / 2), uiposition.y - hpSize.y - 30.0f, hpSize.x, hpSize.y), hp);
	}
	
    public Vector3 position {  
		get
		{
			return _position;
		}

		set
		{
			_position = value;
			
			if(gameObject != null)
				gameObject.transform.position = _position;
		}    
    }

	public IServerEntity logicEntity
	{
		get
		{
			return _logicEntity;
		}
		set
		{
			_logicEntity = value;

		}
	}

	public Vector3 eulerAngles {  
		get
		{
			return _eulerAngles;
		}

		set
		{
			_eulerAngles = value;
			
			if(gameObject != null)
			{
				gameObject.transform.eulerAngles = _eulerAngles;
			}
		}    
    }  

    public Quaternion rotation {  
		get
		{
			return Quaternion.Euler(_eulerAngles);
		}

		set
		{
			eulerAngles = value.eulerAngles;
		}    
    }  
    
    public Vector3 scale {  
		get
		{
			return _scale;
		}

		set
		{
			_scale = value;
			
			if(gameObject != null)
				gameObject.transform.localScale = _scale;
		}    
    } 

    public float speed {  
		get
		{
			return _speed;
		}

		set
		{
			_speed = value;
		}    
    } 

    public UInt32 spaceID {  
		get
		{
			return _spaceID;
		}

		set
		{
			_spaceID = value;
		}    
    }

	public Vector3 moveDirection
	{
		get
		{
			return moveMotor.moveDirection;
		}

	}



	public void entityEnable(KBEngine.IServerEntity entity)
	{
		entityEnabled = true;
		logicEntity = entity;
	}

	public void entityDisable()
	{
		entityEnabled = false;
	}

	public void set_state(sbyte v)
	{
		if (v == 3) 
		{
			if(isPlayer)
				gameObject.transform.Find ("Graphics").GetComponent<MeshRenderer> ().material.color = Color.green;
			else
				gameObject.transform.Find ("Graphics").GetComponent<MeshRenderer> ().material.color = Color.red;
		} else if (v == 0) 
		{
			if(isPlayer)
				gameObject.transform.Find ("Graphics").GetComponent<MeshRenderer> ().material.color = Color.blue;
			else
				gameObject.transform.Find ("Graphics").GetComponent<MeshRenderer> ().material.color = Color.white;
		} else if (v == 1) {
			gameObject.transform.Find ("Graphics").GetComponent<MeshRenderer> ().material.color = Color.black;
		}
	}

    
	void Update () 
	{
		position = gameObject.transform.position;
		eulerAngles = gameObject.transform.eulerAngles;


		if (isPlayer == true && isControlled == false)
		{
			if(isOnGround != characterController.isGrounded)
			{
		    	KBEngine.Entity player = KBEngineApp.app.player();
		    	player.isOnGround = characterController.isGrounded;
		    	isOnGround = characterController.isGrounded;
		    }

		}
		KBEngine.Monster mon = logicEntity as KBEngine.Monster;
        if (mon != null)
        {
			isOnGround = mon.isOnGround;
		}
	}


	public void setMoveType(MoveConst moveType)
	{
		moveMotor.setMoveType(moveType);
	}

	public void setEnitiyInbattle(bool inBattle)
	{
		moveMotor.setInBattle(inBattle);
	}

	public void setEntityInUseSkill(int skillid)
	{
		moveMotor.setInUseSkill(skillid);
	}

	public void setEntityFinishSkill(int skillid)
	{
		moveMotor.setFinishSkill(skillid);
	}

	public void setEntityFaceDirection(VECTOR3 faceDirection)
	{
		moveMotor.setFaceDirection(faceDirection);
	}

	public void setEntityMoveDirection(VECTOR3 moveDirection)
	{
		moveMotor.setMoveDirection(moveDirection);
	}

	public void setAiMovePath(PATH_POINTS param, uint _pathIndex)
	{
		moveMotor.setAiMovePath(param, _pathIndex);
	}

	public void setAiMoveTarget(Int32 entityId)
	{
		Entity entity = KBEngineApp.app.findEntity(entityId);
		var renderEntity = entity.renderObj as UnityEngine.GameObject;
		moveMotor.setAiMoveTarget(renderEntity.transform);
	}

	public void setAiMovType(AiMoveConst aiMoveType)
	{
		moveMotor.setAiMovType(aiMoveType);
	}

	public void setAiMovPoint(Vector3 movePostion)
	{
		moveMotor.setAiMovPoint(movePostion);
	}

	public void confirmMoveTimeStamp(float timeStamp, MoveConst moveType, Vector3 position, Vector3 direction, Vector3 moveDirection, bool inBattle)
	{
		moveMotor.confirmMoveTimeStamp(timeStamp, moveType, position, direction, moveDirection, inBattle);
	}

	public void OnJump()
	{
		Debug.Log("jumpState: " + jumpState);
		
		if(jumpState != 0)
			return;
		
		jumpState = 1;
	}

	public void createWeaponEvent(int tag)
	{
		
		GameObject Prefab = (GameObject)Resources.Load("Prefabs/Weapon/GreatSword_01");
		GameObject weapon = Instantiate(Prefab, Vector3.zero, Quaternion.identity) as GameObject;
		weapon.transform.SetParent(hand_r.transform, false);
		
	}

	public void removeWeaponEvent(int tag)
	{
		Debug.Log("RemoveWeaponEvent:" + tag);
		var sub = hand_r.transform.GetComponentsInChildren<Transform>();
        foreach (var item in sub)
        {
            if (item.gameObject == hand_r)
            {
				continue;
            }
			Destroy(item.gameObject);
        }
	}

	public void palyerAnimation(string animationName)
	{
		moveMotor.animatorController.playerSkillAttackAnimatior(moveMotor.animator, animationName);
	}

	public void setAnimationFloatParam(string parmName, float value)
	{
		moveMotor.animator.SetFloat(parmName, value);
	}




	public Dictionary<string, PlayableBinding> bindings = new Dictionary<string, PlayableBinding>();

	public void palyerTimeLine(string timeLineName)
	{
		TimelineAsset asset = Resources.Load<TimelineAsset>("TimeLine/Skill/" + timeLineName);
        foreach (var item in asset.outputs)
        {
			m_playabledirector.SetGenericBinding(item.sourceObject, gameObject);
			
		}
		m_playabledirector.Play(asset);

	}

}

