using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SabaSimpleDIContainer;

namespace SabaSimpleDIContainer.property
{
    public static class DiExtend
    {
        public static void RegisterProperty<T>(this IContainer container, T instance) where T : ReactiveProperty<T>
        {
            container.Register<T>(instance,LifeTime.singleton);
        }
    }
}
