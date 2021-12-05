using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = UnityEngine.Random;

public class SecondHomework : MonoBehaviour
{
    private NativeArray<int> _array;
    private NativeArray<Vector3> _positions;
    private NativeArray<Vector3> _velocities;
    private NativeArray<Vector3> _finalPositions;
    private JobHandle _handle;
   

    private void Start()
    {
        #region first_task

        // _array = new NativeArray<int>(10, Allocator.TempJob);
        // for (int i = 0; i < _array.Length; i++)
        // {
        //     _array[i] = Random.Range(0, 15);
        // }
        // for (int i = 0; i < _array.Length; i++)
        // {
        //     Debug.Log(_array[i]);
        // }
        // JobStruct jobStruct = new JobStruct();
        // jobStruct.Array = _array;
        // jobStruct.Execute();
        // for (int i = 0; i < _array.Length; i++)
        // {
        //     Debug.Log(_array[i]);
        // }

        #endregion

        _positions = new NativeArray<Vector3>(100, Allocator.TempJob);
        for (int i = 0; i < _positions.Length; i++)
        {
            _positions[i] = new Vector3(Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10));
        }

        _velocities = new NativeArray<Vector3>(100, Allocator.TempJob);
        for (int i = 0; i < _velocities.Length; i++)
        {
            _velocities[i] = new Vector3(Random.Range(0, 40), Random.Range(0, 40), Random.Range(0, 40));
        }
        _finalPositions= new NativeArray<Vector3>(100,Allocator.TempJob);
        JobStruct job = new JobStruct();
        job.Positions = _positions;
        job.Velocities = _velocities;
        job.FinalPositions = _finalPositions;

        _handle = job.Schedule(100, 10);
        _handle.Complete();
        StartCoroutine(JobCoroutine());

    }

    IEnumerator JobCoroutine()
    {
        while (_handle.IsCompleted == false)
        {
            yield return new WaitForEndOfFrame();
        }

        foreach (Vector3 vector in _finalPositions)
        {
            print(vector);
        }
        
        
        
    }

    // public struct JobStruct :  IJob
    // {
    //     public NativeArray<int> Array;
    //
    //     public void Execute()
    //     {
    //         for (int i = 0; i < Array.Length; i++)
    //         {
    //             if (Array[i] > 10)
    //             {
    //                 Array[i] = 0;
    //             }
    //         }
    //     }
    // }
    public struct JobStruct : IJobParallelFor
    {
        public NativeArray<Vector3> Positions;
        public NativeArray<Vector3> Velocities;
        public NativeArray<Vector3> FinalPositions;

        
        public void Execute(int index)
        {
            FinalPositions[index] = Positions[index] + Velocities[index];
        }

       

        
    }
    private void OnDestroy()
    {
        _positions.Dispose();
        _velocities.Dispose();
        _finalPositions.Dispose();
    }
}