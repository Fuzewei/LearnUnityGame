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

        public override void runP1()
        {
            attackBoxId = World.world.createObject("Prefabs/Common/BoxTriger",Utils.localTime() + 0.5f );
            GameobjectHolderBase obj = World.world.getObject(attackBoxId);
            World.world.addObjectCallback(attackBoxId, onAttack);
            World.world.setObjectParentToEntity(attackBoxId, avatarOwner.renderEntity);
        }

        public override void runP3()
        {

        }

        public override void serverCall(TABLE args)
        {
            INT32 attackeEntityId = args.values[0];
            Dbg.DEBUG_MSG("CommonAttack:serverCall" + attackeEntityId);
            var entity = KBEngineApp.app.findEntity(attackeEntityId) as Avatar;
            entity.renderEntity.palyerAnimation("Attacked.BeGreatSword_Attack01");
        }

        //timeLine结束
        public override void OnDestory()
        {
            World.world.deleteObject(attackBoxId);
        }
        public void onAttack(ObjectArgeBase arge)
        {
            AttackBoxArges _r = arge as AttackBoxArges;
            INT32 attackeEntityId = _r.entityId;
           
            var entity = KBEngineApp.app.findEntity(attackeEntityId) as Avatar;
            Dbg.DEBUG_MSG("CommonAttack:onAttack" + _r.type);
            //entity.renderEntity.palyerAnimation("Attack.BeGreatSword_Attack01");

            TABLE arg = new TABLE();
            arg.dictOrlist = 0;
            arg.values.Add(attackeEntityId);

            ((skillTimeLine)owneTimeLine).callServer(1, arg);
        }


    }
}
