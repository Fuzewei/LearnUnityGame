using System;
using System.Collections.Generic;
using KBEngine.Const;
using UnityEngine;
using KBEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class SampleBase
{
    public MoveConst _moveType;
    public Vector3 faceDirection;
    public Vector3 position;
    public Vector3 moveDirection;
    public bool inBattle;
    public MoveConst moveType {

        set { _moveType = value; }
        get
        {
            return _moveType;
        }
    }

    public SampleBase(MoveConst moveType,  Vector3 position, Vector3 faceDirection, Vector3 moveDirection, bool inBattle)
    {
        this.moveType = moveType;
        this.faceDirection = faceDirection;
        this.position = position;
        this.moveDirection = moveDirection;
        this.inBattle = inBattle;
    }
}

public class SampleQueue
{
    private List<SampleBase> opQueue; //操作队列
    private List<float> positionQueue; //递增数组

    public SampleQueue()
    {
        opQueue = new List<SampleBase>();
        positionQueue = new List<float>();
    }

    public float recentPosition()
    {
        return positionQueue[positionQueue.Count - 1];
    }
    public SampleBase getSampleByPosition(float position)
    {
        Tuple<Int32, Int32> leftAndRight = getLeftAndRight(position);
        Int32 left = leftAndRight.Item1;
        Int32 right = leftAndRight.Item2;
        //Debug.Log(left);

        SampleBase leftSample = opQueue[left];
        SampleBase rightSample = right > 0 ? opQueue[right] : null;
        float percentage = right > left ? (position - positionQueue[left]) / (positionQueue[right] - positionQueue[left]) : 0;

        if (right <= 0)
        {
            //Debug.Log("getSampleByPosition:");
           
            return leftSample;
        }
        return leftSample;
    }

    public Tuple<Int32, Int32> getLeftAndRight(float position)
    {
        Int32 left = 0 ;
        Int32 right = -1;
        for (int i = 0; i < positionQueue.Count; i++)
        {
            if (positionQueue[i] <= position)
            {
                left = i;
                continue;
            }
                

            if (positionQueue[i] > position) {
                right = i;
                break;
            }   
        }
        return Tuple.Create<Int32, Int32>(left, right);
    }


    public Int32 getRightOpCount(float position)
    {
        Int32 ans = positionQueue.FindIndex((n)=> n > position);
        if (ans < 0)
        {
            return 0;
        }
        return positionQueue.Count - ans;
    }
   

    public void push(SampleBase sample, float position) {
        opQueue.Insert(opQueue.Count, sample);
        positionQueue.Insert(positionQueue.Count, position);
    }

    public SampleBase end()
    {
        return opQueue[opQueue.Count - 1];
    }

    public void popBeforePosition(float position)
    {
        Tuple<Int32, Int32> leftAndRight = getLeftAndRight(position);
        int popCount = leftAndRight.Item1;
        while (popCount -- > 0) {
            pop();
        }
    }

    public void pop()
    {
        opQueue.RemoveAt(0);
        positionQueue.RemoveAt(0);
    }
    public Int32 lenth()
    {
        return opQueue.Count;
    }

}

