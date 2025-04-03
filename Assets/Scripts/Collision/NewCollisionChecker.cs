using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SabaSimpleDIContainer;
using SabaSimpleDIContainer.Pipe;


public interface ICollisionChecker
{
    public void CheckCollisions();

}


public class   NewCollisionChecker:ICollisionChecker
{
    public List<ICollisionable2D> _collisionables { get; private set; } = new();
    private ISubscriber<CollisionableAddEvent> _collisioinableAddEvent;
    [Injection]
    private void CollisionableAdd(ISubscriber<CollisionableAddEvent> subscriber)
    {
        
        _collisioinableAddEvent = subscriber;
        _collisioinableAddEvent.Subscribe(OnCollisionableAdded);


    }
    //追加通知を受け取ったら管理対象に追加
    private void OnCollisionableAdded(CollisionableAddEvent addEvent)
    {
        
        AddObserveCollision(addEvent.collisionable);

    }

    public NewCollisionChecker()
    {
        GetAllCollisionable();
    }
    /// <summary>
    /// 登録されているICollisionable同士が接触しているか判定をとる
    /// </summary>
    public void CheckCollisions()
    {
        //ループ
        //AABB的な判定付けてもいいかも
        for (int i = 0; i < _collisionables.Count; i++)
        {
            //非接触モードだったら次へ
            if (_collisionables[i].CheckCollisionMode == CheckCollisionMode.dontCollisionable)
            {
                continue;
            }
            for (int j = i + 1; j < _collisionables.Count; j++)
            {
                
                //非接触モードだったら次へ
                if (_collisionables[j].CheckCollisionMode == CheckCollisionMode.dontCollisionable)
                {
                    continue;
                }

                //接触判定
                //判定->レイヤー判定は逆のほうがいいかも
                //hitBが自分
                //hitAが相手
                if (_collisionables[i].BaseData.CheckCollision(_collisionables[j].BaseData, out Vector2 hitPointA, out Vector2 hitPointB))
                {
                    Vector2 depth = GetDepth(hitPointB, hitPointA);
                    //if(IsLayerInMask(_collisionables[i].CollisionableLayer, _collisionables[j].gameObject)|| IsLayerInMask(_collisionables[j].CollisionableLayer, _collisionables[i].gameObject))
                    //{
                    //    Debug.Log(_collisionables[i].gameObject.transform.name +"B=" + hitPointB+ "Depth=" + depth + _collisionables[j].gameObject.transform.name +"A=" + hitPointA + "Depth=" + -depth);
                    //}
                    
                    if (IsLayerInMask(_collisionables[i].CollisionableLayer, _collisionables[j].gameObject))
                    {
                        CollisionData2D collisionDataI = new(hitPointB, _collisionables[j], depth);
                        _collisionables[i].OnCollisionEvent(collisionDataI);
                    }

                    if (IsLayerInMask(_collisionables[j].CollisionableLayer, _collisionables[i].gameObject))
                    {
                        CollisionData2D collisionDataJ = new(hitPointA, _collisionables[i], -depth);
                        _collisionables[j].OnCollisionEvent(collisionDataJ);
                    }
                }
            }
        }
    }

    private bool IsLayerInMask(LayerMask mask, GameObject taraget)
    {
        return (mask & (1 << taraget.layer)) != 0;
    }

    public void AddObserveCollision(ICollisionable2D collisionable)
    {

        _collisionables.Add(collisionable);

    }
    public void AddObserveCollisions(List<ICollisionable2D> collisionables)
    {
        _collisionables.AddRange(collisionables);
    }
    public void AddObserveCollisions(ICollisionable2D[] collisionables)
    {
        _collisionables.AddRange(collisionables);
    }
    /// <summary>
    /// 最初に使う
    /// </summary>
    private void GetAllCollisionable()
    {
        AddObserveCollisions(FindObjectsByInterface<ICollisionable2D>());
    }
    /// <summary>
    /// hitPointAがhitPointBにどれだけ浸入しているかを返す
    /// </summary>
    /// <param name="hitPointA"></param>
    /// <param name="hitPointB"></param>
    private Vector2 GetDepth(Vector2 hitPointA, Vector2 hitPointB)
    {
        return hitPointB - hitPointA;
    }
    public static List<T> FindObjectsByInterface<T>() where T : class
    {
        List<T> results = new();
        MonoBehaviour[] monoBehaviours = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (MonoBehaviour monoBehaviour in monoBehaviours)
        {
            if (monoBehaviour is T targetInterface)
            {
                results.Add(targetInterface);

            }
        }

        return results;
    }
}
