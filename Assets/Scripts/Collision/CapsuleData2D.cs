using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 一括で管理するため
/// </summary>
public interface IBaseCollisionData2D
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    /// <param name="hitPointA">メソッド使ってるほうの接触点</param>
    /// <param name="hitPointB">対象の接触点</param>
    /// <returns></returns>
    public bool CheckCollision(IBaseCollisionData2D other,out Vector2 hitPointA,out Vector2 hitPointB);
    public bool CheckCollisionWithCapsule(CapsuleData2D capsule, out Vector2 hitPointA, out Vector2 hitPointB);
    public bool CheckCollisionWithCircle(CircleData2D circle, out Vector2 hitPointA, out Vector2 hitPointB);
    public bool CheckCollisionWithLine(LineData2D line, out Vector2 hitPointA, out Vector2 hitPointB);
    public bool CheckCollisionWithBox(BoxData2D box, out Vector2 hitPointA, out Vector2 hitPointB);
}

public class CapsuleData2D:IBaseCollisionData2D
{
    public Vector2 endPoint;
    public Vector2 originPoint;
    public float radius;
    private LineData2D lineData;
    public CapsuleData2D()
    {
        lineData = new LineData2D(originPoint,endPoint);
    }
    public CapsuleData2D(Vector2 origin, Vector2 end, float radius)
    {
        this.endPoint = end;
        this.originPoint = origin;
        this.radius = radius;
    }
    public void SetData(Vector2 origin, Vector2 end, float radius)
    {
        this.endPoint = end;
        this.originPoint = origin;
        this.radius = radius;
    }
    public LineData2D ToLine()
    {
        lineData.SetData(originPoint, endPoint);
        return lineData;
    }
    public bool CheckCollision(IBaseCollisionData2D other,out Vector2 hitPointA,out Vector2 hitPointB)
    {
        return other.CheckCollisionWithCapsule(this, out hitPointA,out hitPointB);
    }
    public bool CheckCollisionWithCapsule(CapsuleData2D capsule, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        return capsule.CheckCollision(this, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithCircle(CircleData2D circle, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        return this.CheckCollision2D(circle, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithLine(LineData2D line, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        return this.CheckCollision2D(line, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithBox(BoxData2D box,out Vector2 hitPointA, out Vector2 hitPointB)
    {
        return this.CheckCollision2D(box, out hitPointA, out hitPointB);
    }
}
public class CircleData2D:IBaseCollisionData2D
{
    public Vector2 position;
    public float radius;
    public CircleData2D(Vector2 position, float radius)
    {
        this.position = position;
        this.radius = radius;
    }
    public void SetData(Vector2 position,float radius)
    {
        this.position = position;
        this.radius = radius;
    }
    public bool CheckCollision(IBaseCollisionData2D other, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        return other.CheckCollisionWithCircle(this, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithCapsule(CapsuleData2D capsule, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        return capsule.CheckCollision(this, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithCircle(CircleData2D circle, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        return this.CheckCollision2D(circle, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithLine(LineData2D line, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        return this.CheckCollision2D(line, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithBox(BoxData2D box, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        return this.CheckCollision2D(box, out hitPointA, out hitPointB);
    }
}
public class LineData2D:IBaseCollisionData2D
{
    public Vector2 endPoint;
    public Vector2 originPoint;
    public LineData2D(Vector2 origin, Vector2 end)
    {
        this.endPoint = end;
        this.originPoint = origin;
    }
    public void SetData(Vector2 origin, Vector2 end)
    {
        this.endPoint = end;
        this.originPoint = origin;
    }
    public bool CheckCollision(IBaseCollisionData2D other, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        return other.CheckCollisionWithLine(this, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithCapsule(CapsuleData2D capsule, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        return capsule.CheckCollision(this, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithCircle(CircleData2D circle, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        return this.CheckCollision2D(circle, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithLine(LineData2D line, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        return this.CheckCollision2D(line, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithBox(BoxData2D box, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        return this.CheckCollision2D(box, out hitPointA, out hitPointB);
    }

}
public class BoxData2D : IBaseCollisionData2D
{
    public Vector2 originPoint; // ボックスの開始点
    public Vector2 endPoint;    // ボックスの終了点
    public Vector2 boxCenter;   // ボックスの中心点
    public Vector2 forward;     // 中心線の方向ベクトル
    public float boxWidth;      // ボックスの幅

    private Vector2[] vertices = new Vector2[4]; // ボックスの頂点
    private Vector2[][] edges = new Vector2[4][]; // ボックスのエッジ

    private LineData2D lineData;

    // コンストラクタ
    public BoxData2D(Vector2 originPoint, Vector2 endPoint, float boxWidth)
    {
        this.originPoint = originPoint;
        this.endPoint = endPoint;
        this.boxWidth = boxWidth;

        // 必要なデータを更新
        this.boxCenter = (originPoint + endPoint) / 2;
        this.forward = (endPoint - originPoint).normalized;
        // 頂点とエッジを初期化
        CalculateVerticesAndEdges();
        lineData = new LineData2D(originPoint, endPoint);
    }

    // データをセットするメソッド
    public void SetData(Vector2 originPoint, Vector2 endPoint, float boxWidth)
    {
        this.originPoint = originPoint;
        this.endPoint = endPoint;
        this.boxWidth = boxWidth;

        // 必要なデータを更新
        this.boxCenter = (originPoint + endPoint) / 2;
        this.forward = (endPoint - originPoint).normalized;

        // 頂点とエッジを再計算
        CalculateVerticesAndEdges();
        
    }
    // ボックスの頂点とエッジを計算
    private void CalculateVerticesAndEdges()
    {
        // 中心線の垂直方向のベクトルを計算
        Vector2 perpendicular = new Vector2(-forward.y, forward.x) * (boxWidth / 2);

        // ボックスの4つの頂点を計算
        vertices[0] = originPoint - perpendicular; // 左下
        vertices[1] = endPoint - perpendicular;   // 右下
        vertices[2] = endPoint + perpendicular;   // 右上
        vertices[3] = originPoint + perpendicular; // 左上

        // ボックスのエッジを構成
        edges[0] = new Vector2[] { vertices[0], vertices[1] }; // 下辺
        edges[1] = new Vector2[] { vertices[1], vertices[2] }; // 右辺
        edges[2] = new Vector2[] { vertices[2], vertices[3] }; // 上辺
        edges[3] = new Vector2[] { vertices[3], vertices[0] }; // 左辺
    }
    // LineData2D に変換
    public LineData2D ToLine()
    {
        lineData.SetData(originPoint, endPoint);
        return lineData;
    }

    public bool CheckCollision(IBaseCollisionData2D other, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        return other.CheckCollisionWithBox(this, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithCapsule(CapsuleData2D capsule, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        return capsule.CheckCollision(this, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithCircle(CircleData2D circle, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        return this.CheckCollision2D(circle, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithLine(LineData2D line, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        return this.CheckCollision2D(line, out hitPointA, out hitPointB);
    }
    public bool CheckCollisionWithBox(BoxData2D box, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        return this.CheckCollision2D(box, out hitPointA, out hitPointB);
    }



    // ボックスの全頂点を取得
    public Vector2[] GetVertices()
    {
        return vertices;
    }

    // ボックスの全エッジを取得
    public Vector2[][] GetEdges()
    {
        return edges;
    }
}
public struct CollisionData2D
{
    public readonly Vector2 hitPoint;
    public readonly ICollisionable2D collisionable;
    public readonly Vector2 depth;
    public CollisionData2D(Vector2 hitpoint,ICollisionable2D collisionable,Vector2 depth)
    {
        this.hitPoint = hitpoint;
        this.collisionable = collisionable;
        this.depth = depth;
    }


}



