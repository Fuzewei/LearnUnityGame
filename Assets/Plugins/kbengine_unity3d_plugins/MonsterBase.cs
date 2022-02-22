/*
	Generated by KBEngine!
	Please do not modify this file!
	Please inherit this module, such as: (class Monster : MonsterBase)
	tools = kbcmd
*/

namespace KBEngine
{
	using UnityEngine;
	using System;
	using System.Collections;
	using System.Collections.Generic;

	// defined in */scripts/entity_defs/Monster.def
	// Please inherit and implement "class Monster : MonsterBase"
	public abstract class MonsterBase : Entity
	{
		public EntityBaseEntityCall_MonsterBase baseEntityCall = null;
		public EntityCellEntityCall_MonsterBase cellEntityCall = null;

		public Int32 HP = 0;
		public virtual void onHPChanged(Int32 oldValue) {}
		public Int32 HP_Max = 0;
		public virtual void onHP_MaxChanged(Int32 oldValue) {}
		public Int32 MP = 0;
		public virtual void onMPChanged(Int32 oldValue) {}
		public Int32 MP_Max = 0;
		public virtual void onMP_MaxChanged(Int32 oldValue) {}
		public UInt32 entityNO = 0;
		public virtual void onEntityNOChanged(UInt32 oldValue) {}
		public Int32 forbids = 0;
		public virtual void onForbidsChanged(Int32 oldValue) {}
		public Byte inBattle = 0;
		public virtual void onInBattleChanged(Byte oldValue) {}
		public UInt32 modelID = 0;
		public virtual void onModelIDChanged(UInt32 oldValue) {}
		public Byte modelScale = 30;
		public virtual void onModelScaleChanged(Byte oldValue) {}
		public Byte moveSpeed = 50;
		public virtual void onMoveSpeedChanged(Byte oldValue) {}
		public string name = "";
		public virtual void onNameChanged(string oldValue) {}
		public SByte state = 0;
		public virtual void onStateChanged(SByte oldValue) {}
		public Byte subState = 0;
		public virtual void onSubStateChanged(Byte oldValue) {}
		public UInt32 uid = 0;
		public virtual void onUidChanged(UInt32 oldValue) {}
		public UInt32 utype = 0;
		public virtual void onUtypeChanged(UInt32 oldValue) {}

		public abstract void recvDamage(Int32 arg1, Int32 arg2, Int32 arg3, Int32 arg4); 
		public abstract void skillNodeCallClient(UInt32 arg1, Int32 arg2, TABLE arg3); 

		public MonsterBase()
		{
		}

		public override void onComponentsEnterworld()
		{
		}

		public override void onComponentsLeaveworld()
		{
		}

		public override void onGetBase()
		{
			baseEntityCall = new EntityBaseEntityCall_MonsterBase(id, className);
		}

		public override void onGetCell()
		{
			cellEntityCall = new EntityCellEntityCall_MonsterBase(id, className);
		}

		public override void onLoseCell()
		{
			cellEntityCall = null;
		}

		public override EntityCall getBaseEntityCall()
		{
			return baseEntityCall;
		}

		public override EntityCall getCellEntityCall()
		{
			return cellEntityCall;
		}

		public override void attachComponents()
		{
		}

		public override void detachComponents()
		{
		}

		public override void onRemoteMethodCall(MemoryStream stream)
		{
			ScriptModule sm = EntityDef.moduledefs["Monster"];

			UInt16 methodUtype = 0;
			UInt16 componentPropertyUType = 0;

			if(sm.usePropertyDescrAlias)
			{
				componentPropertyUType = stream.readUint8();
			}
			else
			{
				componentPropertyUType = stream.readUint16();
			}

			if(sm.useMethodDescrAlias)
			{
				methodUtype = stream.readUint8();
			}
			else
			{
				methodUtype = stream.readUint16();
			}

			Method method = null;

			if(componentPropertyUType == 0)
			{
				method = sm.idmethods[methodUtype];
			}
			else
			{
				Property pComponentPropertyDescription = sm.idpropertys[componentPropertyUType];
				switch(pComponentPropertyDescription.properUtype)
				{
					default:
						break;
				}

				return;
			}

			switch(method.methodUtype)
			{
				case 44:
					Int32 recvDamage_arg1 = stream.readInt32();
					Int32 recvDamage_arg2 = stream.readInt32();
					Int32 recvDamage_arg3 = stream.readInt32();
					Int32 recvDamage_arg4 = stream.readInt32();
					recvDamage(recvDamage_arg1, recvDamage_arg2, recvDamage_arg3, recvDamage_arg4);
					break;
				case 45:
					UInt32 skillNodeCallClient_arg1 = stream.readUint32();
					Int32 skillNodeCallClient_arg2 = stream.readInt32();
					TABLE skillNodeCallClient_arg3 = ((DATATYPE_TABLE)method.args[2]).createFromStreamEx(stream);
					skillNodeCallClient(skillNodeCallClient_arg1, skillNodeCallClient_arg2, skillNodeCallClient_arg3);
					break;
				default:
					break;
			};
		}

