using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utilities
{
    /// <summary>
    /// Triggers an <see cref="Action"/> after a specified time
    /// </summary>
    public class FunctionTimer
    {
        /// <summary>
        /// Local reference to <see cref="MonoBehaviour"/> 
        /// </summary>
        private class MonoBehaviourHook : MonoBehaviour 
        {
            public Action onUpdate;

            private void Update()
            {
                onUpdate?.Invoke();
            }
        }
        
        /// <summary>
        /// Keeps track of all existing <see cref="FunctionTimer"/>s
        /// </summary>
        private static List<FunctionTimer> _timerList;
        
        /// <summary>
        /// Global <see cref="GameObject"/> used for initializing <see cref="FunctionTimer"/>
        /// </summary>
        /// <remarks>Destroyed on scene change</remarks>
        private static GameObject _initGameObject;
        
        /// <summary>
        /// Initializes Global <see cref="GameObject"/>
        /// </summary>
        private static void Init() 
        {
            if (_initGameObject == null) 
            {
                _initGameObject = new GameObject("FunctionTimer_Global");
                
                _timerList = new List<FunctionTimer>();
            }
        }
        
        /// <summary>
        /// Creates a <see cref="FunctionTimer"/>
        /// </summary>
        /// <param name="action">Action invoked after timer elapsed</param>
        /// <param name="timer">Length of time before invoking Action</param>
        /// <returns>A new instance of <see cref="FunctionTimer"/></returns>
        public static FunctionTimer Create(Action action, float timer) 
        {
            return Create(action, timer, "", false, false);
        }
        
        /// <summary>
        /// Creates a <see cref="FunctionTimer"/>
        /// </summary>
        /// <param name="action">Action invoked after timer elapsed</param>
        /// <param name="timer">Length of time before invoking Action</param>
        /// <param name="functionName">Name of function</param>
        /// <returns>A new instance of <see cref="FunctionTimer"/></returns>
        public static FunctionTimer Create(Action action, float timer, string functionName) 
        {
            return Create(action, timer, functionName, false, false);
        }
        
        /// <summary>
        /// Creates a <see cref="FunctionTimer"/>
        /// </summary>
        /// <param name="action">Action invoked after timer elapsed</param>
        /// <param name="timer">Length of time before invoking Action</param>
        /// <param name="functionName">Name of function</param>
        /// <param name="useUnscaledDeltaTime">Determines if unscaled delta time is used</param>
        /// <returns>A new instance of <see cref="FunctionTimer"/></returns>
        public static FunctionTimer Create(Action action, float timer, string functionName, bool useUnscaledDeltaTime) 
        {
            return Create(action, timer, functionName, useUnscaledDeltaTime, false);
        }
        
        /// <summary>
        /// Creates a <see cref="FunctionTimer"/>
        /// </summary>
        /// <param name="action">Action invoked after timer elapsed</param>
        /// <param name="timer">Length of time before invoking Action</param>
        /// <param name="functionName">Name of function</param>
        /// <param name="useUnscaledDeltaTime">If true, unscaled delta time is used</param>
        /// <param name="stopAllWithSameName">If true, timers with same name are stopped</param>
        /// <returns>A new instance of <see cref="FunctionTimer"/></returns>
        public static FunctionTimer Create(Action action, float timer, string functionName, bool useUnscaledDeltaTime, bool stopAllWithSameName) 
        {
            Init();

            if (stopAllWithSameName) 
            {
                StopAllTimersWithName(functionName);
            }

            var obj = new GameObject($"FunctionTimer Object {functionName}", typeof(MonoBehaviourHook));
            
            var funcTimer = new FunctionTimer(obj, action, timer, functionName, useUnscaledDeltaTime);
            
            obj.GetComponent<MonoBehaviourHook>().onUpdate = funcTimer.Update;

            _timerList.Add(funcTimer);

            return funcTimer;
        }
        
        /// <summary>
        /// Removes specified <see cref="FunctionTimer"/>
        /// </summary>
        /// <param name="funcTimer">Timer to remove</param>
        public static void RemoveTimer(FunctionTimer funcTimer) 
        {
            Init();
            
            _timerList.Remove(funcTimer);
        }
        
        /// <summary>
        /// Stops all <see cref="FunctionTimer"/>s with specified name.
        /// </summary>
        /// <param name="functionName">Name of timers to stop</param>
        public static void StopAllTimersWithName(string functionName) 
        {
            Init();
            
            for (var i = 0; i < _timerList.Count; i++) 
            {
                if (_timerList[i]._functionName == functionName) 
                {
                    _timerList[i].DestroySelf();
                    
                    i--;
                }
            }
        }
        
        /// <summary>
        /// Stops first <see cref="FunctionTimer"/> found with specified name
        /// </summary>
        /// <param name="functionName">Name of timers to stop</param>
        public static void StopFirstTimerWithName(string functionName) 
        {
            Init();
            
            for (var i = 0; i < _timerList.Count; i++) 
            {
                if (_timerList[i]._functionName == functionName) 
                {
                    _timerList[i].DestroySelf();
                    
                    return;
                }
            }
        }
        
        /// <summary>
        /// <see cref="GameObject"/> instance
        /// </summary>
        private readonly GameObject _gameObject;
        
        /// <summary>
        /// Length of time remaining
        /// </summary>
        private float _timer;
        
        /// <summary>
        /// Name of function
        /// </summary>
        private readonly string _functionName;
        
        /// <summary>
        /// True if active
        /// </summary>
        private bool _active;
        
        /// <summary>
        /// True if using unscaled delta time
        /// </summary>
        private readonly bool _useUnscaledDeltaTime;
        
        /// <summary>
        /// Action to be called when timer runs out
        /// </summary>
        private readonly Action _action;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gameObject">GameObject instance</param>
        /// <param name="action">Action to invoke when timer is finished</param>
        /// <param name="timer">Amount of time left on timer</param>
        /// <param name="functionName">Name of function</param>
        /// <param name="useUnscaledDeltaTime">If true, use unscaled delta time</param>
        public FunctionTimer(GameObject gameObject, Action action, float timer, string functionName, bool useUnscaledDeltaTime) 
        {
            _gameObject = gameObject;
            
            _action = action;
            
            _timer = timer;
            
            _functionName = functionName;
            
            _useUnscaledDeltaTime = useUnscaledDeltaTime;
        }
        
        private void Update() 
        {
            if (_useUnscaledDeltaTime) 
            {
                _timer -= UnityEngine.Time.unscaledDeltaTime;
            } 
            else 
            {
                _timer -= UnityEngine.Time.deltaTime;
            }
            if (_timer <= 0) 
            {
                _action();
                
                DestroySelf();
            }
        }
        
        /// <summary>
        ///  Destroys timer
        /// </summary>
        private void DestroySelf() 
        {
            RemoveTimer(this);
            
            if (_gameObject != null) 
            {
                Object.Destroy(_gameObject);
            }
        }
        
        /// <summary>
        /// Class to trigger Actions manually without creating a GameObject
        /// </summary>
        public class FunctionTimerObject 
        {
            private float _timer;
            
            private Action _callback;

            public FunctionTimerObject(Action callback, float timer) 
            {
                _callback = callback;
                
                _timer = timer;
            }

            public void Update() 
            {
                Update(UnityEngine.Time.deltaTime);
            }
            
            public void Update(float deltaTime) 
            {
                _timer -= deltaTime;
                
                if (_timer <= 0) 
                {
                    _callback();
                }
            }
        }
    }
}
