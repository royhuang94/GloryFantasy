﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameUtility.Animate;

public class test : MonoBehaviour {


    // Use this for initialization
    void Start () {
        Task task1 = TaskManager.Instance().CreateTask(DoSomthing());
        task1.Finished += (bool isStop) => { Debug.Log("Task1 Finish.isStop:" + isStop +" ,Time:" + Time.time); };
        AnimateQueue.Instance().AddPlay(task1);

        Task task2 = TaskManager.Instance().CreateTask(DoSomthing());
        task2.Finished += (bool isStop) => { Debug.Log("Task2 Finish.isStop:" + isStop + " ,Time:" + Time.time); };
        AnimateQueue.Instance().AddPlay(task2);

        //错误用法，同一个task不能被使用多次
        AnimateQueue.Instance().AddPlay(task1);
    }

    IEnumerator DoSomthing()
    {
        Debug.Log("Start time:" + Time.time);
        yield return new WaitForSeconds(2f);
        Debug.Log("End time:" + Time.time);
    }
}