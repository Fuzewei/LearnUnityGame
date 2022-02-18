using System;
using System.Collections.Generic;
using KBEngine;
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

    void tickGameObjectManager()
    {
        List<uint> uuids = new List<uint>();
        foreach (var item in worldObjects)
        {
            if (item.Value.destoryTimestamp < Utils.localTime())
            {
                uuids.Add(item.Key);
            }
        }

        foreach (var item in uuids)
        {
            deleteObject(item);
        }
    }

    public uint createObject(string path, float destoryTimestamp = float.MaxValue)
    {
        var uuid = getNewUUid();
        GameObject Prefab = (GameObject)Resources.Load(path);
        GameObject obj = UnityEngine.Object.Instantiate(Prefab, Vector3.zero, Quaternion.identity) as GameObject;
        GameobjectHolderBase _t = obj.GetComponent<GameobjectHolderBase>();
        _t.objId = uuid;
        _t.destoryTimestamp = destoryTimestamp;
        worldObjects.Add(uuid, _t);
        return uuid;
    }
    public GameobjectHolderBase getObject(uint uuid)
    {
        GameobjectHolderBase ans;
        worldObjects.TryGetValue(uuid, out ans);
        return ans;
    }
    public void deleteObject(uint uuid)
    {
        if (!containObject(uuid))
        {
            return;
        }
        Destroy(worldObjects[uuid].gameObject);
        worldObjects.Remove(uuid);
    }

    public void addObjectCallback(uint uuid, noticeCallback cb)
    {
        worldObjects[uuid].addNoticeCallback(cb);
    }
    public void setObjectParentToEntity(uint uuid, GameEntity entity)
    {
        if (!containObject(uuid))
        {
            return;
        }
        worldObjects[uuid].gameObject.transform.SetParent(entity.hand_r.transform, false);
    }

    public bool containObject(uint uuid)
    {
        return worldObjects.ContainsKey(uuid);
    }
}
