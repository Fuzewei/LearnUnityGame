using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KBEngine.Const;
using UnityEngine;

namespace SwordMaster
{
    class WalkSamples : SampleBase
    {
        public WalkSamples(Vector3 position, Vector3 faceDirection, Vector3 moveDirection, bool inBattle) : base(MoveConst.Walk, position, faceDirection, moveDirection, inBattle)
        {

        }

    }

    class IdelSamples : SampleBase
    {
        public IdelSamples(Vector3 position, Vector3 faceDirection, Vector3 moveDirection, bool inBattle) : base(MoveConst.Idel, position, faceDirection, moveDirection, inBattle)
        {

        }
    }

    class RunSamples : SampleBase
    {
        public RunSamples(Vector3 position, Vector3 faceDirection, Vector3 moveDirection, bool inBattle) : base(MoveConst.Run, position, faceDirection, moveDirection, inBattle)
        {

        }
    }

    class JumpSamples : SampleBase
    {
        public JumpSamples(Vector3 position, Vector3 faceDirection, Vector3 moveDirection, bool inBattle) : base(MoveConst.Jump, position, faceDirection, moveDirection, inBattle)
        {
        }
    }

   
}
