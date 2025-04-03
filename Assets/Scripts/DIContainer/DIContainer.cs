using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;





namespace SabaSimpleDIContainer
{
    public interface IContainer
    {
        public void StartContainer();
        public void Register<T>(LifeTime lifeTime) where T : class;
        public void Register<T>(object instance, LifeTime lifeTime) where T : class;
        public void Register<Key, Value>() where Key : class where Value : Key;

        public void DisposeAll();
    }
    public interface IResolver
    {
        public void Resolve<T>(object instance) where T : class;
    }

    public class DIContainer : IContainer, IResolver
    {
        private Dictionary<Type, List<object>> _resolvedInstances = new(); // 解決済み
        private Dictionary<Type, List<object>> _unresolvedInstances = new(); // 未解決       
        private Dictionary<Type, InjectInfo> _infos = new();
        private Dictionary<Type, LifeTime> _lifeTimes = new();
        private HashSet<Type> _resolvingTypes = new();

        private Dictionary<Type, MultipleInfo> _unresolvedMultipleInfos = new();
        private class MultipleInfo
        {
            readonly public Type type;
            readonly public object instance;
            public IReadOnlyList<InjectMethodInfo> methodInfos;
            public IReadOnlyList<FieldInfo> fieldInfos;
            public IReadOnlyList<PropertyInfo> propertyInfos;

            public MultipleInfo(Type type, object instance)
            {
                this.type = type;
                methodInfos = new List<InjectMethodInfo>();
                fieldInfos = new List<FieldInfo>();
                propertyInfos = new List<PropertyInfo>();
                this.instance = instance;
            }
            public void SetMethodInfos(IReadOnlyList<InjectMethodInfo> methodInfos)
            {
                this.methodInfos = methodInfos;
            }
            public void SetFieldInfos(IReadOnlyList<FieldInfo> fieldInfos)
            {
                this.fieldInfos = fieldInfos;
            }
            public void SetPropertyInfos(IReadOnlyList<PropertyInfo> propertyInfos)
            {
                this.propertyInfos = propertyInfos;
            }
        }



