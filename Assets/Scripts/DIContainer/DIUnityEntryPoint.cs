using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SabaSimpleDIContainer.Unity;
using SabaSimpleDIContainer;

namespace SabaSimpleDIContainer.Unity
{
    public class DIUnityEntryPoint : MonoBehaviour
    {
        [Injection]
        private IAwakable[] awakables=new IAwakable[] { };
        [Injection]
        private IStartable[] startables=new IStartable[] { };
        [Injection]
        private ITickable[] tickables=new ITickable[] { };
        [Injection]
        private IFixedTickable[] fixedTicks=new IFixedTickable[] { };

        private void Awake()
        {
           
            foreach(IAwakable awakable in awakables)
            {
                awakable.Awake();
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            
            foreach (IStartable startable in startables)
            {
                startable.Start();
            }
        }

        // Update is called once per frame
        void Update()
        {
            
            foreach (ITickable tickable in tickables)
            {
                tickable.Tick();
            }
        }
        private void FixedUpdate()
        {
            
            foreach(IFixedTickable tickable in fixedTicks)
            {
                tickable.FixedTick();
            }
        }
    }
}
