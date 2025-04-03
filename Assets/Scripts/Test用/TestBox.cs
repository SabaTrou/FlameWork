using UnityEngine;

public class TestBox : MonoBehaviour,ICollisionable3D
{
    IBaseCollisionData3D ICollisionable3D.BaseData => _box;
    private BoxData3D _box;
    CheckCollisionMode ICollisionable3D.CheckCollisionMode =>CheckCollisionMode.collisionable;
    [SerializeField]
    private LayerMask _mask;
    LayerMask ICollisionable3D.CollisionableLayer => _mask;

   

    void ICollisionable3D.OnCollisionEvent(CollisionData3D collisionable)
    {
        Debug.Log(collisionable);
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
