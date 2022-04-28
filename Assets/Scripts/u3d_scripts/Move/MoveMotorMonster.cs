using System.Collections.Generic;
using UnityEngine;
using KBEngine.Const;
using KBEngine;
using SwordMaster;
using System;

class MoveMotorMonster : MoveMotor
{

    new protected void Update()
    {

    }

    protected override MoveControlersBase getMoveControl(MoveConst moveType)
    {
        MoveControlersBase ans = null;
        switch (moveType)
        {
            case MoveConst.Idel:
                break;
        }
        return ans;
    }
}

