using System;
using System.Collections.Generic;


namespace SabaSimpleDIContainer.Pipe
{
   
    public interface IPublisher<T>
    {
        public void Publish(T message);


    }
    public interface ISubscriber<T>
    {
        public void Subscribe(Action<T> subscriber);
        public void Unsubscribe(Action<T> subscriber);
    }

    public class MessageBroker<T>:IPublisher<T>,ISubscriber<T>
    {
        private List<Action<T>> _subscribers = new List<Action<T>>();
        public MessageBroker()
        {
            
        }
        public void Publish(T message)
        {
            foreach (Action<T> subscriber in _subscribers)
            {
                subscriber.Invoke(message);
            }
        }

        public void Subscribe(Action<T> subscriber)
        {
            _subscribers.Add(subscriber);
        }
        public void Unsubscribe(Action<T> subscriber)
        {
            _subscribers.Remove(subscriber);
        }
    }

    public static class ContainerExpantion
    {
        public static void RegisterBroker<TMessage>(this IContainer container)
        {
            MessageBroker<TMessage> broker = new MessageBroker<TMessage>();
            container.Register<IPublisher<TMessage>>(broker, LifeTime.singleton);
            container.Register<ISubscriber<TMessage>>(broker, LifeTime.singleton);
            
        }
    }

}
