using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SabaSimpleDIContainer;
using SabaSimpleDIContainer.Pipe;
using System.Text.RegularExpressions;



public class CollisionChecker:ICollisionChecker
{
    public List<ICollisionable2D> _collisionables { get; private set; } = new();
    private ISubscriber<CollisionableAddEvent> _collisioinableAddEvent;
    [Injection]
    private void CollisionableAdd(ISubscriber<CollisionableAddEvent> subscriber)
    {
        _collisioinableAddEvent = subscriber;
        _collisioinableAddEvent.Subscribe(OnCollisionableAdded);


    }
    //’Ç‰Á’Ê’m‚ğó‚¯æ‚Á‚½‚çŠÇ—‘ÎÛ‚É’Ç‰Á
    private void OnCollisionableAdded(CollisionableAddEvent addEvent)
    {
        Debug.Log("added Collisionable"+addEvent.collisionable.gameObject.transform.name);
        AddObserveCollision(addEvent.collisionable);
        
    }

    public CollisionChecker()
    {
        GetAllCollisionable();
    }
    /// <summary>
    /// “o˜^‚³‚ê‚Ä‚¢‚éICollisionable“¯m‚ªÚG‚µ‚Ä‚¢‚é‚©”»’è‚ğ‚Æ‚é
    /// </summary>
    public void CheckCollisions()
    {
        

        for (int i = 0; i < _collisionables.Count; i++)
        {

            if (_collisionables[i].CheckCollisionMode == CheckCollisionMode.dontCollisionable)
            {
                continue;
            }
            for (int j = i + 1; j < _collisionables.Count; j++)
            {
                
                if (_collisionables[j].CheckCollisionMode == CheckCollisionMode.dontCollisionable)
                {
                    continue;
                }
                
                switch (_collisionables[j].BaseData)
                {
                    case CapsuleData2D capsule:
                        {
                           
                            
                            
                            if (_collisionables[i].BaseData.CheckCollision(capsule,out Vector2 hitPointA,out Vector2 hitPointB))
                            {
                                Vector2 depth = GetDepth(hitPointA,hitPointB);
                                if(IsLayerInMask(_collisionables[i].CollisionableLayer, _collisionables[j].gameObject))
                                {
                                    CollisionData2D collisionDataI = new(hitPointA, _collisionables[j], depth);
                                    _collisionables[i].OnCollisionEvent(collisionDataI);
                                }
                                
                                if(IsLayerInMask(_collisionables[j].CollisionableLayer, _collisionables[i].gameObject))
                                {
                                    CollisionData2D collisionDataJ = new(hitPointB, _collisionables[i], -depth);
                                    _collisionables[j].OnCollisionEvent(collisionDataJ);
                                }
                               
                            }
                            break;
                        }
                    case CircleData2D circle:
                        {

                            if (_collisionables[i].BaseData.CheckCollision(circle, out Vector2 hitPointA, out Vector2 hitPointB))
                            {
                                Vector2 depth = GetDepth(hitPointA, hitPointB);
                                if (IsLayerInMask(_collisionables[i].CollisionableLayer, _collisionables[j].gameObject))
                                {
                                    CollisionData2D collisionDataI = new(hitPointA, _collisionables[j], depth);
                                    _collisionables[i].OnCollisionEvent(collisionDataI);
                                }

                                if (IsLayerInMask(_collisionables[j].CollisionableLayer, _collisionables[i].gameObject))
                                {
                                    CollisionData2D collisionDataJ = new(hitPointB, _collisionables[i], -depth);
                                    _collisionables[j].OnCollisionEvent(collisionDataJ);
                                }

                            }
                            break;
                        }
                    case LineData2D line:
                        {
                            if (_collisionables[i].BaseData.CheckCollision(line, out Vector2 hitPointA, out Vector2 hitPointB))
                            {
                                Vector2 depth = GetDepth(hitPointA, hitPointB);
                                if (IsLayerInMask(_collisionables[i].CollisionableLayer, _collisionables[j].gameObject))
                                {
                                    CollisionData2D collisionDataI = new(hitPointA, _collisionables[j], depth);
                                    _collisionables[i].OnCollisionEvent(collisionDataI);
                                }

                                if (IsLayerInMask(_collisionables[j].CollisionableLayer, _collisionables[i].gameObject))
                                {
                                    CollisionData2D collisionDataJ = new(hitPointB, _collisionables[i], -depth);
                                    _collisionables[j].OnCollisionEvent(collisionDataJ);
                                }

                            }
                            break;
                        }
                    case BoxData2D box:
                        {
                            if (_collisionables[i].BaseData.CheckCollision(box, out Vector2 hitPointA, out Vector2 hitPointB))
                            {
                                Vector2 depth = GetDepth(hitPointA, hitPointB);
                                if (IsLayerInMask(_collisionables[i].CollisionableLayer, _collisionables[j].gameObject))
                                {
                                    CollisionData2D collisionDataI = new(hitPointA, _collisionables[j], depth);
                                    _collisionables[i].OnCollisionEvent(collisionDataI);
                                }

                                if (IsLayerInMask(_collisionables[j].CollisionableLayer, _collisionables[i].gameObject))
                                {
                                    CollisionData2D collisionDataJ = new(hitPointB, _collisionables[i], -depth);
                                    _collisionables[j].OnCollisionEvent(collisionDataJ);
                                }

                            }
                            break;
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
    /// Å‰‚Ég‚¤
    /// </summary>
    private void GetAllCollisionable()
    {
        AddObserveCollisions(FindObjectsByInterface<ICollisionable2D>());
    }
    /// <summary>
    /// hitPointA‚ªhitPointB‚É‚Ç‚ê‚¾‚¯Z“ü‚µ‚Ä‚¢‚é‚©‚ğ•Ô‚·
    /// </summary>
    /// <param name="hitPointA"></param>
    /// <param name="hitPointB"></param>
    private Vector2 GetDepth(Vector2 hitPointA,Vector2 hitPointB)
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
