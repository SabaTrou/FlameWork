using System.Collections.Generic;
using SabaSimpleDIContainer;

namespace SabaSimpleDIContainer
{
    public interface IStartable
    {
        void Start();
    }
    public class Startable
    {
        private readonly List<IStartable> startables = new List<IStartable>();

        // IStartableを登録
        public void AddStartable(IStartable startable)
        {
            if (!startables.Contains(startable))
            {
                startables.Add(startable);
            }
        }

        // 登録済みのIStartableを一括で初期化
        public void StartAll()
        {
            foreach (var startable in startables)
            {
                startable.Start();
            }
        }
    }
   
}