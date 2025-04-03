using System;
using UnityEngine;




public class CollisionableAddEvent
{
    public readonly ICollisionable2D collisionable;
    public CollisionableAddEvent(ICollisionable2D collisionable)
    {
        this.collisionable = collisionable;
    }
}