        /// <summary>
        /// 初期化
        /// </summary>
        public DIContainer()
        {
            Register<IResolver>(this, LifeTime.singleton);

        }
        /// <summary>
        /// フレーム指定
        /// </summary>
        /// <param name="frameLate"></param>
        public DIContainer(int frameLate)
        {
            Register<IResolver>(this, LifeTime.singleton);

        }
        #region IContainer
        public void StartContainer()
        {
           
            InjectSingletonAll();

            InjectMultipleAll();
            //try
            //{
            //    InjectSingletonAll();

            //    InjectMultipleAll();

               
            //}
            //catch (Exception ex)
            //{

            //    Debug.LogError($"DIContainer Error: {ex.Message}");
            //}
            //finally
            //{

            //}
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InjectSingletonAll()
        {
           
            foreach (Type type in _unresolvedInstances.Keys.ToList()) // 未解決の型を順に解決
            {
               
                if(_resolvedInstances.ContainsKey(type))
                {
                    continue;
                }
                var unresolvedList = _unresolvedInstances[type].ToList(); // コピーを作成
                foreach (object instance in unresolvedList)
                {
                    
                    InjectSearchTree(type);
                }

            }
        }
        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        private void InjectMultipleAll()
        {
            foreach (MultipleInfo multipleInfos in _unresolvedMultipleInfos.Values)
            {
                #region method
                foreach (InjectMethodInfo methodInfo in multipleInfos.methodInfos)
                {
                    object[] parameters = new object[methodInfo.parameters.Length];
                    int i = -1;
                    foreach (ParameterInfo parameterInfo in methodInfo.parameters)
                    {
                        i++;

                        // 要素型を取得
                        Type elementType = GetElementType(parameterInfo.ParameterType);
                        if (!_resolvedInstances.TryGetValue(elementType, out List<object> value))
                        {
                            Debug.Log(GetElementType(parameterInfo.ParameterType));
                            continue;
                        }
                        if (!IsEnumerable(parameterInfo.ParameterType))
                        {
                            parameters[i] = value[0];
                            continue;
                        }
                        if (parameterInfo.ParameterType.IsArray)
                        {
                            Array array = Array.CreateInstance(elementType, value.Count);
                            for (int j = 0; j < value.Count; j++)
                            {
                                array.SetValue(value[j], j);
                            }
                            parameters[i] = array;
                            continue;
                        }
                        parameters[i] = value.ToList();
                    }
                    Debug.Log(parameters[0]);
                    methodInfo.info.Invoke(multipleInfos.instance, parameters);
                }
                #endregion
                #region field
                foreach (FieldInfo fieldInfo in multipleInfos.fieldInfos)
                {
                    Type elementType = GetElementType(fieldInfo.FieldType);
                    if (!_resolvedInstances.TryGetValue(elementType, out List<object> value))
                    {
                        continue;
                    }

                    if (fieldInfo.FieldType.IsArray)
                    {
                        Array array = Array.CreateInstance(elementType, value.Count);
                        for (int j = 0; j < value.Count; j++)
                        {
                            array.SetValue(value[j], j);
                        }
                        fieldInfo.SetValue(multipleInfos.instance, array);
                    }
                    else if (IsEnumerable(fieldInfo.FieldType))
                    {
                        fieldInfo.SetValue(multipleInfos.instance, value.ToList());
                    }
                    else//要らないけど念のため
                    {
                        fieldInfo.SetValue(multipleInfos.instance, value[0]);
                    }
                }
                #endregion
                #region property
                foreach (PropertyInfo propertyInfo in multipleInfos.propertyInfos)
                {
                    Type elementType = GetElementType(propertyInfo.PropertyType);

                    if (!_resolvedInstances.TryGetValue(elementType, out List<object> value))
                    {
                        Debug.Log($"未解決のプロパティ: {propertyInfo.PropertyType}");
                        continue;
                    }

                    if (propertyInfo.PropertyType.IsArray)
                    {
                        Array array = Array.CreateInstance(elementType, value.Count);
                        for (int j = 0; j < value.Count; j++)
                        {
                            array.SetValue(value[j], j);
                        }
                        propertyInfo.SetValue(multipleInfos.instance, array);
                    }
                    else if (IsEnumerable(propertyInfo.PropertyType))
                    {
                        propertyInfo.SetValue(multipleInfos.instance, value.ToList());
                    }
                    else
                    {
                        propertyInfo.SetValue(multipleInfos.instance, value[0]);
                    }
                }
                #endregion
            }
        }

        #region Register
        public void Register<T>(LifeTime lifeTime) where T : class
        {
            Type type = typeof(T);

            if (_unresolvedInstances.ContainsKey(type) || _resolvedInstances.ContainsKey(type)) return;

            _infos[type] = GetInjectValue(type);
            _lifeTimes[type] = lifeTime;

            // 未解決として登録
            RegisterUnresolvedInstance(type, Activator.CreateInstance(type));
        }

        public void Register<T>(object instance, LifeTime lifeTime) where T : class
        {
            Type type = typeof(T);

            if (instance is not T)
            {
                throw new ArgumentException($"Instance type {instance.GetType()} does not match the expected type {type}");
            }

            _infos[type] = GetInjectValue(type);
            _lifeTimes[type] = lifeTime;

            // 未解決として登録
            RegisterUnresolvedInstance(type, instance);
        }

        public void Register<Key, Value>() where Key : class where Value : Key
        {
            _infos[typeof(Key)] = GetInjectValue(typeof(Value));
            _lifeTimes[typeof(Key)] = LifeTime.singleton;
            Value instance = (Value)Activator.CreateInstance(typeof(Value));
            RegisterUnresolvedInstance(typeof(Key), instance);
        }

        private void RegisterUnresolvedInstance(Type type, object instance)
        {
            if (!_unresolvedInstances.ContainsKey(type))
            {
                _unresolvedInstances[type] = new List<object>();
            }

            // 未解決インスタンスとして登録
            _unresolvedInstances[type].Add(instance);
        }

        private void RegisterResolvedInstance(Type type, object instance)
        {
            if (!_resolvedInstances.ContainsKey(type))
            {
                _resolvedInstances[type] = new List<object>();
            }

            // 解決済みインスタンスとして登録
            _resolvedInstances[type].Add(instance);

            // 未解決リストから削除
            if (_unresolvedInstances.ContainsKey(type))
            {
                _unresolvedInstances[type].Remove(instance);
                if (_unresolvedInstances[type].Count == 0)
                {
                    _unresolvedInstances.Remove(type);
                }
            }
        }

       
        
        #endregion register
        #region singleton
        private object InjectSearchTree(Type type)
        {

            
            if (_lifeTimes[type] == LifeTime.singleton&& _resolvedInstances.TryGetValue(type, out List<object> values))
            {
                _unresolvedMultipleInfos[type] = new(type, values[0]);
                return values[0];
            }


            if (_resolvingTypes.Contains(type))
            {
                throw new InvalidOperationException($"循環依存が発生しました: {type}");
            }

            _resolvingTypes.Add(type);

            try
            {
                if (!_infos.ContainsKey(type))
                {
                    return null;
                }

                object instance;
                List<object> instances;
                if (_lifeTimes[type] == LifeTime.singleton && _unresolvedInstances.TryGetValue(type, out instances))
                {
                    instance = instances[0];
                }
                else if (_lifeTimes[type] == LifeTime.Multiple)
                {
                    if(_unresolvedInstances.TryGetValue(type, out instances))
                    {
                        instance = instances[0];
                    }
                    else if(_resolvedInstances.TryGetValue(type, out instances))
                    {
                        instance = instances[0];
                        
                    }
                    else
                    {
                        instance = Activator.CreateInstance(_infos[type].type);
                        RegisterUnresolvedInstance(type, instance);
                    }
                   
                }
                else
                {
                    instance = Activator.CreateInstance(_infos[type].type);
                    RegisterUnresolvedInstance(type, instance);
                }
                

                InjectTrees(type, instance);
                // 解決済みとして登録
                if (_lifeTimes[type] == LifeTime.singleton)
                {
                    //RegisterResolvedInstance(type, instance);
                }
                else
                {
                    RegisterResolvedInstance(type, instance);
                }

                return instance;
            }
            finally
            {
                _resolvingTypes.Remove(type);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InjectTrees(Type type, object instance)
        {
            
            if(!_unresolvedMultipleInfos.ContainsKey(type))
            {
                _unresolvedMultipleInfos[type] = new(type, instance);
            }
            InjectFieldTree(type, instance);
            InjectMethodTree(type, instance);
            InjectPropertyTree(type, instance);
        }
        private object InjectFieldTree(Type type, object instance)
        {

            if (!_infos.ContainsKey(type))
            {
                return null;
            }
            List<FieldInfo> iEnumerableInfos = new();
            foreach (FieldInfo info in _infos[type].fieldInfos)
            {
                if (!IsDefinedInjection(info))
                {
                    continue;
                }
                if (IsEnumerable(info.FieldType))
                {
                    iEnumerableInfos.Add(info);
                    continue;
                }
                info.SetValue(instance, InjectSearchTree(info.FieldType));
            }
            _unresolvedMultipleInfos[type].SetFieldInfos(iEnumerableInfos);
            return instance;
        }

        private object InjectMethodTree(Type type, object instance)
        {
            if (!_infos.ContainsKey(type)) return null;
            List<InjectMethodInfo> iEnumerableInfos = new List<InjectMethodInfo>();
            foreach (InjectMethodInfo info in _infos[type].methodInfos)
            {
                if (!IsDefinedInjection(info.info))
                {
                    continue;
                }
                object[] parameters = new object[info.parameters.Length];
                int index = 0;
                bool isBreaked = false;
                foreach (ParameterInfo param in info.parameters)
                {
                    if (IsEnumerable(param.ParameterType))
                    {

                        isBreaked = true;
                        continue;
                    }
                    
                    parameters[index] = InjectSearchTree(param.ParameterType);

                    index++;
                }
                if (isBreaked)
                {
                    iEnumerableInfos.Add(info);
                    continue;
                }
                info.info.Invoke(instance, parameters);
            }
            _unresolvedMultipleInfos[type].SetMethodInfos(iEnumerableInfos);
            return instance;
        }

        private object InjectPropertyTree(Type type, object instance)
        {
            if (!_infos.ContainsKey(type)) return null;

            List<PropertyInfo> iEnumerableInfos = new List<PropertyInfo>();
            foreach (PropertyInfo info in _infos[type].propertyInfos)
            {
                if (!IsDefinedInjection(info)) continue;
                if (!IsEnumerable(info.PropertyType))
                {
                    iEnumerableInfos.Add(info);
                    continue;
                }
                object injectInstance = InjectSearchTree(info.PropertyType);
                info.SetValue(instance, injectInstance);
            }
            _unresolvedMultipleInfos[type].SetPropertyInfos(iEnumerableInfos);
            return instance;
        }
        #endregion singleton
        
        private InjectInfo GetInjectValue(Type type)
        {
            return new InjectInfo(
                type,
                GetInjectMethodInfos(type),
                GetFieldInfos(type),
                GetPropertyInfos(type));
        }

        private List<InjectMethodInfo> GetInjectMethodInfos(Type type)
        {
            MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            return methods.Select(method => new InjectMethodInfo(method)).ToList();
        }

        private List<FieldInfo> GetFieldInfos(Type type)
        {
            return type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList();
        }

        private List<PropertyInfo> GetPropertyInfos(Type type)
        {
            return type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).ToList();
        }

        private bool IsDefinedInjection(MemberInfo info)
        {
            return info.IsDefined(typeof(Injection), false);
        }
        /// <summary>
        /// IEnumerableか判別
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool IsEnumerable(Type type)
        {
            // 配列型は IEnumerable として扱う
            if (type.IsArray)
            {
                return true;
            }

            // 非ジェネリック型の IEnumerable
            if (type == typeof(IEnumerable))
            {
                return true;
            }

            // ジェネリック型の IEnumerable<T>
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return true;
            }

            // インターフェースに IEnumerable<T> が含まれているか
            return type.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        /// <summary>
        /// IEnumerableの属性を取得
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Type GetElementType(Type type)
        {
            // 配列型の場合
            if (type.IsArray)
            {
                return type.GetElementType(); // 配列の要素型を取得
            }

            // IEnumerable<T> 型の場合
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return type.GetGenericArguments()[0]; // ジェネリック引数を取得
            }

            // IEnumerable<T> を実装している場合（直接ではなくインターフェース経由）
            var ienumerableType = type.GetInterfaces()
                                      .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (ienumerableType != null)
            {
                return ienumerableType.GetGenericArguments()[0]; // ジェネリック引数を取得
            }

            // 非ジェネリック型の IEnumerable または対象外の型
            return null;
        }
        public void DisposeAll()
        {
            _resolvedInstances.Clear();
            _unresolvedInstances.Clear();
            _infos.Clear();

        }
        #endregion
        #region IResolver
        void IResolver.Resolve<T>(object instance) where T : class
        {
            Register<T>(instance, LifeTime.Transient);
            InjectTrees(typeof(T), instance);
        }
        #endregion




    }


    public enum LifeTime
    {
        singleton,
        Transient,
        Multiple
    }

    /// <summary>
    /// 注入可能属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class Injection : Attribute
    {

    }


}

