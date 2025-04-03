using UnityEngine;

public class TestCapsule : MonoBehaviour,ICollisionable3D
{
    IBaseCollisionData3D ICollisionable3D.BaseData => _capsuleData;
    private CapsuleData3D _capsuleData;
    CheckCollisionMode ICollisionable3D.CheckCollisionMode =>CheckCollisionMode.collisionable;
    [SerializeField]
    private LayerMask _mask;
    LayerMask ICollisionable3D.CollisionableLayer => _mask;

    

    void ICollisionable3D.OnCollisionEvent(CollisionData3D collisionable)
    {
        throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
