
using UnityEngine;
using KBEngine;
using KBEngine.Const;
using System.Collections;
using System;
using System.Xml;
using System.Collections.Generic;

public class GameEntity : MonoBehaviour 
{
	public bool isPlayer = false;
	private IServerEntity _logicEntity;
	
	private Vector3 _position = Vector3.zero;
	private Vector3 _eulerAngles = Vector3.zero;
	private Vector3 _scale = Vector3.zero;
	private UInt32 _spaceID = 0;
	
	public Vector3 destPosition = Vector3.zero;
	public Vector3 destDirection = Vector3.zero;
	
	private float _speed = 0f;
	private byte jumpState = 0;
	private float currY = 1.0f;
	
	private Camera playerCamera = null;
	
	public string entity_name;
	
	public string hp = "100/100";
	
	float npcHeight = 2.0f;
	
	public CharacterController characterController;
	public MoveMotor moveMotor;

	public MoveConst moveType;


	public bool isOnGround = true;

	public bool isControlled = false;
	
	public bool entityEnabled = true;

	//位置引用
	public GameObject hand_r;


	void Awake ()   
	{
	}
	
	void Start() 
	{
		characterController = ((UnityEngine.GameObject)gameObject).GetComponent<CharacterController>();
		moveMotor = GetComponent<MoveMotor>();
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



	public void entityEnable(KBEngine.Avatar entity)
	{
		entityEnabled = true;
		logicEntity = entity;
		logicEntity.onRenderObjectCreat(this);
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
		
	}

	public SampleBase getMoveSample()
	{
		return moveMotor.localOpQueue.end();
	}

	public void setMoveType(MoveConst moveType)
	{
		moveMotor.setMoveType(moveType);
	}



	public void setEnitiyInbattle(bool inBattle)
	{
		moveMotor.setInBattle(inBattle);
	}

	public void setEnitiyInUseSkill(int skillid)
	{
		moveMotor.setInUseSkill(skillid);
	}

	public void setEnitiyFinishSkill(int skillid)
	{
		moveMotor.setFinishSkill(skillid);
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
		KBEngine.Avatar a = (KBEngine.Avatar)logicEntity;
		GameObject Prefab = (GameObject)Resources.Load("Prefabs/Weapon/GreatSword_01");
		GameObject weapon = Instantiate(Prefab, Vector3.zero, Quaternion.identity) as GameObject;
		weapon.transform.SetParent(hand_r.transform, false);
		Debug.Log("CreateWeaponEvent:" + a.inBattle);
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

}