		public override void onUpdatePropertys(MemoryStream stream)
		{
			ScriptModule sm = EntityDef.moduledefs["Monster"];
			Dictionary<UInt16, Property> pdatas = sm.idpropertys;

			while(stream.length() > 0)
			{
				UInt16 _t_utype = 0;
				UInt16 _t_child_utype = 0;

				{
					if(sm.usePropertyDescrAlias)
					{
						_t_utype = stream.readUint8();
						_t_child_utype = stream.readUint8();
					}
					else
					{
						_t_utype = stream.readUint16();
						_t_child_utype = stream.readUint16();
					}
				}

				Property prop = null;

				if(_t_utype == 0)
				{
					prop = pdatas[_t_child_utype];
				}
				else
				{
					Property pComponentPropertyDescription = pdatas[_t_utype];
					switch(pComponentPropertyDescription.properUtype)
					{
						default:
							break;
					}

					return;
				}

				switch(prop.properUtype)
				{
					case 47001:
						Int32 oldval_HP = HP;
						HP = stream.readInt32();

						if(prop.isBase())
						{
							if(inited)
								onHPChanged(oldval_HP);
						}
						else
						{
							if(inWorld)
								onHPChanged(oldval_HP);
						}

						break;
					case 47002:
						Int32 oldval_HP_Max = HP_Max;
						HP_Max = stream.readInt32();

						if(prop.isBase())
						{
							if(inited)
								onHP_MaxChanged(oldval_HP_Max);
						}
						else
						{
							if(inWorld)
								onHP_MaxChanged(oldval_HP_Max);
						}

						break;
					case 47003:
						Int32 oldval_MP = MP;
						MP = stream.readInt32();

						if(prop.isBase())
						{
							if(inited)
								onMPChanged(oldval_MP);
						}
						else
						{
							if(inWorld)
								onMPChanged(oldval_MP);
						}

						break;
					case 47004:
						Int32 oldval_MP_Max = MP_Max;
						MP_Max = stream.readInt32();

						if(prop.isBase())
						{
							if(inited)
								onMP_MaxChanged(oldval_MP_Max);
						}
						else
						{
							if(inWorld)
								onMP_MaxChanged(oldval_MP_Max);
						}

						break;
					case 40001:
						Vector3 oldval_direction = direction;
						direction = stream.readVector3();

						if(prop.isBase())
						{
							if(inited)
								onDirectionChanged(oldval_direction);
						}
						else
						{
							if(inWorld)
								onDirectionChanged(oldval_direction);
						}

						break;
					case 51007:
						UInt32 oldval_entityNO = entityNO;
						entityNO = stream.readUint32();

						if(prop.isBase())
						{
							if(inited)
								onEntityNOChanged(oldval_entityNO);
						}
						else
						{
							if(inWorld)
								onEntityNOChanged(oldval_entityNO);
						}

						break;
					case 47005:
						Int32 oldval_forbids = forbids;
						forbids = stream.readInt32();

						if(prop.isBase())
						{
							if(inited)
								onForbidsChanged(oldval_forbids);
						}
						else
						{
							if(inWorld)
								onForbidsChanged(oldval_forbids);
						}

						break;
					case 39:
						Byte oldval_inBattle = inBattle;
						inBattle = stream.readUint8();

						if(prop.isBase())
						{
							if(inited)
								onInBattleChanged(oldval_inBattle);
						}
						else
						{
							if(inWorld)
								onInBattleChanged(oldval_inBattle);
						}

						break;
					case 41006:
						UInt32 oldval_modelID = modelID;
						modelID = stream.readUint32();

						if(prop.isBase())
						{
							if(inited)
								onModelIDChanged(oldval_modelID);
						}
						else
						{
							if(inWorld)
								onModelIDChanged(oldval_modelID);
						}

						break;
					case 41007:
						Byte oldval_modelScale = modelScale;
						modelScale = stream.readUint8();

						if(prop.isBase())
						{
							if(inited)
								onModelScaleChanged(oldval_modelScale);
						}
						else
						{
							if(inWorld)
								onModelScaleChanged(oldval_modelScale);
						}

						break;
					case 36:
						Byte oldval_moveSpeed = moveSpeed;
						moveSpeed = stream.readUint8();

						if(prop.isBase())
						{
							if(inited)
								onMoveSpeedChanged(oldval_moveSpeed);
						}
						else
						{
							if(inWorld)
								onMoveSpeedChanged(oldval_moveSpeed);
						}

						break;
					case 41003:
						string oldval_name = name;
						name = stream.readUnicode();

						if(prop.isBase())
						{
							if(inited)
								onNameChanged(oldval_name);
						}
						else
						{
							if(inWorld)
								onNameChanged(oldval_name);
						}

						break;
					case 40000:
						Vector3 oldval_position = position;
						position = stream.readVector3();

						if(prop.isBase())
						{
							if(inited)
								onPositionChanged(oldval_position);
						}
						else
						{
							if(inWorld)
								onPositionChanged(oldval_position);
						}

						break;
					case 40002:
						stream.readUint32();
						break;
					case 47006:
						SByte oldval_state = state;
						state = stream.readInt8();

						if(prop.isBase())
						{
							if(inited)
								onStateChanged(oldval_state);
						}
						else
						{
							if(inWorld)
								onStateChanged(oldval_state);
						}

						break;
					case 47007:
						Byte oldval_subState = subState;
						subState = stream.readUint8();

						if(prop.isBase())
						{
							if(inited)
								onSubStateChanged(oldval_subState);
						}
						else
						{
							if(inWorld)
								onSubStateChanged(oldval_subState);
						}

						break;
					case 41004:
						UInt32 oldval_uid = uid;
						uid = stream.readUint32();

						if(prop.isBase())
						{
							if(inited)
								onUidChanged(oldval_uid);
						}
						else
						{
							if(inWorld)
								onUidChanged(oldval_uid);
						}

						break;
					case 41005:
						UInt32 oldval_utype = utype;
						utype = stream.readUint32();

						if(prop.isBase())
						{
							if(inited)
								onUtypeChanged(oldval_utype);
						}
						else
						{
							if(inWorld)
								onUtypeChanged(oldval_utype);
						}

						break;
					default:
						break;
				};
			}
		}

