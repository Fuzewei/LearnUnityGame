using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KBEngine.GameobjectHolder
{

    public class GameobjectHolderBase
    {
        public uint objId;
        public GameObject obj;
        public Type script;
        public float destoryTimestamp = float.MaxValue;
        public GameobjectHolderBase(uint _id, GameObject _obj)
        {
            objId = _id;
            obj = _obj;
        }

        public void generate()
        {

        }
    }

    public class AttackBox: GameobjectHolderBase
    {
        public AttackBox(uint _id, GameObject _obj):base(_id, _obj)
        {
            script = typeof(BoxCollider);
        }

    }

}
