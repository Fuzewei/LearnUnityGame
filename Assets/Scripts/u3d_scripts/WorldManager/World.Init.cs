using System;
using System.Collections.Generic;
using KBEngine;
using UnityEngine;
using KBEngine.GameobjectHolder;

//初始化类
public partial class World : MonoBehaviour
{
    private void WorldInit()
    {
        SkillFactory.Init();
    }
}
