using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SabaSimpleDIContainer;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace SabaSimpleDIContainer.Unity
{
    public static class DiExtend
    {
        public static void RegisterComponent<T>(this IContainer container, T component) where T : MonoBehaviour
        {
            container.Register<T>(component, LifeTime.Multiple);
        }

        public static void RegisterEntryPoint<T>(this IContainer container) where T : class
        {
            Type type = typeof(T);
            object instance = Activator.CreateInstance(type);
            container.Register<T>(instance, LifeTime.singleton);

            if(instance is IAwakable awakable)
            {
                container.Register<IAwakable>(instance, LifeTime.Multiple);
            }
            if (instance is IStartable startable)
            {
                container.Register<IStartable>(instance,LifeTime.Multiple);
            }
            if (instance is ITickable tickable)
            {
                container.Register<ITickable>(instance, LifeTime.Multiple);
            }
            if(instance is IFixedTickable fixedTickable)
            {
                container.Register<IFixedTickable>(instance, LifeTime.Multiple);
            }


        }
    }
   
    
    
}