		public override void callPropertysSetMethods()
		{
			ScriptModule sm = EntityDef.moduledefs["Monster"];
			Dictionary<UInt16, Property> pdatas = sm.idpropertys;

			Int32 oldval_HP = HP;
			Property prop_HP = pdatas[4];
			if(prop_HP.isBase())
			{
				if(inited && !inWorld)
					onHPChanged(oldval_HP);
			}
			else
			{
				if(inWorld)
				{
					if(prop_HP.isOwnerOnly() && !isPlayer())
					{
					}
					else
					{
						onHPChanged(oldval_HP);
					}
				}
			}

			Int32 oldval_HP_Max = HP_Max;
			Property prop_HP_Max = pdatas[5];
			if(prop_HP_Max.isBase())
			{
				if(inited && !inWorld)
					onHP_MaxChanged(oldval_HP_Max);
			}
			else
			{
				if(inWorld)
				{
					if(prop_HP_Max.isOwnerOnly() && !isPlayer())
					{
					}
					else
					{
						onHP_MaxChanged(oldval_HP_Max);
					}
				}
			}

			Int32 oldval_MP = MP;
			Property prop_MP = pdatas[6];
			if(prop_MP.isBase())
			{
				if(inited && !inWorld)
					onMPChanged(oldval_MP);
			}
			else
			{
				if(inWorld)
				{
					if(prop_MP.isOwnerOnly() && !isPlayer())
					{
					}
					else
					{
						onMPChanged(oldval_MP);
					}
				}
			}

			Int32 oldval_MP_Max = MP_Max;
			Property prop_MP_Max = pdatas[7];
			if(prop_MP_Max.isBase())
			{
				if(inited && !inWorld)
					onMP_MaxChanged(oldval_MP_Max);
			}
			else
			{
				if(inWorld)
				{
					if(prop_MP_Max.isOwnerOnly() && !isPlayer())
					{
					}
					else
					{
						onMP_MaxChanged(oldval_MP_Max);
					}
				}
			}

			Vector3 oldval_direction = direction;
			Property prop_direction = pdatas[2];
			if(prop_direction.isBase())
			{
				if(inited && !inWorld)
					onDirectionChanged(oldval_direction);
			}
			else
			{
				if(inWorld)
				{
					if(prop_direction.isOwnerOnly() && !isPlayer())
					{
					}
					else
					{
						onDirectionChanged(oldval_direction);
					}
				}
			}

			UInt32 oldval_entityNO = entityNO;
			Property prop_entityNO = pdatas[8];
			if(prop_entityNO.isBase())
			{
				if(inited && !inWorld)
					onEntityNOChanged(oldval_entityNO);
			}
			else
			{
				if(inWorld)
				{
					if(prop_entityNO.isOwnerOnly() && !isPlayer())
					{
					}
					else
					{
						onEntityNOChanged(oldval_entityNO);
					}
				}
			}

			Int32 oldval_forbids = forbids;
			Property prop_forbids = pdatas[9];
			if(prop_forbids.isBase())
			{
				if(inited && !inWorld)
					onForbidsChanged(oldval_forbids);
			}
			else
			{
				if(inWorld)
				{
					if(prop_forbids.isOwnerOnly() && !isPlayer())
					{
					}
					else
					{
						onForbidsChanged(oldval_forbids);
					}
				}
			}

			Byte oldval_inBattle = inBattle;
			Property prop_inBattle = pdatas[10];
			if(prop_inBattle.isBase())
			{
				if(inited && !inWorld)
					onInBattleChanged(oldval_inBattle);
			}
			else
			{
				if(inWorld)
				{
					if(prop_inBattle.isOwnerOnly() && !isPlayer())
					{
					}
					else
					{
						onInBattleChanged(oldval_inBattle);
					}
				}
			}

			UInt32 oldval_modelID = modelID;
			Property prop_modelID = pdatas[11];
			if(prop_modelID.isBase())
			{
				if(inited && !inWorld)
					onModelIDChanged(oldval_modelID);
			}
			else
			{
				if(inWorld)
				{
					if(prop_modelID.isOwnerOnly() && !isPlayer())
					{
					}
					else
					{
						onModelIDChanged(oldval_modelID);
					}
				}
			}

			Byte oldval_modelScale = modelScale;
			Property prop_modelScale = pdatas[12];
			if(prop_modelScale.isBase())
			{
				if(inited && !inWorld)
					onModelScaleChanged(oldval_modelScale);
			}
			else
			{
				if(inWorld)
				{
					if(prop_modelScale.isOwnerOnly() && !isPlayer())
					{
					}
					else
					{
						onModelScaleChanged(oldval_modelScale);
					}
				}
			}

			Byte oldval_moveSpeed = moveSpeed;
			Property prop_moveSpeed = pdatas[13];
			if(prop_moveSpeed.isBase())
			{
				if(inited && !inWorld)
					onMoveSpeedChanged(oldval_moveSpeed);
			}
			else
			{
				if(inWorld)
				{
					if(prop_moveSpeed.isOwnerOnly() && !isPlayer())
					{
					}
					else
					{
						onMoveSpeedChanged(oldval_moveSpeed);
					}
				}
			}

			string oldval_name = name;
			Property prop_name = pdatas[14];
			if(prop_name.isBase())
			{
				if(inited && !inWorld)
					onNameChanged(oldval_name);
			}
			else
			{
				if(inWorld)
				{
					if(prop_name.isOwnerOnly() && !isPlayer())
					{
					}
					else
					{
						onNameChanged(oldval_name);
					}
				}
			}

			Vector3 oldval_position = position;
			Property prop_position = pdatas[1];
			if(prop_position.isBase())
			{
				if(inited && !inWorld)
					onPositionChanged(oldval_position);
			}
			else
			{
				if(inWorld)
				{
					if(prop_position.isOwnerOnly() && !isPlayer())
					{
					}
					else
					{
						onPositionChanged(oldval_position);
					}
				}
			}

			SByte oldval_state = state;
			Property prop_state = pdatas[15];
			if(prop_state.isBase())
			{
				if(inited && !inWorld)
					onStateChanged(oldval_state);
			}
			else
			{
				if(inWorld)
				{
					if(prop_state.isOwnerOnly() && !isPlayer())
					{
					}
					else
					{
						onStateChanged(oldval_state);
					}
				}
			}

			Byte oldval_subState = subState;
			Property prop_subState = pdatas[16];
			if(prop_subState.isBase())
			{
				if(inited && !inWorld)
					onSubStateChanged(oldval_subState);
			}
			else
			{
				if(inWorld)
				{
					if(prop_subState.isOwnerOnly() && !isPlayer())
					{
					}
					else
					{
						onSubStateChanged(oldval_subState);
					}
				}
			}

			UInt32 oldval_uid = uid;
			Property prop_uid = pdatas[17];
			if(prop_uid.isBase())
			{
				if(inited && !inWorld)
					onUidChanged(oldval_uid);
			}
			else
			{
				if(inWorld)
				{
					if(prop_uid.isOwnerOnly() && !isPlayer())
					{
					}
					else
					{
						onUidChanged(oldval_uid);
					}
				}
			}

			UInt32 oldval_utype = utype;
			Property prop_utype = pdatas[18];
			if(prop_utype.isBase())
			{
				if(inited && !inWorld)
					onUtypeChanged(oldval_utype);
			}
			else
			{
				if(inWorld)
				{
					if(prop_utype.isOwnerOnly() && !isPlayer())
					{
					}
					else
					{
						onUtypeChanged(oldval_utype);
					}
				}
			}

		}
	}
}