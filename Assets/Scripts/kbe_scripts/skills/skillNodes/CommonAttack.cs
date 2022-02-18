using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KBEngine;
using KBEngine.GameobjectHolder;

namespace KBEngine
{
    public class CommonAttack : SkillNodeBase
    {
        uint attackBoxId;
       
        public CommonAttack(float timeStamp) : base(timeStamp)
        {
       
        }

        public override void Run()
        {
            Avatar a = ((skillTimeLine)owneTimeLine).ownerEntity as Avatar;
            attackBoxId = World.world.createObject("Prefabs/Common/BoxTriger",Utils.localTime() + 2.5f );
            GameobjectHolderBase obj = World.world.getObject(attackBoxId);
            World.world.addObjectCallback(attackBoxId, onAttack);
            World.world.setObjectParentToEntity(attackBoxId, a.renderEntity);

        }

        public override void OnDestory()
        {
            World.world.deleteObject(attackBoxId);
        }
        public void onAttack(ObjectArgeBase arge)
        {
            AttackBoxArges _r = arge as AttackBoxArges;
            GameEntity gameEntity = _r.gameEntity;
            Dbg.DEBUG_MSG("CommonAttack:onAttack" + _r.type);
        }


    }
}
