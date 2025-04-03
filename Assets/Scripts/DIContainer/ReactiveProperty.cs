using System;
using System.Collections.Generic;

public interface IReadOnlyReactiveProperty<T>
{
    public T Value { get; }
    public IDisposable Subscribe(Action<T> onValueChanged);

}

public class ReactiveProperty<T>:IReadOnlyReactiveProperty<T>
{
    #region　変数
    private T _value;
    private List<Action<T>> _subscribers = new List<Action<T>>();
    #endregion
    public ReactiveProperty(T initialValue)
    {
        _value = initialValue;
    }
    // 値のプロパティ
    public T Value
    {
        get => _value;
        set
        {
            if (!EqualityComparer<T>.Default.Equals(_value, value))
            {
                
                _value = value;
                NotifySubscribers(); // 値の変更時に通知
            }
        }
    }
     // 購読メソッド
    public IDisposable Subscribe(Action<T> onValueChanged)
    {
        _subscribers.Add(onValueChanged);
        // 初期値を即時通知
        onValueChanged(_value);

        // 購読解除用のDisposableを返す
        return new Unsubscriber(() => _subscribers.Remove(onValueChanged));
    }

    // 内部通知メソッド
    private void NotifySubscribers()
    {
        foreach (Action<T> subscriber in _subscribers)
        {
            
            subscriber(_value);
        }
    }

    // 購読解除用のヘルパークラス
    private class Unsubscriber : IDisposable
    {
        private readonly Action _unsubscribe;
        public Unsubscriber(Action unsubscribe) => _unsubscribe = unsubscribe;

        /// <summary>
        /// Disposeを呼ぶと購読解除
        /// </summary>
        public void Dispose()
        {
            _unsubscribe();
        }
    }
}