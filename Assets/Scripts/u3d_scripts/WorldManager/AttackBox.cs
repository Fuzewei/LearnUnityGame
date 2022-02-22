using System.Collections;
using KBEngine.GameobjectHolder;
using UnityEngine;
using System;

namespace KBEngine.GameobjectHolder
{
    public class AttackBoxArges : ObjectArgeBase
    {
        public uint type;
        public Int32 entityId;
    }
}

public class AttackBox : GameobjectHolderBase
{

    void OnTriggerEnter(Collider other)
    {
        AttackBoxArges args = new AttackBoxArges();
        GameEntity ownerEntity = gameObject.GetComponentInParent<GameEntity>();
        args.type = 1;
        if (ownerEntity != null && other.gameObject != ownerEntity.gameObject)
        {
            var showEntity = other.GetComponent<GameEntity>();
            var avatar = showEntity.logicEntity as KBEngine.Avatar;
            if (avatar != null)
            {
                args.entityId = avatar.id;
                notice(args);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
    }
}
