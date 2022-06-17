
using KBEngine;
using UnityEngine;
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
            attackBoxId = World.world.createObject("Prefabs/Common/BoxTriger", Utils.localTime() + 0.5f);
            GameobjectHolderBase obj = World.world.getObject(attackBoxId);
            World.world.addObjectCallback(attackBoxId, onAttack);
            World.world.setObjectParentToEntity(attackBoxId, avatarOwner.renderEntity);
        }

        public override void runMonster()
        {
            if (avatarOwner.isBeControl())
            {
                runP1();
            }
        }

        public override void serverCall(TABLE args)
        {
            INT32 attackeEntityId = (INT32) args.values[0];
            Dbg.DEBUG_MSG("CommonAttack:serverCall" + attackeEntityId);
            var entity = KBEngineApp.app.findEntity(attackeEntityId) as IServerEntity;
            entity.renderEntity.setAnimationFloatParam("Param1", args.values[1]);
            entity.renderEntity.setAnimationFloatParam("Param2", args.values[2]);
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
           
            var entity = KBEngineApp.app.findEntity(attackeEntityId) as Entity;
            var entity_R = KBEngineApp.app.findEntity(attackeEntityId) as IServerEntity;
            Dbg.DEBUG_MSG("CommonAttack:onAttack" + _r.type);
            //entity.renderEntity.palyerAnimation("Attack.BeGreatSword_Attack01");

            TABLE arg = new TABLE();
            arg.dictOrlist = 0;
            arg.values.Add(attackeEntityId);

            var hitTar = entity.position - ((Entity)avatarOwner).position;
            hitTar.y = 0;
            hitTar.Normalize();
            var inverstRotation = Quaternion.Inverse(entity_R.renderEntity.rotation);
            hitTar = inverstRotation * hitTar;

            arg.values.Add(hitTar.x);
            arg.values.Add(hitTar.z);

            ((skillTimeLine)owneTimeLine).callServer(nodeId, arg);
        }


    }
}
