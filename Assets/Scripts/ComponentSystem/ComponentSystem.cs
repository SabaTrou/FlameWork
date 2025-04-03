using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ComponentSystem;
using UnityEngine;

namespace ComponentSystem
{
    public abstract class ComponentHolder : IDisposable
    {
        // GCHandle の配列（コンポーネントを格納するためのハンドル）
        private readonly ComponentHandler[] _handles;
        private readonly int[] _getCounts;

        // コンポーネントの最大数
        private readonly int _capacity;

        // 現在格納されているコンポーネントの数
        private int _count = 0;



        // コンストラクタ：デフォルトで8個のコンポーネントを保持できる
        public ComponentHolder(int capacity)
        {
            _capacity = capacity;
            _handles = new ComponentHandler[capacity]; // GCHandle 配列を確保（初期はすべて未使用）
            _getCounts = new int[capacity];
        }

        // 指定した型のコンポーネントを取得（見つからなければ null を返す）
        public T? GetComponent<T>() where T : class, IComponent
        {


            Span<ComponentHandler> span = _handles.AsSpan(); // Span<T> に変換（境界チェック最適化）
            
            for (int i = 0; i < _capacity; i++)
            {
                if (span[i].handle.IsAllocated && span[i].type==typeof(T))
                {
                    T component = (T)span[i].handle.Target; // GCHandle からインスタンスを取得
                    SortGCHandle(i);
                    return component; // 型が一致するコンポーネントを見つけたら返す
                }
            }
            return null; // 見つからなければ null
        }
        public bool TryGetComponent<T>(out T? component) where T : class, IComponent
        {
           component= GetComponent<T>();
            if(component!=null)
            {
                return true;
            }
            return false;
        }

        // 既存のインスタンスをコンポーネントとして追加
        public void AddComponent<T>(T component) where T : class, IComponent
        {
            if (component == null) throw new ArgumentNullException(nameof(component)); // null チェック
            if (_count >= _capacity) throw new InvalidOperationException("Component array is full."); // 容量オーバー
            
            Span<ComponentHandler> span = _handles.AsSpan();
           
            for (int i = 0; i < _capacity; i++)
            {
                if (span[i].handle.IsAllocated) // 未使用スロットを探す
                {
                    continue;
                }
                span[i] = new ComponentHandler( GCHandle.Alloc(component, GCHandleType.Normal),typeof(T)); // ハンドルを割り当て
                _count++;
                return;
            }
        }

        // 型指定で新しいコンポーネントを作成して追加
        public void AddComponent<T>() where T : class, IComponent, new()
        {
            if (_count >= _capacity) throw new InvalidOperationException("Component array is full."); // 容量オーバー

            
            Span<ComponentHandler> span = _handles.AsSpan();
           
            for (int i = 0; i < _capacity; i++)
            {
                if (span[i].handle.IsAllocated) // 未使用スロットを探す
                {
                    continue;
                }

                span[i] = new ComponentHandler(GCHandle.Alloc(new T(), GCHandleType.Normal),typeof(T)); // 新しいインスタンスを作成して登録
                _count++;
                return;
            }
        }

        // 指定した型のコンポーネントを削除
        public void RemoveComponent<T>() where T : class, IComponent
        {
            Span<ComponentHandler> span = _handles.AsSpan();
            for (int i = 0; i < _capacity; i++)
            {
                if (span[i].handle.IsAllocated && span[i].type==typeof(T)) // 指定した型のコンポーネントが見つかったら
                {
                    span[i].handle.Free(); // GCHandle を解放
                    _count--;
                    return;
                }
            }
        }

        private void SortGCHandle(int handleIndex)
        {
            _getCounts[handleIndex]++;
            for (int i = handleIndex; i > 0; i--)
            {
                if (_getCounts[i] <= _getCounts[i - 1])
                {
                    break;
                }
                ComponentHandler temp = _handles[i];
                _handles[i] = _handles[i - 1];
                _handles[i - 1] = temp;
            }
        }
       
        // すべてのコンポーネントを解放（Dispose パターンの実装）
        public void Dispose()
        {


            Span<ComponentHandler> span = _handles;
            for (int i = 0; i < _capacity; i++)
            {
                if (span[i].handle.IsAllocated)
                {
                    span[i].handle.Free();
                    span[i] = default;
                }
            }

            //GC.SuppressFinalize(this); // GCの負担軽減　今回は不要
        }
    }
    public struct ComponentHandler
    {
        public readonly GCHandle handle;
        public readonly Type type;
        public ComponentHandler(GCHandle handle, Type type)
        {
            this.handle = handle;
            this.type = type;
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public interface IComponent
    {
    }
}





