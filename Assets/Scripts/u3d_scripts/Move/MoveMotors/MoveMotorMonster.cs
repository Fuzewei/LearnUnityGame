using System.Collections.Generic;
using UnityEngine;
using KBEngine.Const;
using KBEngine;
using SwordMaster;
using System;

class MoveMotorMonster : MoveMotorAvatarP3
{
    public int pathIndex = 0;
    public List<Vector3> path = new List<Vector3>(); //怪物移动需要路点

    public Vector3 movepoint = new Vector3(); //怪物移动点和路点不同是只有一个点

    public Transform moveTarget = null; //怪物移动目标对象

    public AiMoveConst aiMovingType = 0;   //服务端移动类型 

#if UNITY_EDITOR
    GameObject sphere;

    private void Start()
    {
        sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }
#endif

    public override bool setAiMovePath(PATH_POINTS _path, uint _pathIndex)
    {
        pathIndex = 0;
        path = _path as List<Vector3>;
        return true;
    }

    public override bool setAiMoveTarget(Transform tr)
    {
        moveTarget = tr;
        return true;
    }

    public override bool setAiMovType(AiMoveConst _aiMoveType)
    {
        aiMovingType = _aiMoveType;
        return true;
    }

    public override bool setAiMovPoint(Vector3 movePostion)
    {
        movepoint = movePostion;
        return true;
    }


    public override void onUpdate()
    {
        base.onUpdate();

    }

    public override void beforeMoveUpdate()
    {
        if (aiMovingType == AiMoveConst.CHAST_RUN)
        {
            Vector3 _direction = moveTarget.position - transform.position;
            _direction.y = 0;
            _direction = _direction.normalized;
            setMoveDirection(_direction);

            float rotateAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
            Vector3 faceDirection = new Vector3(0, rotateAngle, 0);
            setFaceDirection(faceDirection);

        }
        if (aiMovingType == AiMoveConst.FIGHT_MOVE)
        {
#if UNITY_EDITOR
            sphere.transform.position = movepoint;
#endif
            Vector3 enemyDirection = moveTarget.position - transform.position;
            enemyDirection.y = 0;
            enemyDirection = enemyDirection.normalized;

            Vector3 movePointDirection = movepoint - transform.position;
            movePointDirection.y = 0;
            var dis = movePointDirection.magnitude;
            Debug.Log("movePointDirection.magnitude: " + dis);
            if (dis < 0.5f)
            {
                setMoveType(MoveConst.Idel);
                return;
            }
            movePointDirection = movePointDirection.normalized;
            setMoveDirection(movePointDirection);


            float rotateAngle = Mathf.Atan2(enemyDirection.x, enemyDirection.z) * Mathf.Rad2Deg;
            Vector3 faceDirection = new Vector3(0, rotateAngle, 0);
            setFaceDirection(faceDirection);
        }
    }

    public override void afterMoveUpdate()
    {
        if (aiMovingType == AiMoveConst.RANDOM_MOVE)
        {
            Vector3 pos = transform.position;
            Vector3 nextPoint = path[pathIndex];
            Vector3 _direction = nextPoint - pos;
            _direction = _direction.normalized;
            setMoveDirection(_direction);

            float rotateAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
            Vector3 faceDirection = new Vector3(0, rotateAngle, 0);
            setFaceDirection(faceDirection);

        }

        if (aiMovingType == AiMoveConst.CHAST_RUN)
        {
            Vector3 _direction = moveTarget.position - transform.position;
            _direction.y = 0;
            _direction = _direction.normalized;
            setMoveDirection(_direction);
            float rotateAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
            Vector3 faceDirection = new Vector3(0, rotateAngle, 0);
            setFaceDirection(faceDirection);

        }
    }

    protected override void clientUpdatePositionAfterServerUpdate()
    {
        base.clientUpdatePositionAfterServerUpdate();
    }


    public override void confirmMoveTimeStamp(float timeStamp, MoveConst moveType, Vector3 position, Vector3 faceDirection, Vector3 moveDirection, bool inBattle)
    {
        base.confirmMoveTimeStamp(timeStamp, moveType, position, faceDirection, moveDirection, inBattle);
        Debug.Log("monster.confirmMoveTimeStamp" + timeStamp);
    }
}

