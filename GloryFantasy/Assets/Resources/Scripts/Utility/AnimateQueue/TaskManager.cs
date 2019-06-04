/// Simple, really.  There is no need to initialize or even refer to TaskManager.  
/// When the first Task is created in an application, a "TaskManager" GameObject  
/// will automatically be added to the scene root with the TaskManager component  
/// attached.  This component will be responsible for dispatching all coroutines  
/// behind the scenes.  
///  
/// Task also provides an event that is triggered when the coroutine exits.  

using UnityEngine;
using System.Collections;
using System;

namespace GameUtility.Animate
{
    //Task用来封装协程方法，提供暂停和中止，并且可以设置完成委托方法
    /// A Task object represents a coroutine.  Tasks can be started, paused, and stopped.  
    /// It is an error to attempt to start a task that has been stopped or which has  
    /// naturally terminated.  
    public class Task : IPlay
    {
        #region IPlay的显示接口实现
        Action _playNext;
        Action IPlay.PlayNext
        {
            get
            {
                return _playNext;
            }

            set
            {
                _playNext = value;
            }
        }
        void IPlay.Start()
        {
            Start();
        }
        #endregion

        IEnumerator coroutine;
        //标记任务的协程是否在跑
        bool running;
        //任务是否被暂停
        bool paused;
        //任务是否被终止
        bool stopped;

        // Returns true if and only if the coroutine is running.  Paused tasks  
        // are considered to be running.  
        public bool Running
        {
            get
            {
                return running;
            }
        }

        // Returns true if and only if the coroutine is currently paused.  
        public bool Paused
        {
            get
            {
                return paused;
            }
        }

        //Termination event.  Triggered when the coroutine completes execution.  
        //public event FinishedHandler Finished;
        public Action<bool> Finished;

        public Task(IEnumerator c)
        {
            coroutine = c;
        }

        /// Begins execution of the coroutine  
        public void Start()
        {
            Debug.Log("Start time:" + Time.time);
            running = true;
            TaskManager.Instance().StartCoroutine(CallWrapper());
            
        }

        /// Discontinues execution of the coroutine at its next yield.  
        public void Stop()
        {
            stopped = true;
            running = false;
        }

        public void Pause()
        {
            paused = true;
        }

        public void Unpause()
        {
            paused = false;
        }

        public void Reset()
        {
            running = false;
            paused = false;
            stopped = false;
            //协程方法重置

        }

        IEnumerator CallWrapper()
        {
            Debug.Log("Time:" + Time.time + " Begin CallWraper");
            yield return null;
            IEnumerator e = coroutine;
            Debug.Log("Time:" + Time.time + " runing is " + running);
            while (running)
            {
                if (paused)
                    yield return null;
                else
                {
                    if (e != null && e.MoveNext())
                    {
                        yield return e.Current;
                    }
                    else
                    {
                        running = false;
                    }
                }
            }

            if (Finished != null)
            {
                Finished(stopped);
                Reset();
                //IPlay接口需要自己调用,调用的队列保证此不为空
                _playNext();
            }
            Debug.Log("Time:" + Time.time + " End CallWraper");
        }
    }

    class TaskManager : UnitySingleton<TaskManager>
    {
        public Task CreateTask(IEnumerator coroutine, bool autoStart = false)
        {
            Task task = new Task(coroutine);
            if (autoStart)
            {
                task.Start();
            }
            return task;
        }
    }
}