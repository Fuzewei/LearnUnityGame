namespace KBEngine
{
  	using UnityEngine; 
	using System; 
	using System.Collections; 
	using System.Collections.Generic;

	public partial class Monster : MonsterBase, IServerEntity
	{
		public GameEntity _renderEntity;

		public GameEntity renderEntity
		{
			get { return _renderEntity; }
			set { _renderEntity = value; }
		}


		public Monster() : base()
		{
		}

		public override void __init__()
		{
			// 由于任何玩家被同步到该客户端都会使用这个模块创建，因此客户端可能存在很多这样的实体
			// 但只有一个是自己的玩家实体，所以需要判断一下
			__init__Combat();
			__init__Motion();
		}

		//渲染层准备就绪
		public void onRenderObjectCreate(GameEntity render)
		{
			renderEntity = render;
			Event.fireOut("onRenderObjectCreate", this);
			Dbg.DEBUG_MSG("Monster:onRenderObjectCreate:0");
			if (isBeControl() && p3MoveTimer == 0)
			{
				Dbg.DEBUG_MSG("Monster:onRenderObjectCreate:");
				p3MoveTimer = TimerUtils.addTimer(0.1f, 0.1f, new TimerCallback(uploadPositionAndRotation), Utils.localTime(), confirmTime);
			}
			confirmMoveTimeStamp(confirmTime);
		}

		public override void recvDamage(Int32 attackerID, Int32 skillID, Int32 damageType, Int32 damage)
		{
			// Dbg.DEBUG_MSG(className + "::recvDamage: attackerID=" + attackerID + ", skillID=" + skillID + ", damageType=" + damageType + ", damage=" + damage);

			Entity entity = KBEngineApp.app.findEntity(attackerID);

			Event.fireOut("recvDamage", new object[] { this, entity, skillID, damageType, damage });
		}

		public override void onHPChanged(Int32 oldValue)
		{
			// Dbg.DEBUG_MSG(className + "::set_HP: " + old + " => " + v); 
			Event.fireOut("set_HP", new object[] { this, HP, HP_Max });
		}

		public override void onMPChanged(Int32 oldValue)
		{
			// Dbg.DEBUG_MSG(className + "::set_MP: " + old + " => " + v); 
			Event.fireOut("set_MP", new object[] { this, MP, MP_Max });
		}

		public override void onHP_MaxChanged(Int32 oldValue)
		{
			// Dbg.DEBUG_MSG(className + "::set_HP_Max: " + old + " => " + v); 
			Event.fireOut("set_HP_Max", new object[] { this, HP_Max, HP });
		}

		public override void onMP_MaxChanged(Int32 oldValue)
		{
			// Dbg.DEBUG_MSG(className + "::set_MP_Max: " + old + " => " + v); 
			Event.fireOut("set_MP_Max", new object[] { this, MP_Max, MP });
		}

		public override void onNameChanged(string oldValue)
		{
			// Dbg.DEBUG_MSG(className + "::set_name: " + old + " => " + v); 
			Event.fireOut("set_name", new object[] { this, name });
		}

		public override void onStateChanged(SByte oldValue)
		{
			// Dbg.DEBUG_MSG(className + "::set_state: " + old + " => " + v); 
			Event.fireOut("set_state", new object[] { this, state });
		}

		public override void onMoveSpeedChanged(float oldValue)
		{
			Dbg.DEBUG_MSG(className + "::set_moveSpeed: " + oldValue + " => " + moveSpeed); 
			Event.fireOut("set_moveSpeed", new object[] { this, moveSpeed });
		}

		public override void onModelScaleChanged(Byte oldValue)
		{
			// Dbg.DEBUG_MSG(className + "::set_modelScale: " + old + " => " + v); 
			Event.fireOut("set_modelScale", new object[] { this, modelScale });
		}

		public override void onModelIDChanged(UInt32 oldValue)
		{
			// Dbg.DEBUG_MSG(className + "::set_modelID: " + old + " => " + v); 
			Event.fireOut("set_modelID", new object[] { this, modelID });
		}

		//！！！！！！！！！！！！！！后面要重构的,avatar和monster
		public override void skillNodeCallClient(UInt32 arg1, Int32 arg2, TABLE arg3)
		{

		}

		public Vector3 renderPosition
		{
			get { return renderEntity.position; }
			set { renderEntity.position = value; }
		}
		//渲染层对象的旋转，只有渲染对象创建后才有效
		public Vector3 renderRotation
		{
			get //unity上显示的rotation是-180到180，但是实际值是0-360，应该进行一个转换
			{
				Vector3 ans = renderEntity.eulerAngles;
				var y = (double)(ans.y / 360 * (System.Math.PI * 2));
				if (y - System.Math.PI > 0.0)
					y -= System.Math.PI * 2;
				ans.y = (float)y;
				ans.x = 0;
				ans.z = 0;
				return ans;
			}
			set { renderEntity.eulerAngles = value; }
		}
		public Vector3 renderMoveDirection
		{
			get
			{
				Vector3 ans = renderEntity.moveDirection;
				return ans;
			}

		}


	}
} 
