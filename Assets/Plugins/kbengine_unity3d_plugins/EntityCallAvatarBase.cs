/*
	Generated by KBEngine!
	Please do not modify this file!
	
	tools = kbcmd
*/

namespace KBEngine
{
	using UnityEngine;
	using System;
	using System.Collections;
	using System.Collections.Generic;

	// defined in */scripts/entity_defs/Avatar.def
	public class EntityBaseEntityCall_AvatarBase : EntityCall
	{
		public EntityBaseEntityCall_TestBase component1 = null;
		public EntityBaseEntityCall_TestBase component2 = null;
		public EntityBaseEntityCall_TestNoBaseBase component3 = null;

		public EntityBaseEntityCall_AvatarBase(Int32 eid, string ename) : base(eid, ename)
		{
			component1 = new EntityBaseEntityCall_TestBase(20, id);
			component2 = new EntityBaseEntityCall_TestBase(25, id);
			component3 = new EntityBaseEntityCall_TestNoBaseBase(26, id);
			type = ENTITYCALL_TYPE.ENTITYCALL_TYPE_BASE;
		}

	}

	public class EntityCellEntityCall_AvatarBase : EntityCall
	{
		public EntityCellEntityCall_TestBase component1 = null;
		public EntityCellEntityCall_TestBase component2 = null;
		public EntityCellEntityCall_TestNoBaseBase component3 = null;

		public EntityCellEntityCall_AvatarBase(Int32 eid, string ename) : base(eid, ename)
		{
			component1 = new EntityCellEntityCall_TestBase(20, id);
			component2 = new EntityCellEntityCall_TestBase(25, id);
			component3 = new EntityCellEntityCall_TestNoBaseBase(26, id);
			type = ENTITYCALL_TYPE.ENTITYCALL_TYPE_CELL;
		}

		public void clientRequestUseSkill(UInt32 arg1, Int32 arg2)
		{
			Bundle pBundle = newCall("clientRequestUseSkill", 0);
			if(pBundle == null)
				return;

			bundle.writeUint32(arg1);
			bundle.writeInt32(arg2);
			sendCall(null);
		}

		public void clientSkillFinish(Int32 arg1)
		{
			Bundle pBundle = newCall("clientSkillFinish", 0);
			if(pBundle == null)
				return;

			bundle.writeInt32(arg1);
			sendCall(null);
		}

		public void clientTimeLineFinish(UInt32 arg1)
		{
			Bundle pBundle = newCall("clientTimeLineFinish", 0);
			if(pBundle == null)
				return;

			bundle.writeUint32(arg1);
			sendCall(null);
		}

		public void dialog(Int32 arg1, UInt32 arg2)
		{
			Bundle pBundle = newCall("dialog", 0);
			if(pBundle == null)
				return;

			bundle.writeInt32(arg1);
			bundle.writeUint32(arg2);
			sendCall(null);
		}

		public void jump()
		{
			Bundle pBundle = newCall("jump", 0);
			if(pBundle == null)
				return;

			sendCall(null);
		}

		public void relive(Byte arg1)
		{
			Bundle pBundle = newCall("relive", 0);
			if(pBundle == null)
				return;

			bundle.writeUint8(arg1);
			sendCall(null);
		}

		public void requestPull()
		{
			Bundle pBundle = newCall("requestPull", 0);
			if(pBundle == null)
				return;

			sendCall(null);
		}

		public void setInBattle(float arg1, Byte arg2)
		{
			Bundle pBundle = newCall("setInBattle", 0);
			if(pBundle == null)
				return;

			bundle.writeFloat(arg1);
			bundle.writeUint8(arg2);
			sendCall(null);
		}

		public void setPostionAndRotation(Vector3 arg1, Vector3 arg2, Vector3 arg3)
		{
			Bundle pBundle = newCall("setPostionAndRotation", 0);
			if(pBundle == null)
				return;

			bundle.writeVector3(arg1);
			bundle.writeVector3(arg2);
			bundle.writeVector3(arg3);
			sendCall(null);
		}

		public void skillNodeCallServer(UInt32 arg1, Int32 arg2, TABLE arg3)
		{
			Bundle pBundle = newCall("skillNodeCallServer", 0);
			if(pBundle == null)
				return;

			bundle.writeUint32(arg1);
			bundle.writeInt32(arg2);
			((DATATYPE_TABLE)EntityDef.id2datatypes[34]).addToStreamEx(bundle, arg3);
			sendCall(null);
		}

		public void updateAvatarMoveState(float arg1, UInt32 arg2, Vector3 arg3, Vector3 arg4, Vector3 arg5, Byte arg6)
		{
			Bundle pBundle = newCall("updateAvatarMoveState", 0);
			if(pBundle == null)
				return;

			bundle.writeFloat(arg1);
			bundle.writeUint32(arg2);
			bundle.writeVector3(arg3);
			bundle.writeVector3(arg4);
			bundle.writeVector3(arg5);
			bundle.writeUint8(arg6);
			sendCall(null);
		}

		public void updateMovetype(float arg1, UInt32 arg2)
		{
			Bundle pBundle = newCall("updateMovetype", 0);
			if(pBundle == null)
				return;

			bundle.writeFloat(arg1);
			bundle.writeUint32(arg2);
			sendCall(null);
		}

		public void updatePosition(float arg1, Vector3 arg2, Vector3 arg3, Vector3 arg4)
		{
			Bundle pBundle = newCall("updatePosition", 0);
			if(pBundle == null)
				return;

			bundle.writeFloat(arg1);
			bundle.writeVector3(arg2);
			bundle.writeVector3(arg3);
			bundle.writeVector3(arg4);
			sendCall(null);
		}

		public void useTargetSkill(Int32 arg1, Int32 arg2)
		{
			Bundle pBundle = newCall("useTargetSkill", 0);
			if(pBundle == null)
				return;

			bundle.writeInt32(arg1);
			bundle.writeInt32(arg2);
			sendCall(null);
		}

	}
	}
