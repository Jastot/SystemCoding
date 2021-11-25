using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private int _health = 36;
    private int _minHeal = 5;
    private CancellationTokenSource cancellationToken = new CancellationTokenSource();
    public void Start()
    {
        
        //UnitAsyncTasks();
        //ReceiveHealing();
        Task task1 = Task.Run(()=>UnitAsyncTask1(cancellationToken.Token));
        Task task2 = Task.Run(()=>UnitAsyncTask2(cancellationToken.Token));
        Debug.Log(Task.Run(() => WhatTaskFasterAsync(cancellationToken.Token, task1, task2)).Result);
    }

    public void ReceiveHealing()
    {
        StartCoroutine(HealingCoroutine());
    }

    private async void UnitAsyncTasks()
    {
        CancellationTokenSource cancellationToken = new CancellationTokenSource();
        Task task1 = new Task(()=>UnitAsyncTask1(cancellationToken.Token));
        Task task2 = new Task(()=>UnitAsyncTask2(cancellationToken.Token));
        await Task.WhenAll(task1, task2);
    }
    
    private  async Task UnitAsyncTask1(CancellationToken cancellationToken)
    {
        await Task.Delay(1000);
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        Debug.Log("Task 1 ends");
    }
    
    private  async Task UnitAsyncTask2(CancellationToken cancellationToken )
    {
        int count = 0;
        while (count<60)
        {
            await Task.Yield();
            count++;
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
        }
        Debug.Log("Task 2 ends");
      
    }

    public async Task<bool> WhatTaskFasterAsync(CancellationToken ctx,Task task1,Task task2)
    {
        Task result = await Task.WhenAny(task1, task2);
        cancellationToken.Cancel();
        cancellationToken.Dispose();
        if (result==task1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    IEnumerator HealingCoroutine()
    {
        while ((Time.time<3f)||(_health<100))
        {
            yield return new WaitForSecondsRealtime(0.5f);
            _health += 5;
            if (_health>=100)
            {
                _health = 100;
                ShowHealth();
                break;
            }
            else
                ShowHealth();
        }
        yield break;
    }

    public void ShowHealth()
    {
        Debug.Log("Current heath: "+_health);
    }
}
