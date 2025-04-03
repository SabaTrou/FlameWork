using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SabaSimpleDIContainer;
using System;

namespace SabaSimpleDIContainer.Unity
{

    /// <summary>
    /// gameObjectÇ…Ç¬ÇØÇƒDIContainerÇÃinstanceÇï€éùÇµÇƒÇ¢ÇÈ
    /// </summary>
    public class LifeTimeScope : MonoBehaviour
    {
        private IContainer _container;
        private delegate void StartUpDel();
        private StartUpDel _startUpDel;
        private DIUnityEntryPoint _entryPoint;
        //private Tickable _frameManager;
        //private Startable _startableManager;
        private void Awake()
        {
            _container = new DIContainer();
            Debug.Log(this.gameObject);
            _entryPoint = this.gameObject.AddComponent<DIUnityEntryPoint>();
            _container.Register<DIUnityEntryPoint>(_entryPoint,LifeTime.singleton);
            _startUpDel += CallConfigure;
            _startUpDel += CallInjectAll;
            //int frame = Mathf.FloorToInt(1 / Time.fixedDeltaTime);           
            _startUpDel.Invoke();
            
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        private void CallConfigure()
        {
            Configure(_container);
        }
        private void CallInjectAll()
        {
            _container.StartContainer();
        }
        protected virtual void Configure(IContainer container)
        {

        }
        private void OnDestroy()
        {
            DisposeAll();

        }
        private void DisposeAll()
        {
            _container.DisposeAll();
            _container = null;
        }

       
    }



}
