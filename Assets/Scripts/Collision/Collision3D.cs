using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Collision3D
{
    #region 計算
    private static Vector3 GetNearestPoint3D(Vector3 origin, Vector3 end, Vector3 position)
    {
        //カプセルの始点から終点のベクトルを正規化したもの
        Vector3 vector1 = (end - origin).normalized;
        //始点から点へのベクトル
        Vector3 originToPoint = position - origin;
        //終点から点へのベクトル
        Vector3 endToPoint = position - end;

        if (0 > Vector3.Dot(vector1, originToPoint))
        {

            return origin;
        }
        else if (0 < Vector3.Dot(vector1, endToPoint))
        {
            return end;
        }

        Vector3 point = origin + vector1 * Vector3.Dot(vector1, originToPoint);

        return point;
    }
    private static Vector3 GetNearestPoint3D(LineData3D lineData1, LineData3D lineData2)
    {
        Vector3 originNearPoint = GetNearestPoint3D(lineData1.originPoint, lineData1.endPoint, lineData2.originPoint);
        float originDistance = GetDistance3D(originNearPoint, lineData2.originPoint);
        Vector3 endNearPoint = GetNearestPoint3D(lineData1.originPoint, lineData1.endPoint, lineData2.endPoint);
        float endDistance = GetDistance3D(endNearPoint, lineData2.endPoint);
        if (originDistance <= endDistance)
        {
            return originNearPoint;
        }
        return endNearPoint;
    }


    private static float GetDistance3D(Vector3 point1, Vector3 point2)
    {

        Vector3 distanceVec = point1 - point2;
        float distance = Mathf.Sqrt(
            Mathf.Pow(Mathf.Abs(distanceVec.x), 2) +
            Mathf.Pow(Mathf.Abs(distanceVec.y), 2) +
            Mathf.Pow(Mathf.Abs(distanceVec.z), 2)
            );
        return distance;
    }
    private static float GetLinesDistance3D(LineData3D lineData1, LineData3D lineData2)
    {
        Vector3 originNearPoint = GetNearestPoint3D(lineData1.originPoint, lineData1.endPoint, lineData2.originPoint);
        float originDistance = GetDistance3D(originNearPoint, lineData2.originPoint);
        Vector3 endNearPoint = GetNearestPoint3D(lineData1.originPoint, lineData1.endPoint, lineData2.endPoint);
        float endDistance = GetDistance3D(endNearPoint, lineData2.endPoint);
        if (originDistance <= endDistance)
        {
            return originDistance;
        }
        return endDistance;
    }
    #endregion
    #region capsule
    public static bool CheckCollision2D(this CapsuleData2D capsuleData, IBaseCollisionData2D collisionData, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        switch (collisionData)
        {
            case CircleData2D circle:
                {
                    return capsuleData.CheckCollision2D(circle, out hitPointA, out hitPointB);
                }
            case CapsuleData2D capsule:
                {
                    return capsuleData.CheckCollision2D(capsule, out hitPointA, out hitPointB);
                }
            case LineData2D line:
                {
                    return capsuleData.CheckCollision2D(line, out hitPointA, out hitPointB);
                }
            case BoxData2D box:
                {
                    return capsuleData.CheckCollision2D(box, out hitPointA, out hitPointB);
                }
        }
        hitPointA = default(Vector3);
        hitPointB = default(Vector3);
        return false;
    }

    public static bool CheckCollision3D(this CapsuleData3D capsuleData, Vector3 position, out Vector3 hitPoint)
    {
        Vector3 point = GetNearestPoint3D(capsuleData.originPoint, capsuleData.endPoint, position);
        //Debug.Log(point);
        float distance = GetDistance3D(position, point);
        if (distance - capsuleData.radius <= 0)
        {
            hitPoint = position;
            return true;
        }
        hitPoint = default;
        return false;
    }

    /// <summary>
    /// カプセルと円の3D衝突判定を行い、両側の接触点を計算します。
    /// </summary>
    /// <param name="capsuleData">カプセルデータ</param>
    /// <param name="circleData">円データ</param>
    /// <param name="hitPointA">カプセル基準の接触点</param>
    /// <param name="hitPointB">円基準の接触点</param>
    /// <returns>衝突している場合はtrue</returns>
    public static bool CheckCollision3D(this CapsuleData3D capsuleData, CircleData3D circleData, out Vector3 hitPointA, out Vector3 hitPointB)
    {

        Vector3 point = GetNearestPoint3D(capsuleData.originPoint, capsuleData.endPoint, circleData.position);

        // 円の中心と最近点のベクトルおよび距離を計算
        Vector3 direction = circleData.position - point;
        float distance = GetDistance3D(circleData.position, point);
        if (distance - (capsuleData.radius + circleData.radius) <= 0)
        {
            // カプセル基準の接触点
            hitPointA = point + direction.normalized * capsuleData.radius;
            // 円基準の接触点
            hitPointB = circleData.position - direction.normalized * circleData.radius;
            return true;
        }
        // 衝突していない場合はデフォルト値を返す
        hitPointA = hitPointB = default;
        return false;
    }
    /// <summary>
    /// カプセルと線分の3D衝突判定を行い、両側の接触点を計算します。
    /// </summary>
    /// <param name="capsuleData">カプセルデータ</param>
    /// <param name="lineData">線分データ</param>
    /// <param name="hitPointA">カプセル基準の接触点</param>
    /// <param name="hitPointB">線分基準の接触点</param>
    /// <returns>衝突している場合はtrue</returns>
    public static bool CheckCollision3D(this CapsuleData3D capsuleData, LineData3D lineData, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        // 線分上の最近点を計算
        Vector3 lineNearestPoint = GetNearestPoint3D(lineData.originPoint, lineData.endPoint, capsuleData.originPoint);

        // カプセルの中心線上の最近点を計算
        Vector3 capsuleNearestPoint = GetNearestPoint3D(capsuleData.originPoint, capsuleData.endPoint, lineData.originPoint);
        float distance = GetLinesDistance3D(capsuleData.ToLine(), lineData);

        if (distance - (capsuleData.radius + capsuleData.radius) <= 0)
        {
            // 接触点を計算
            hitPointA = capsuleNearestPoint;  // カプセル基準の接触点
            hitPointB = lineNearestPoint;    // 線分基準の接触点
            return true;
        }
        hitPointA = hitPointB = default;
        return false;
    }

    /// <summary>
    /// 2つのカプセルの3D衝突判定を行い、両側の接触点を計算します。
    /// </summary>
    /// <param name="capsuleData1">1つ目のカプセルデータ</param>
    /// <param name="capsuleData2">2つ目のカプセルデータ</param>
    /// <param name="hitPointA">1つ目のカプセル基準の接触点</param>
    /// <param name="hitPointB">2つ目のカプセル基準の接触点</param>
    /// <returns>衝突している場合はtrue</returns>
    public static bool CheckCollision3D(this CapsuleData3D capsuleData1, CapsuleData3D capsuleData2, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        // 両カプセルの線分上の最近点を取得
        Vector3 nearestPoint1 = GetNearestPoint3D(capsuleData1.originPoint, capsuleData1.endPoint, capsuleData2.originPoint);
        Vector3 nearestPoint2 = GetNearestPoint3D(capsuleData2.originPoint, capsuleData2.endPoint, capsuleData1.originPoint);
        float distance = GetLinesDistance3D(capsuleData1.ToLine(), capsuleData2.ToLine());
        // 最近点間の距離を計算
        Vector3 direction = nearestPoint2 - nearestPoint1;
        if (distance - (capsuleData1.radius + capsuleData2.radius) <= 0)
        {
            // 1つ目のカプセル基準の接触点
            hitPointA = nearestPoint1 + direction.normalized * capsuleData1.radius;
            // 2つ目のカプセル基準の接触点
            hitPointB = nearestPoint2 - direction.normalized * capsuleData2.radius;
            return true;
        }

        // 衝突していない場合はデフォルト値を返す
        hitPointA = hitPointB = default;
        return false;
    }

    /// <summary>
    /// カプセルとボックスの2D衝突判定を行い、両側の接触点を計算します。
    /// </summary>
    /// <param name="capsule">カプセルデータ</param>
    /// <param name="box">ボックスデータ</param>
    /// <param name="hitPointA">カプセル基準の接触点</param>
    /// <param name="hitPointB">ボックス基準の接触点</param>
    /// <returns>衝突している場合はtrue</returns>
    public static bool CheckCollision3D(this CapsuleData3D capsule, BoxData3D box, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        hitPointA = hitPointB = default;

        // ボックスのエッジリストを取得
        Vector3[][] edges = box.GetEdges();
        float minDistance = float.MaxValue;
        Vector3 closestCapsulePoint = default;
        Vector3 closestBoxPoint = default;
        // ボックスの各エッジについて判定
        foreach (Vector3[] edge in edges)
        {
            Vector3 edgeStart = edge[0];
            Vector3 edgeEnd = edge[1];

            // カプセルの線分とボックスのエッジ間の最近点を計算
            Vector3 capsuleNearestPoint = GetNearestPoint3D(capsule.originPoint, capsule.endPoint, edgeStart);
            Vector3 edgeNearestPoint = GetNearestPoint3D(edgeStart, edgeEnd, capsuleNearestPoint);

            // 最近点間の距離を計算
            float distance = GetDistance3D(capsuleNearestPoint, edgeNearestPoint);

            // 最短距離の更新
            if (distance < minDistance)
            {
                minDistance = distance;
                closestCapsulePoint = capsuleNearestPoint;
                closestBoxPoint = edgeNearestPoint;
            }
        }
        // 衝突判定: 最短距離がカプセルの半径以下か
        if (minDistance <= capsule.radius)
        {
            // 接触点を設定
            Vector3 direction = (closestBoxPoint - closestCapsulePoint).normalized;
            hitPointA = closestCapsulePoint + direction * capsule.radius; // カプセル基準の接触点
            hitPointB = closestBoxPoint; // ボックス基準の接触点
            return true;
        }

        return false;
    }
    #endregion
    #region circle
    /// <summary>
    /// CircleData2Dと任意のオブジェクトの衝突判定。
    /// </summary>
    public static bool CheckCollision3D(this CircleData3D circleData, IBaseCollisionData3D collisionData, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        switch (collisionData)
        {
            case CircleData3D circle:
                return circleData.CheckCollision3D(circle, out hitPointA, out hitPointB);
            case CapsuleData3D capsule:
                return circleData.CheckCollision3D(capsule, out hitPointA, out hitPointB);
            case LineData3D line:
                return circleData.CheckCollision3D(line, out hitPointA, out hitPointB);
            case BoxData3D box:
                return circleData.CheckCollision3D(box, out hitPointA, out hitPointB);
        }

        // 対応していない型の場合は衝突なしとみなす
        hitPointA = hitPointB = default;
        return false;
    }



    public static bool CheckCollision3D(this CircleData3D circleData, CircleData3D circleData1, out Vector3 hitPointA, out Vector3 hitPointB)
    {


        float distance = GetDistance3D(circleData.position, circleData1.position);

        if (distance - (circleData.radius + circleData1.radius) <= 0)
        {
            // それぞれの接触点を計算
            Vector3 direction = (circleData1.position - circleData.position).normalized;
            hitPointA = circleData.position + direction * circleData.radius; // circleData基準
            hitPointB = circleData1.position - direction * circleData1.radius; // circleData1基準
            return true;
        }
        hitPointA = hitPointB = default;
        return false;
    }

    public static bool CheckCollision3D(this CircleData3D circleData, Vector3 point, out Vector3 hitPoint)
    {

        //Debug.Log(point);
        float distance = GetDistance3D(circleData.position, point);
        if (distance - circleData.radius <= 0)
        {
            hitPoint = point;
            return true;
        }
        hitPoint = default;
        return false;
    }

    public static bool CheckCollision3D(this CircleData3D circleData, LineData3D lineData, out Vector3 hitPointA, out Vector2 hitPointB)
    {

        Vector3 point = GetNearestPoint3D(lineData.originPoint, lineData.endPoint, circleData.position);
        float distance = GetDistance3D(circleData.position, point);
        if (distance - circleData.radius <= 0)
        {
            Vector3 direction = (point - circleData.position).normalized;
            hitPointA = circleData.position + direction * circleData.radius; // 円の接触点
            hitPointB = point; // 線分の接触点
            return true;
        }
        hitPointA = hitPointB = default;
        return false;
    }

    public static bool CheckCollision3D(this CircleData3D circleData, CapsuleData3D capsuleData, out Vector3 hitPointA, out Vector3 hitPointB)
    {

        Vector3 point = GetNearestPoint3D(capsuleData.originPoint, capsuleData.endPoint, circleData.position);
        float distance = GetDistance3D(circleData.position, point);
        if (distance - (circleData.radius + capsuleData.radius) <= 0)
        {
            Vector3 direction = (point - circleData.position).normalized;
            hitPointA = circleData.position + direction * circleData.radius; // 円の接触点
            hitPointB = point - direction * capsuleData.radius; // カプセルの接触点
            return true;
        }
        hitPointA = hitPointB = default;
        return false;
    }
    /// <summary>
    /// box
    /// </summary>
    /// <param name="circleData"></param>
    /// <param name="box"></param>
    /// <returns></returns>
    public static bool CheckCollision3D(this CircleData3D circle, BoxData3D box, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        hitPointA = hitPointB = default;

        float minDistance = float.MaxValue;
        Vector3 closestPoint = default;

        // ボックスの各エッジと円の最近傍点を計算
        foreach (Vector3[] edge in box.GetEdges())
        {
            if (CheckLineOut(edge[0], edge[1], circle.position, out Vector3 nearestPoint))
            {
                float distance = GetDistance3D(nearestPoint, circle.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = nearestPoint;
                }
            }
        }

        // 最近傍点が円の半径以内でなければ接触なし
        if (minDistance > circle.radius)
        {
            return false;
        }


        hitPointB = closestPoint; // ボックス基準の接触点

        // 円周基準の接触点を計算
        Vector3 direction = (closestPoint - circle.position).normalized;
        hitPointA = circle.position + direction * circle.radius;

        return true;
    }
    #endregion
    #region line
    /// <summary>
    /// 線分と任意の衝突対象との衝突判定を行い、双方向の接触点を返します。
    /// </summary>
    /// <param name="lineData">線分データ</param>
    /// <param name="collisionData">衝突対象データ</param>
    /// <param name="hitPointA">線分基準の接触点</param>
    /// <param name="hitPointB">衝突対象基準の接触点</param>
    /// <returns>衝突しているかどうか</returns>
    public static bool CheckCollision3D(this LineData3D lineData, IBaseCollisionData3D collisionData, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        switch (collisionData)
        {
            case CircleData3D circle:
                {
                    return lineData.CheckCollision3D(circle, out hitPointA, out hitPointB);
                }
            case CapsuleData3D capsule:
                {
                    return lineData.CheckCollision3D(capsule, out hitPointA, out hitPointB);
                }
            case LineData3D line:
                {
                    return lineData.CheckCollision3D(line, out hitPointA, out hitPointB);
                }
            case BoxData3D box:
                {
                    return lineData.CheckCollision3D(box, out hitPointA, out hitPointB);
                }
        }

        hitPointA = default;
        hitPointB = default;
        return false;
    }
    /// <summary>
    /// 線分と点の衝突判定を行い、双方向の接触点を返します。
    /// </summary>
    /// <param name="lineData">線分データ</param>
    /// <param name="position">点の座標</param>
    /// <param name="hitPointA">線分基準の接触点</param>
    /// <param name="hitPointB">点基準の接触点</param>
    /// <returns>衝突しているかどうか</returns>
    public static bool CheckCollision3D(this LineData3D lineData, Vector3 position, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        Vector3 point = GetNearestPoint3D(lineData.originPoint, lineData.endPoint, position);

        float distance = GetDistance3D(position, point);
        if (distance <= 0)
        {
            hitPointA = point;
            hitPointB = position;
            return true;
        }

        hitPointA = default;
        hitPointB = default;
        return false;
    }
    /// <summary>
    /// 線分と円の衝突判定を行い、双方向の接触点を返します。
    /// </summary>
    /// <param name="lineData">線分データ</param>
    /// <param name="circleData">円データ</param>
    /// <param name="hitPointA">線分基準の接触点</param>
    /// <param name="hitPointB">円基準の接触点</param>
    /// <returns>衝突しているかどうか</returns>
    public static bool CheckCollision3D(this LineData3D lineData, CircleData3D circleData, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        Vector3 point = GetNearestPoint3D(lineData.originPoint, lineData.endPoint, circleData.position);
        float distance = GetDistance3D(circleData.position, point);

        if (distance <= circleData.radius)
        {
            Vector3 direction = (point - circleData.position).normalized;
            hitPointA = point;
            hitPointB = circleData.position + direction * circleData.radius;
            return true;
        }

        hitPointA = default;
        hitPointB = default;
        return false;
    }
    /// <summary>
    /// 線分とカプセルの衝突判定を行い、双方向の接触点を返します。
    /// </summary>
    /// <param name="lineData">線分データ</param>
    /// <param name="capsuleData">カプセルデータ</param>
    /// <param name="hitPointA">線分基準の接触点</param>
    /// <param name="hitPointB">カプセル基準の接触点</param>
    /// <returns>衝突しているかどうか</returns>
    public static bool CheckCollision3D(this LineData3D lineData, CapsuleData3D capsuleData, out Vector2 hitPointA, out Vector2 hitPointB)
    {

        float distance = GetLinesDistance3D(lineData, capsuleData.ToLine());
        if (distance - capsuleData.radius <= 0)
        {
            hitPointA = GetNearestPoint3D(lineData.originPoint, lineData.endPoint, capsuleData.ToLine().originPoint);
            hitPointB = capsuleData.ToLine().originPoint;
            return true;
        }
        hitPointA = hitPointB = default;
        return false;
    }

    /// <summary>
    /// 2つの線分の衝突判定を行い、双方向の接触点を返します。
    /// </summary>
    /// <param name="lineData1">1つ目の線分</param>
    /// <param name="lineData2">2つ目の線分</param>
    /// <param name="hitPointA">線分1基準の接触点</param>
    /// <param name="hitPointB">線分2基準の接触点</param>
    /// <returns>衝突しているかどうか</returns>
    public static bool CheckCollision3D(this LineData3D lineData1, LineData3D lineData2, out Vector3 hitPointA, out Vector3 hitPointB)
    {

        float distance = GetLinesDistance3D(lineData1, lineData2);
        if (distance <= 0)
        {
            hitPointA = lineData1.originPoint; // 仮の値
            hitPointB = lineData2.originPoint; // 仮の値
            return true;
        }
        hitPointB = hitPointA = default;
        return false;
    }
    /// <summary>
    /// 線分とボックスの衝突判定を行い、双方向の接触点を返します。
    /// </summary>
    /// <param name="lineData">線分データ</param>
    /// <param name="box">ボックスデータ</param>
    /// <param name="hitPointA">線分基準の接触点</param>
    /// <param name="hitPointB">ボックス基準の接触点</param>
    /// <returns>衝突しているかどうか</returns>
    public static bool CheckCollision3D(this LineData3D lineData, BoxData3D box, out Vector3 hitPointA, out Vector3 hitPointB)
    {

        Vector2 nearestPoint = GetNearestPoint3D(box.ToLine(), lineData);

        if (GetNearestPointOnLine(box, nearestPoint, out Vector3 point))
        {
            hitPointA = hitPointB = default;
            return false;
        }

        if (GetDistance3D(point, nearestPoint) <= box.boxWidth)
        {
            hitPointA = nearestPoint;
            hitPointB = point;
            return true;
        }

        hitPointA= hitPointB = default;
        return false;
    }
    #endregion
    #region box

    /// <summary>
    /// ボックスと任意の衝突対象との衝突判定を行い、接触点を返します。
    /// </summary>
    /// <param name="boxData">ボックスデータ</param>
    /// <param name="collisionData">衝突対象データ</param>
    /// <param name="hitPointA">ボックス基準の接触点</param>
    /// <param name="hitPointB">衝突対象基準の接触点</param>
    /// <returns>衝突しているかどうか</returns>
    public static bool CheckCollision3D(this BoxData3D boxData, IBaseCollisionData3D collisionData,out Vector3 hitPointA,out Vector3 hitPointB)
    {
        switch (collisionData)
        {
            case CircleData3D circle:
                return boxData.CheckCollision3D(circle, out hitPointA, out hitPointB);
            case CapsuleData3D capsule:
                return boxData.CheckCollision3D(capsule, out hitPointA, out hitPointB);
            case LineData3D line:
                return boxData.CheckCollision3D(line, out hitPointA, out hitPointB);
            case BoxData3D box:
                return boxData.CheckCollision3D(box, out hitPointA, out hitPointB);
        }
        hitPointA = hitPointB = default;
        return false; // 未対応の型の場合
    }
  /// <summary>
  /// 
  /// </summary>
  /// <param name="box"></param>
  /// <param name="position"></param>
  /// <param name="hitPointA"></param>
  /// <param name="hitPointB"></param>
  /// <returns></returns>
    public static bool CheckCollision3D(this BoxData3D box, Vector3 position, out Vector3 hitPointA, out Vector3 hitPointB)
    {

        hitPointA = hitPointB = default;

        // 最近傍点をボックスの線分上で取得
        if (!box.GetNearestPointOnLine(position, out Vector3 nearestPoint))
        {
            return false;
        }


        // 最近傍点と点の距離がボックスの幅を超える場合は衝突なし
        if (GetDistance3D(nearestPoint, position) > box.boxWidth / 2)
        {
            return false;
        }


        hitPointA = nearestPoint;  // ボックス基準の接触点
        hitPointB = position;      // 点基準の接触点
        return true;
    }
  /// <summary>
  /// 
  /// </summary>
  /// <param name="box"></param>
  /// <param name="line"></param>
  /// <param name="hitPointA"></param>
  /// <param name="hitPointB"></param>
  /// <returns></returns>
    public static bool CheckCollision3D(this BoxData3D box, LineData3D line,out Vector3 hitPointA,out Vector3 hitPointB)
    {
        hitPointA = hitPointB = default;

        // ボックスと線分の最近傍点を取得
        Vector2 nearestPoint = GetNearestPoint3D(box.ToLine(), line);

        if (!box.GetNearestPointOnLine(nearestPoint, out Vector3 point))
        {
            return false;
        }


        // 最近傍点と線分の距離がボックスの幅を超える場合は衝突なし
        if (GetDistance3D(point, nearestPoint) > box.boxWidth / 2)
        {
            return false;
        }


        hitPointA = point;        // ボックス基準の接触点
        hitPointB = nearestPoint; // 線分基準の接触点
        return true;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="box"></param>
    /// <param name="circle"></param>
    /// <param name="hitPointA"></param>
    /// <param name="hitPointB"></param>
    /// <returns></returns>
    public static bool CheckCollision3D(this BoxData3D box, CircleData3D circle, out Vector3 hitPointA, out Vector3 hitPointB)
    {

        hitPointA = hitPointB = default;

        float minDistance = float.MaxValue;
        Vector3 closestPoint = default;

        // ボックスの各エッジと円の最近傍点を計算
        foreach (Vector3[] edge in box.GetEdges())
        {
            if (CheckLineOut(edge[0], edge[1], circle.position, out Vector3 nearestPoint))
            {
                float distance = GetDistance3D(nearestPoint, circle.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = nearestPoint;
                }
            }
        }
        // 最近傍点が円の半径以内でなければ接触なし
        if (minDistance > circle.radius)
            return false;

        hitPointA = closestPoint; // ボックス基準の接触点

        // 円周基準の接触点を計算
        Vector3 direction = (closestPoint - circle.position).normalized;
        hitPointB = circle.position + direction * circle.radius;
        Debug.Log(hitPointA + " " + hitPointB);
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="box"></param>
    /// <param name="capsule"></param>
    /// <param name="hitPointA"></param>
    /// <param name="hitPointB"></param>
    /// <returns></returns>
    public static bool CheckCollision3D(this BoxData3D box, CapsuleData3D capsule,out Vector3 hitPointA,out Vector3 hitPointB)
    {

        hitPointA = hitPointB = default;

        // ボックスのエッジリストを取得
        Vector3[][] edges = box.GetEdges();

        float minDistance = float.MaxValue;
        Vector3 closestCapsulePoint = default;
        Vector3 closestBoxPoint = default;

        // ボックスの各エッジについて判定
        foreach (Vector3[] edge in edges)
        {
            Vector3 edgeStart = edge[0];
            Vector3 edgeEnd = edge[1];

            // カプセルの線分とボックスのエッジ間の最近点を計算
            Vector3 capsuleNearestPoint = GetNearestPoint3D(capsule.originPoint, capsule.endPoint, edgeStart);
            Vector3 edgeNearestPoint = GetNearestPoint3D(edgeStart, edgeEnd, capsuleNearestPoint);

            // 最近点間の距離を計算
            float distance = GetDistance3D(capsuleNearestPoint, edgeNearestPoint);

            // 最短距離の更新
            if (distance < minDistance)
            {
                minDistance = distance;
                closestCapsulePoint = capsuleNearestPoint;
                closestBoxPoint = edgeNearestPoint;
            }
        }

        // 衝突判定: 最短距離がカプセルの半径以下か
        if (minDistance > capsule.radius)
        {
            return false;
        }
        // 接触点を設定
        Vector3 direction = (closestBoxPoint - closestCapsulePoint).normalized;
        hitPointB = closestCapsulePoint + direction * capsule.radius; // カプセル基準の接触点
        hitPointA = closestBoxPoint; // ボックス基準の接触点
        return true;
    }
    
    public static bool CheckCollision3D(this BoxData3D box1, BoxData3D box2,out Vector3 hitPointA,out Vector3 hitPointB)
    {
        hitPointA = hitPointB = default;

        // 最近傍点を取得
        Vector2 nearestPoint = GetNearestPoint3D(box1.ToLine(), box2.ToLine());

        if (!box1.GetNearestPointOnLine(nearestPoint, out Vector3 point))
            return false;

        // 最近傍点とボックスの距離がそれぞれの幅の合計を超える場合は衝突なし
        if (GetDistance3D(point, nearestPoint) > (box1.boxWidth + box2.boxWidth) / 2)
            return false;

        hitPointA = point;        // ボックス1基準の接触点
        hitPointB = nearestPoint; // ボックス2基準の接触点
        return true;
    }


    
    /// 点がラインの外か判別
    /// </summary>
    /// <param name="box"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private static bool CheckLineOut(Vector3 origin, Vector3 end, Vector3 position, out Vector3 nearestPoint)
    {
        // 線分の方向ベクトル
        Vector3 direction = (end - origin);
        float lineLength = direction.magnitude;   // 線分の長さ
        direction.Normalize();                   // 単位ベクトル化

        // 線分の開始点から対象点へのベクトル
        Vector3 originToPoint = position - origin;

        // 線分上の最近傍点の位置をスカラー値で計算
        float projection = Vector3.Dot(originToPoint, direction);

        // 線分の範囲外かチェック
        if (projection < 0)
        {
            // 最近傍点は origin 側の端点
            nearestPoint = origin;
            return false; // 範囲外
        }
        else if (projection > lineLength)
        {
            // 最近傍点は end 側の端点
            nearestPoint = end;
            return false; // 範囲外
        }

        // 線分上の最近傍点を計算
        nearestPoint = origin + direction * projection;

        return true; // 線分上に最近傍点が存在する
    }

    /// <summary>
    /// 点がボックスの線分の外にあるか判別し、最近傍点を計算します。
    /// </summary>
    private static bool GetNearestPointOnLine(this BoxData3D box, Vector3 position, out Vector3 nearestPoint)
    {
        Vector3 direction = (box.endPoint - box.originPoint).normalized;
        Vector3 originToPoint = position - box.originPoint;
        Vector3 endToPoint = position - box.endPoint;

        if (Vector3.Dot(direction, originToPoint) < 0 || Vector3.Dot(direction, endToPoint) > 0)
        {
            nearestPoint = default;
            return false;
        }

        nearestPoint = box.originPoint + direction * Vector3.Dot(direction, originToPoint);
        return true;
    }
    #endregion
}
