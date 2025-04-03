using SabaSimpleDIContainer.Unity;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SabaSimpleDIContainer.Unity
{
    public interface ITickable
    {
        void Tick();
    }
    public class Tickable
    {
        private readonly List<ITickable> _tickables = new List<ITickable>();
        private readonly int _frameInterval; // ミリ秒単位
        private CancellationTokenSource _cancellationTokenSource;
        private readonly SynchronizationContext _synchronizationContext;

        public Tickable(int targetFps = 60, SynchronizationContext context = null)
        {
            _frameInterval = 1000 / targetFps;
            _synchronizationContext = context; // Unity以外の環境ではnullになる可能性
        }
        public Tickable(int targetFps = 60)
        {
            _frameInterval = 1000 / targetFps;
            _synchronizationContext = null;
        }
        public Tickable(SynchronizationContext context)
        {
            _frameInterval = 1000 / 60;
            _synchronizationContext = context;
        }

        public void AddTickable(ITickable tickable)
        {
            if (!_tickables.Contains(tickable))
            {
                _tickables.Add(tickable);
            }
        }

        public void RemoveTickable(ITickable tickable)
        {
            _tickables.Remove(tickable);
        }

        public async Task Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;

            while (!token.IsCancellationRequested)
            {
               
                foreach (var tickable in _tickables)
                {
                    if (_synchronizationContext != null)
                    {
                        // SynchronizationContextが設定されている場合、Postを使用
                        _synchronizationContext.Post(_ => tickable.Tick(), null);
                    }
                    else
                    {
                        // SynchronizationContextがない場合、その場で実行
                        tickable.Tick();
                    }
                }

                await Task.Delay(_frameInterval, token);
            }
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        
    }
}