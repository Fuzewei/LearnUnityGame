﻿using System;
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

        public override void serverCall(object args)
        {
            
        }

        public override void OnDestory()
        {
            World.world.deleteObject(attackBoxId);
        }
        public void onAttack(ObjectArgeBase arge)
        {
            AttackBoxArges _r = arge as AttackBoxArges;
            GameEntity attackeEntity = _r.gameEntity;
            if (attackeEntity == null)
            {
                return;
            }
            Avatar a = attackeEntity.logicEntity as Avatar;
            Dbg.DEBUG_MSG("CommonAttack:onAttack" + _r.type);
            if (attackeEntity != avatarOwner.renderEntity)
            {
                attackeEntity.palyerAnimation("BeGreatSword_Attack01");
            }
        }


    }
}
