using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// 一括で管理するため
/// </summary>
public interface IBaseCollisionData3D
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <param name="hitPointA">メソッド使ってるほうの接触点</param>
    /// <param name="hitPointB">対象の接触点</param>
    /// <returns></returns>
    public bool CheckCollision(IBaseCollisionData3D other, out Vector3 hitPointA, out Vector3 hitPointB);
    public bool CheckCollisionWithCapsule(CapsuleData3D capsule, out Vector3 hitPointA, out Vector3 hitPointB);
    public bool CheckCollisionWithCircle(CircleData3D circle, out Vector3 hitPointA, out Vector3 hitPointB);
    public bool CheckCollisionWithLine(LineData3D line, out Vector3 hitPointA, out Vector3 hitPointB);
    public bool CheckCollisionWithBox(BoxData3D box, out Vector3 hitPointA, out Vector3 hitPointB);

}

public class CapsuleData3D : IBaseCollisionData3D
{
    public Vector3 endPoint;
    public Vector3 originPoint;
    public float radius;
    private LineData3D lineData;
    public CapsuleData3D()
    {
        lineData = new LineData3D(originPoint, endPoint);
    }
    public CapsuleData3D(Vector3 origin, Vector3 end, float radius)
    {
        this.endPoint = end;
        this.originPoint = origin;
        this.radius = radius;
        lineData = new LineData3D(originPoint, endPoint);
    }
    public void SetData(Vector3 origin, Vector3 end, float radius)
    {
        this.endPoint = end;
        this.originPoint = origin;
        this.radius = radius;
    }
    public LineData3D ToLine()
    {
        
        lineData.SetData(originPoint, endPoint);
        return lineData;
    }
    public bool CheckCollision(IBaseCollisionData3D other, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return other.CheckCollisionWithCapsule(this, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithCapsule(CapsuleData3D capsule, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return this.CheckCollision3D(capsule, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithCircle(CircleData3D circle, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return this.CheckCollision3D(circle, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithLine(LineData3D line, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return this.CheckCollision3D(line, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithBox(BoxData3D box, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return this.CheckCollision3D(box, out hitPointA, out hitPointB);
    }

    
}
public class CircleData3D : IBaseCollisionData3D
{
    public Vector3 position;
    public float radius;
    public CircleData3D(Vector3 position, float radius)
    {
        this.position = position;
        this.radius = radius;
    }

    public bool CheckCollision(IBaseCollisionData3D other, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return other.CheckCollisionWithCircle(this,out hitPointA,out hitPointB);
    }

    public bool CheckCollisionWithBox(BoxData3D box, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return this.CheckCollision3D(box, out hitPointA, out hitPointB);
    }

    public bool CheckCollisionWithCapsule(CapsuleData3D capsule, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return this.CheckCollision3D(capsule, out hitPointA, out hitPointB);
    }

    public bool CheckCollisionWithCircle(CircleData3D circle, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return this.CheckCollision3D(circle, out hitPointA, out hitPointB);
    }

    public bool CheckCollisionWithLine(LineData3D line, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return this.CheckCollision3D(line, out hitPointA, out hitPointB);
    }

    public void SetData(Vector3 position, float radius)
    {
        this.position = position;
        this.radius = radius;
    }
   
}
public class LineData3D : IBaseCollisionData3D
{
    public Vector3 endPoint;
    public Vector3 originPoint;
    public LineData3D(Vector3 origin, Vector3 end)
    {
        this.endPoint = end;
        this.originPoint = origin;
    }
    public void SetData(Vector3 origin, Vector3 end)
    {
        this.endPoint = end;
        this.originPoint = origin;
    }
   

    public bool CheckCollision(IBaseCollisionData3D other, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return other.CheckCollisionWithLine(this, out hitPointA, out hitPointB);
    }

    public bool CheckCollisionWithCapsule(CapsuleData3D capsule, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return this.CheckCollision3D(capsule, out hitPointA, out hitPointB);
    }

    public bool CheckCollisionWithCircle(CircleData3D circle, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return this.CheckCollision3D(circle, out hitPointA, out hitPointB);
    }

    public bool CheckCollisionWithLine(LineData3D line, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return this.CheckCollision3D(line, out hitPointA, out hitPointB);
    }

    public bool CheckCollisionWithBox(BoxData3D box, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return this.CheckCollision3D(box, out hitPointA, out hitPointB);
    }
}
public class BoxData3D : IBaseCollisionData3D
{
    public Vector3 originPoint;
    public Vector3 endPoint;
    public float boxWidth;
    public Vector3 boxCenter;
    public Vector3 forward;
    private LineData3D lineData;
    private Vector3[] vertices = new Vector3[8];
    private Vector3[][] edges = new Vector3[12][];

    public BoxData3D(Vector3 originPoint, Vector3 endPoint, float boxWidth)
    {
        this.originPoint = originPoint;
        this.endPoint = endPoint;
        this.boxWidth = boxWidth;
        this.boxCenter = (originPoint + endPoint) / 2;
        this.forward = (endPoint - originPoint).normalized;
        CalculateVerticesAndEdges();
        lineData = new LineData3D(originPoint, endPoint);
    }

    public void SetData(Vector3 originPoint, Vector3 endPoint, float boxWidth)
    {
        this.originPoint = originPoint;
        this.endPoint = endPoint;
        this.boxWidth = boxWidth;
        this.boxCenter = (originPoint + endPoint) / 2;
        this.forward = (endPoint - originPoint).normalized;
        CalculateVerticesAndEdges();
    }

    private void CalculateVerticesAndEdges()
    {
        Vector3 perpendicular1 = Vector3.Cross(forward, Vector3.up).normalized * (boxWidth / 2);
        Vector3 perpendicular2 = Vector3.Cross(forward, perpendicular1).normalized * (boxWidth / 2);

        vertices[0] = originPoint - perpendicular1 - perpendicular2;
        vertices[1] = originPoint + perpendicular1 - perpendicular2;
        vertices[2] = originPoint + perpendicular1 + perpendicular2;
        vertices[3] = originPoint - perpendicular1 + perpendicular2;
        vertices[4] = endPoint - perpendicular1 - perpendicular2;
        vertices[5] = endPoint + perpendicular1 - perpendicular2;
        vertices[6] = endPoint + perpendicular1 + perpendicular2;
        vertices[7] = endPoint - perpendicular1 + perpendicular2;

        edges[0] = new Vector3[] { vertices[0], vertices[1] };
        edges[1] = new Vector3[] { vertices[1], vertices[2] };
        edges[2] = new Vector3[] { vertices[2], vertices[3] };
        edges[3] = new Vector3[] { vertices[3], vertices[0] };
        edges[4] = new Vector3[] { vertices[4], vertices[5] };
        edges[5] = new Vector3[] { vertices[5], vertices[6] };
        edges[6] = new Vector3[] { vertices[6], vertices[7] };
        edges[7] = new Vector3[] { vertices[7], vertices[4] };
        edges[8] = new Vector3[] { vertices[0], vertices[4] };
        edges[9] = new Vector3[] { vertices[1], vertices[5] };
        edges[10] = new Vector3[] { vertices[2], vertices[6] };
        edges[11] = new Vector3[] { vertices[3], vertices[7] };
    }

    public LineData3D ToLine()
    {
        lineData.SetData(originPoint, endPoint);
        return lineData;
    }


    public bool CheckCollision(IBaseCollisionData3D other, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return other.CheckCollisionWithBox(this, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithCapsule(CapsuleData3D capsule, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return this.CheckCollision3D(capsule, out hitPointA, out hitPointB);
    }

    public bool CheckCollisionWithSphere(CircleData3D sphere, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return this.CheckCollision3D(sphere, out hitPointA, out hitPointB);
    }

    public bool CheckCollisionWithLine(LineData3D line, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return this.CheckCollision3D(line, out hitPointA, out hitPointB);
    }

    public bool CheckCollisionWithBox(BoxData3D box, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return this.CheckCollision3D(box, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithCircle(CircleData3D circle, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        return this.CheckCollision3D(circle, out hitPointA, out hitPointB);
    }
    public Vector3[] GetVertices()
    {
        return vertices;
    }

    public Vector3[][] GetEdges()
    {
        return edges;
    }

   

    
}
public struct CollisionData3D
{
    public readonly Vector3 hitPoint;
    public readonly ICollisionable3D collisionable;
    public readonly Vector3 depth;
    public CollisionData3D(Vector3 hitpoint, ICollisionable3D collisionable, Vector3 depth)
    {
        this.hitPoint = hitpoint;
        this.collisionable = collisionable;
        this.depth = depth;
    }


}



