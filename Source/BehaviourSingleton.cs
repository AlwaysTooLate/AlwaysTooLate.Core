﻿// AlwaysTooLate.Core (c) 2018-2022 Always Too Late. All rights reserved.

using UnityEngine;

namespace AlwaysTooLate.Core
{
    /// <inheritdoc />
    /// <summary>
    ///     Singleton class for MonoBehavior instances.
    /// </summary>
    /// <typeparam name="TSingleton">The type of implemented MonoBehavior.</typeparam>
    [DefaultExecutionOrder(-100)]
    public class BehaviourSingleton<TSingleton> : MonoBehaviour where TSingleton : BehaviourSingleton<TSingleton>
    {
        /// <summary>
        ///     The instance of this singleton.
        /// </summary>
        /// <remarks>
        ///     Singletons are initialized on Awake call with custom ExecutionOrder of -100.
        ///     Instance can be null, when you are trying to access uninitialized singleton.
        /// </remarks>
        public static TSingleton Instance { get; private set; }

        protected void Awake()
        {
            if (Instance)
            {
                Debug.LogError($"Found another singleton instance! This is not expected. Type name '{typeof(TSingleton).Name}'");
            }

            if (Application.isPlaying)
            {
                DontDestroyOnLoad(transform.root.gameObject);
            }
            
            Instance = (TSingleton) this;
            OnAwake();
        }

        protected virtual void OnAwake() { }
    }
}