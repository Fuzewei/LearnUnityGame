namespace KBEngine
{
	using GameLogic;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Const;
	public partial class Avatar : AvatarBase, IServerEntity
	{
		private void __init__CombatProperty()
		{

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



	}
}