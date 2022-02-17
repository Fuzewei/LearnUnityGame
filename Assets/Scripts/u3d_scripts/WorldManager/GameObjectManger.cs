using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using KBEngine.GameobjectHolder;

//场景管理类GameObjectManager
public partial class World : MonoBehaviour
{
    private static uint objectId = 0;
   
    Dictionary<uint, GameobjectHolderBase> worldObjects;

    private uint getNewUUid()
    {
        return objectId++;
    }

    void AwakeGameObjectManager()
    {
        worldObjects = new Dictionary<uint, GameobjectHolderBase>();
    }
}
