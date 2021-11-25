
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private int _health = 36;
    private int _minHeal = 5;
    private CancellationTokenSource _cancellationToken;
    public void Start()
    {
        _cancellationToken = new CancellationTokenSource();
        //UnitAsyncTasks();
        //ReceiveHealing();
        SomeTaskWaiter();
    }

    public void ReceiveHealing()
    {
        StartCoroutine(HealingCoroutine());
    }

    private async void SomeTaskWaiter()
    {
        
        Task task1 = new Task(()=>UnitAsyncTask1(_cancellationToken.Token));
        Task task2 = new Task(()=>UnitAsyncTask2(_cancellationToken.Token));       
        
        Debug.Log("Result: "+ await Task.Run(()=>WhatTaskFasterAsync(_cancellationToken.Token, task1, task2)));
        _cancellationToken.Cancel();
        _cancellationToken.Dispose();
    }
    
    private async void UnitAsyncTasks()
    {
        Task task1 = Task.Run(()=>UnitAsyncTask1(_cancellationToken.Token));
        Task task2 = Task.Run(()=>UnitAsyncTask2(_cancellationToken.Token));
        await Task.WhenAll(task1, task2);
    }
    
    private  async void UnitAsyncTask1(CancellationToken cancellationToken)
    {
        Debug.Log("Task 1 starts");
        for (int i = 0; i < 100; i++)
        {
            await Task.Delay(10);
            if (cancellationToken.IsCancellationRequested)
            {
                Debug.Log("token 1");
                return;
            }
        }
        Debug.Log("Task 1 ends");
    }
    
    private async void UnitAsyncTask2(CancellationToken cancellationToken )
    {
        Debug.Log("Task 2 starts");
        int count = 0;
        while (count<132323240)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Debug.Log("token 2");
                return;
            }
            await Task.Yield();
            count++;
        }
        Debug.Log("Task 2 ends");
      
    }

    public async Task<bool> WhatTaskFasterAsync(CancellationToken ctx,Task task1,Task task2)
    {
        task1.Start();
        task2.Start();
        // по логике все верно, но почему-то whenAny всегда возвращает task2
        // причем без разницы завершен он или нет.
        // 
        Task result = await Task.WhenAny(task1, task2);
        await result;
        if (result == task1 && !ctx.IsCancellationRequested)
            return true;
        else
            return false;
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
