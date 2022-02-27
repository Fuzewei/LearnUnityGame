namespace KBEngine
{
	using UnityEngine;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Const;


	public partial class Avatar : AvatarBase, IServerEntity
	{
		public static SkillBox skillbox = new SkillBox();
		public GameEntity renderEntity; //和渲染层交互的接口对象
		public Vector3 _renderPosition; //渲染层位置
		public Vector3 _renderRotation; //渲染层朝向

		public float postitonConfirmTimer; //服务端同步位置数据确认时间


		//渲染层对象的位置，只有渲染对象创建后才有效
		public Vector3 renderPosition
		{
			get { return renderEntity.position; }
			set { renderEntity.position = value; }
		}
		//渲染层对象的旋转，只有渲染对象创建后才有效
		public Vector3 renderRotation
		{
			get {
				Vector3 ans = renderEntity.eulerAngles;
				var y = (double)(ans.y / 360 * (System.Math.PI * 2));
				if (y - System.Math.PI > 0.0)
					y -= System.Math.PI * 2;
				ans.y = (float)y;
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


		public Avatar() : base()
		{
		}

		public override void __init__()
		{
			// 由于任何玩家被同步到该客户端都会使用这个模块创建，因此客户端可能存在很多这样的实体
			// 但只有一个是自己的玩家实体，所以需要判断一下
			__init__Combat();
			__init__CombatProperty();
			if (isPlayer())
			{
				__init__PlayerAvatar();
				__init__InputManage();
			}

			postitonConfirmTimer = Utils.localTime();
		}
		private void __init__PlayerAvatar()
		{
			Event.registerIn("relive", this, "relive");
			Event.registerIn("jump", this, "jump");
		}
		
		//渲染层准备就绪
		public void onRenderObjectCreat(GameEntity render)
		{
			renderEntity = render;
			Event.fireOut("onRenderObjectCreat", this);
			if (isPlayer())
            {
				TimerUtils.addTimer(0.1f, 0.1f, new TimerCallback(uploadPositionAndRotation));
			}
		}

		public override void onDestroy()
		{
			if (isPlayer())
			{
				KBEngine.Event.deregisterIn(this);
			}
		}

		public void relive(Byte type)
		{
			cellEntityCall.relive(type);
		}


		public override void onLevelChanged(UInt16 oldValue)
		{
			// Dbg.DEBUG_MSG(className + "::set_level: " + old + " => " + v); 
			Event.fireOut("set_level", new object[] { this, level });
		}

		public override void onNameChanged(string oldValue)
		{
			// Dbg.DEBUG_MSG(className + "::set_name: " + old + " => " + v); 
			Event.fireOut("set_name", new object[] { this, name });
		}

		public override void onStateChanged(SByte oldValue)
		{
			Dbg.DEBUG_MSG(className + "::set_state: " + oldValue + " => " + state);
			Event.fireOut("set_state", new object[] { this, state });
		}

		public override void onMoveSpeedChanged(Byte oldValue)
		{
			// Dbg.DEBUG_MSG(className + "::set_moveSpeed: " + oldValue + " => " + moveSpeed); 
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

		public override void onEnterWorld()
		{
			base.onEnterWorld();

			// 当玩家进入世界时，请求获取自己的技能列表
			if (isPlayer())
			{
				Event.fireOut("onAvatarEnterWorld", new object[] { KBEngineApp.app.entity_uuid, id, this });
				SkillBox.inst.pull();
			}
		}

		public void jump()
		{
			cellEntityCall.jump();
		}

		public override void onJump()
		{
			Dbg.DEBUG_MSG(className + "::onJump: " + id);
			Event.fireOut("otherAvatarOnJump", new object[] { this });
		}

		public void uploadMovetype(MoveConst moveTye)
		{
			cellEntityCall.updateMovetype(Utils.localTime(), (uint)moveTye);
			switch (moveTye)
			{
				case MoveConst.Idel:
					break;
				case MoveConst.Walk:
					break;
				case MoveConst.Run:
					break;
				case MoveConst.Jump:
					break;
				case MoveConst.Rush:
					break;
				case MoveConst.Skill:
					requestUseSkill(preUseSkillId);
					break;
				default:
					break;
			}
		}

		public void uploadPostionAndRotation()
		{
			var sample = renderEntity.getMoveSample();
			cellEntityCall.setPostionAndRotation(sample.position, sample.faceDirection, sample.moveDirection);
		}

		public void uploadPositionAndRotation(params object[] args)
		{
			// 更加安全的更新位置，避免将上一个场景的坐标更新到当前场景中的玩家
			//if (currSpaceID > 0 && currSpaceID != KBEngineApp.app.spaceID)
			//{
			//	return;
			//}
			Dbg.DEBUG_MSG("uploadPositionAndRotation:" + renderPosition + " ," + renderRotation);
			cellEntityCall.updatePosition(Utils.localTime(), renderPosition, renderRotation, renderMoveDirection);
		}

		//服务器确认的位置
		public override void confirmMoveTimeStamp(float timeStamp)
		{
			//Dbg.DEBUG_MSG(timeStamp + "confirmMoveTimeStamp:: " + moveType);
			postitonConfirmTimer = timeStamp;
			renderEntity.confirmMoveTimeStamp(timeStamp, (MoveConst)moveType, position, direction, moveDirection, Convert.ToBoolean(inBattle));
		}

		public override void onPositionChanged(Vector3 oldValue)
		{
			base.onPositionChanged(oldValue);
		}

		public override void onAddSkill(Int32 skillID)
		{
			Dbg.DEBUG_MSG(className + "::onAddSkill(" + skillID + ")");
			Event.fireOut("onAddSkill", new object[] { this });

			Skill skill = new Skill();
			skill.id = skillID;
			skill.name = skillID + " ";
			switch (skillID)
			{
				case 1:
					break;
				case 1000101:
					skill.canUseDistMax = 20f;
					break;
				case 2000101:
					skill.canUseDistMax = 20f;
					break;
				case 3000101:
					skill.canUseDistMax = 20f;
					break;
				case 4000101:
					skill.canUseDistMax = 20f;
					break;
				case 5000101:
					skill.canUseDistMax = 20f;
					break;
				case 6000101:
					skill.canUseDistMax = 20f;
					break;
				default:
					break;
			};

			SkillBox.inst.add(skill);
		}

		public override void onRemoveSkill(Int32 skillID)
		{
			Dbg.DEBUG_MSG(className + "::onRemoveSkill(" + skillID + ")");
			Event.fireOut("onRemoveSkill", new object[] { this });
			SkillBox.inst.remove(skillID);
		}

		public override void dialog_addOption(Byte arg1, UInt32 arg2, string arg3, Int32 arg4) { }
		public override void dialog_close() { }
		public override void dialog_setText(string arg1, Byte arg2, UInt32 arg3, string arg4) { }



	}



} 
