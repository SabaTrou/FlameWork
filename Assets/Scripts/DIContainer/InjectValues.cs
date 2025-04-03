using System;
using System.Collections.Generic;
using System.Reflection;


namespace SabaSimpleDIContainer
{
    /// <summary>
    /// InjectParamsk
    /// </summary>
    public class InjectInfo
    {
        public readonly Type type;
       
        public readonly IReadOnlyList<InjectMethodInfo> methodInfos;
        public readonly IReadOnlyList<FieldInfo> fieldInfos;
        public readonly IReadOnlyList<PropertyInfo> propertyInfos;
        public readonly LifeTime lifeTime;
        public InjectInfo(
             Type type,
            IReadOnlyList<InjectMethodInfo> injectMethods,
            IReadOnlyList<FieldInfo> injectFields,
            IReadOnlyList<PropertyInfo> injectProperties
            
            )
        {
            this.type = type;            
            this.methodInfos = injectMethods;
            this.fieldInfos = injectFields;
            this.propertyInfos = injectProperties;
           
        }

    }
    /// <summary>
    /// constructor
    /// </summary>
    public class InjectConstructorInfo
    {
        public readonly ConstructorInfo constructor;
        public readonly ParameterInfo[] parameters;
        public InjectConstructorInfo(ConstructorInfo info)
        {
            constructor = info;
            parameters = info.GetParameters();
        }

    }
    /// <summary>
    /// method
    /// </summary>
    public class InjectMethodInfo
    {
        public readonly MethodInfo info;
        public ParameterInfo[] parameters;
        public InjectMethodInfo(MethodInfo info)
        {
            this.info = info;
            parameters = info.GetParameters();
        }

    }

}