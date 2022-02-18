using System.Collections;
using KBEngine.GameobjectHolder;
using UnityEngine;

namespace KBEngine.GameobjectHolder
{
    public class AttackBoxArges : ObjectArgeBase
    {
        public uint type;
        public GameEntity gameEntity;
    }
}

public class AttackBox : GameobjectHolderBase
{

    void OnTriggerEnter(Collider other)
    {
        AttackBoxArges args = new AttackBoxArges();
        args.type = 1;
        args.gameEntity = other.GetComponent<GameEntity>();
        notice(args); 
    }

    void OnTriggerExit(Collider other)
    {
        AttackBoxArges args = new AttackBoxArges();
        args.type = 2;
    }
}
