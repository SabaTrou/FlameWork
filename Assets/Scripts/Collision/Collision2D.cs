using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;


public static class CapsuleCollision2D
{
    
    #region 計算
    private static Vector2 GetNearestPoint2D(Vector2 origin, Vector2 end, Vector2 position)
    {
        //カプセルの始点から終点のベクトルを正規化したもの
        Vector2 vector1 = (end - origin).normalized;
        //始点から点へのベクトル
        Vector2 originToPoint = position - origin;
        //終点から点へのベクトル
        Vector2 endToPoint = position - end;

        if (0 > Vector2.Dot(vector1, originToPoint))
        {

            return origin;
        }
        else if (0 < Vector2.Dot(vector1, endToPoint))
        {
            return end;
        }

        Vector2 point = origin + vector1 * Vector2.Dot(vector1, originToPoint);

        return point;
    }
    private static Vector2 GetNearestPoint2D(LineData2D lineData1, LineData2D lineData2)
    {
        Vector2 originNearPoint = GetNearestPoint2D(lineData1.originPoint, lineData1.endPoint, lineData2.originPoint);
        float originDistance = GetDistance2D(originNearPoint, lineData2.originPoint);
        Vector2 endNearPoint = GetNearestPoint2D(lineData1.originPoint, lineData1.endPoint, lineData2.endPoint);
        float endDistance = GetDistance2D(endNearPoint, lineData2.endPoint);
        if (originDistance <= endDistance)
        {
            return originNearPoint;
        }
        return endNearPoint;
    }


    private static float GetDistance2D(Vector2 point1, Vector2 point2)
    {

        Vector2 distanceVec = point1 - point2;
        float distance = Mathf.Sqrt(
            Mathf.Pow(Mathf.Abs(distanceVec.x), 2) +
            Mathf.Pow(Mathf.Abs(distanceVec.y), 2)
            );
        return distance;
    }
    private static float GetLinesDistance2D(LineData2D lineData1, LineData2D lineData2)
    {
        Vector2 originNearPoint = GetNearestPoint2D(lineData1.originPoint, lineData1.endPoint, lineData2.originPoint);
        float originDistance = GetDistance2D(originNearPoint, lineData2.originPoint);
        Vector2 endNearPoint = GetNearestPoint2D(lineData1.originPoint, lineData1.endPoint, lineData2.endPoint);
        float endDistance = GetDistance2D(endNearPoint, lineData2.endPoint);
        if (originDistance <= endDistance)
        {
            return originDistance;
        }
        return endDistance;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector2 GetVector(Vector2 point, Vector2 target)
    {
        return target - point;
    }
    #endregion
    #region capsule
    public static bool CheckCollision2D(this CapsuleData2D capsuleData, IBaseCollisionData2D collisionData, out Vector2 hitPointA, out Vector2 hitPointB)
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
        hitPointA = default(Vector2);
        hitPointB = default(Vector2);
        return false;
    }
    /// <summary>
    /// point
    /// </summary>
    /// <param name="capsuleData"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static bool CheckCollision2D(this CapsuleData2D capsuleData, Vector2 position, out Vector2 hitPoint)
    {
        Vector2 point = GetNearestPoint2D(capsuleData.originPoint, capsuleData.endPoint, position);
        //Debug.Log(point);
        float distance = GetDistance2D(position, point);

        if (distance - capsuleData.radius <= 0)
        {
            hitPoint = point;
            return true;
        }
        hitPoint = default;
        return false;
    }
    /// <summary>
    /// カプセルと円の2D衝突判定を行い、両側の接触点を計算します。
    /// </summary>
    /// <param name="capsuleData">カプセルデータ</param>
    /// <param name="circleData">円データ</param>
    /// <param name="hitPointA">カプセル基準の接触点</param>
    /// <param name="hitPointB">円基準の接触点</param>
    /// <returns>衝突している場合はtrue</returns>
    public static bool CheckCollision2D(this CapsuleData2D capsuleData, CircleData2D circleData, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        // カプセル線分上の最近点を取得
        Vector2 nearestPoint = GetNearestPoint2D(capsuleData.originPoint, capsuleData.endPoint, circleData.position);

        // 円の中心と最近点のベクトルおよび距離を計算
        Vector2 direction = circleData.position - nearestPoint;
        float distance = direction.magnitude;

        // 衝突判定: 距離が両オブジェクトの半径の合計以下か
        if (distance < capsuleData.radius + circleData.radius)
        {
            // カプセル基準の接触点
            hitPointA = nearestPoint + direction.normalized * capsuleData.radius;
            // 円基準の接触点
            hitPointB = circleData.position - direction.normalized * circleData.radius;
            return true;
        }

        // 衝突していない場合はデフォルト値を返す
        hitPointA = hitPointB = default;
        return false;
    }
    /// <summary>
    /// カプセルと線分の2D衝突判定を行い、両側の接触点を計算します。
    /// </summary>
    /// <param name="capsuleData">カプセルデータ</param>
    /// <param name="lineData">線分データ</param>
    /// <param name="hitPointA">カプセル基準の接触点</param>
    /// <param name="hitPointB">線分基準の接触点</param>
    /// <returns>衝突している場合はtrue</returns>
    public static bool CheckCollision2D(this CapsuleData2D capsuleData, LineData2D lineData, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        // 線分上の最近点を計算
        Vector2 lineNearestPoint = GetNearestPoint2D(lineData.originPoint, lineData.endPoint, capsuleData.originPoint);

        // カプセルの中心線上の最近点を計算
        Vector2 capsuleNearestPoint = GetNearestPoint2D(capsuleData.originPoint, capsuleData.endPoint, lineData.originPoint);

        // 最近点間の距離を計算
        float distance = Vector2.Distance(lineNearestPoint, capsuleNearestPoint);

        // 衝突判定: 距離がカプセルの半径以下なら衝突
        if (distance <= capsuleData.radius)
        {
            // 接触点を計算
            hitPointA = capsuleNearestPoint;  // カプセル基準の接触点
            hitPointB = lineNearestPoint;    // 線分基準の接触点
            return true;
        }

        // 衝突していない場合はデフォルト値を返す
        hitPointA = hitPointB = default;
        return false;
    }

    /// <summary>
    /// 2つのカプセルの2D衝突判定を行い、両側の接触点を計算します。
    /// </summary>
    /// <param name="capsuleData1">1つ目のカプセルデータ</param>
    /// <param name="capsuleData2">2つ目のカプセルデータ</param>
    /// <param name="hitPointA">1つ目のカプセル基準の接触点</param>
    /// <param name="hitPointB">2つ目のカプセル基準の接触点</param>
    /// <returns>衝突している場合はtrue</returns>
    public static bool CheckCollision2D(this CapsuleData2D capsuleData1, CapsuleData2D capsuleData2, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        // 両カプセルの線分上の最近点を取得
        Vector2 nearestPoint1 = GetNearestPoint2D(capsuleData1.originPoint, capsuleData1.endPoint, capsuleData2.originPoint);
        Vector2 nearestPoint2 = GetNearestPoint2D(capsuleData2.originPoint, capsuleData2.endPoint, capsuleData1.originPoint);

        // 最近点間の距離を計算
        Vector2 direction = nearestPoint2 - nearestPoint1;
        float distance = direction.magnitude;

        // 衝突判定: 距離が半径の合計以下か
        if (distance < (capsuleData1.radius + capsuleData2.radius))
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
    public static bool CheckCollision2D(this CapsuleData2D capsule, BoxData2D box, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        hitPointA = hitPointB = default;

        // ボックスのエッジリストを取得
        Vector2[][] edges = box.GetEdges();

        float minDistance = float.MaxValue;
        Vector2 closestCapsulePoint = default;
        Vector2 closestBoxPoint = default;

        // ボックスの各エッジについて判定
        foreach (Vector2[] edge in edges)
        {
            Vector2 edgeStart = edge[0];
            Vector2 edgeEnd = edge[1];

            // カプセルの線分とボックスのエッジ間の最近点を計算
            Vector2 capsuleNearestPoint = GetNearestPoint2D(capsule.originPoint, capsule.endPoint, edgeStart);
            Vector2 edgeNearestPoint = GetNearestPoint2D(edgeStart, edgeEnd, capsuleNearestPoint);

            // 最近点間の距離を計算
            float distance = GetDistance2D(capsuleNearestPoint, edgeNearestPoint);

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
            Vector2 direction = (closestBoxPoint - closestCapsulePoint).normalized;
            hitPointA = closestCapsulePoint + direction * capsule.radius; // カプセル基準の接触点
            hitPointB = closestBoxPoint; // ボックス基準の接触点
            return true;
        }

        return false;
    }
    #endregion
    #region circle

    /// <summary>
    /// CircleData2D同士の衝突判定。両方のオブジェクト基準での接触点を計算します。
    /// </summary>
    public static bool CheckCollision2D(this CircleData2D circleData, CircleData2D circleData1, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        // 2つの円の中心間の距離を計算
        float distance = GetDistance2D(circleData.position, circleData1.position);

        // 衝突判定: 距離が半径の合計以下か
        if (distance <= circleData.radius + circleData1.radius)
        {
            // それぞれの接触点を計算
            Vector2 direction = (circleData1.position - circleData.position).normalized;
            hitPointA = circleData.position + direction * circleData.radius; // circleData基準
            hitPointB = circleData1.position - direction * circleData1.radius; // circleData1基準
            return true;
        }

        // 衝突がない場合はデフォルト値
        hitPointA = hitPointB = default;
        return false;
    }

    /// <summary>
    /// CircleData2Dと任意のオブジェクトの衝突判定。
    /// </summary>
    public static bool CheckCollision2D(this CircleData2D circleData, IBaseCollisionData2D collisionData, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        switch (collisionData)
        {
            case CircleData2D circle:
                return circleData.CheckCollision2D(circle, out hitPointA, out hitPointB);
            case CapsuleData2D capsule:
                return circleData.CheckCollision2D(capsule, out hitPointA, out hitPointB);
            case LineData2D line:
                return circleData.CheckCollision2D(line, out hitPointA, out hitPointB);
            case BoxData2D box:
                return circleData.CheckCollision2D(box, out hitPointA, out hitPointB);
        }

        // 対応していない型の場合は衝突なしとみなす
        hitPointA = hitPointB = default;
        return false;
    }

    /// <summary>
    /// CircleData2DとLineData2Dの衝突判定。接触点を計算します。
    /// </summary>
    public static bool CheckCollision2D(this CircleData2D circleData, LineData2D lineData, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        // 線分上の円に最も近い点を計算
        Vector2 nearestPoint = GetNearestPoint2D(lineData.originPoint, lineData.endPoint, circleData.position);
        float distance = GetDistance2D(circleData.position, nearestPoint);

        // 衝突判定: 最近点が円の半径以内にあるか
        if (distance <= circleData.radius)
        {
            Vector2 direction = (nearestPoint - circleData.position).normalized;
            hitPointA = circleData.position + direction * circleData.radius; // 円の接触点
            hitPointB = nearestPoint; // 線分の接触点
            return true;
        }

        // 衝突がない場合はデフォルト値
        hitPointA = hitPointB = default;
        return false;
    }

    /// <summary>
    /// CircleData2DとCapsuleData2Dの衝突判定。接触点を計算します。
    /// </summary>
    public static bool CheckCollision2D(this CircleData2D circleData, CapsuleData2D capsuleData, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        // カプセルの線分上の最近点を取得
        Vector2 nearestPoint = GetNearestPoint2D(capsuleData.originPoint, capsuleData.endPoint, circleData.position);
        float distance = GetDistance2D(circleData.position, nearestPoint);

        // 衝突判定: 最近点が円とカプセルの半径の合計以内か
        if (distance <= circleData.radius + capsuleData.radius)
        {
            Vector2 direction = (nearestPoint - circleData.position).normalized;
            hitPointA = circleData.position + direction * circleData.radius; // 円の接触点
            hitPointB = nearestPoint - direction * capsuleData.radius; // カプセルの接触点
            return true;
        }

        // 衝突がない場合はデフォルト値
        hitPointA = hitPointB = default;
        return false;
    }

    /// <summary>
    /// CircleData2DとBoxData2Dの衝突判定。接触点を計算します。
    /// </summary>
    public static bool CheckCollision2D(this CircleData2D circle, BoxData2D box, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        hitPointA = hitPointB = default;

        float minDistance = float.MaxValue;
        Vector2 closestPoint = default;

        // ボックスの各エッジと円の最近傍点を計算
        foreach (Vector2[] edge in box.GetEdges())
        {
            if (CheckLineOut(edge[0], edge[1], circle.position, out Vector2 nearestPoint))
            {
                float distance = GetDistance2D(nearestPoint, circle.position);
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

        hitPointB = closestPoint; // ボックス基準の接触点

        // 円周基準の接触点を計算
        Vector2 direction = (closestPoint - circle.position).normalized;
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
    public static bool CheckCollision2D(this LineData2D lineData, IBaseCollisionData2D collisionData, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        switch (collisionData)
        {
            case CircleData2D circle:
                {
                    return lineData.CheckCollision2D(circle, out hitPointA, out hitPointB);
                }
            case CapsuleData2D capsule:
                {
                    return lineData.CheckCollision2D(capsule, out hitPointA, out hitPointB);
                }
            case LineData2D line:
                { 
                    return lineData.CheckCollision2D(line, out hitPointA, out hitPointB); 
                }
            case BoxData2D box:
                {
                    return lineData.CheckCollision2D(box, out hitPointA, out hitPointB);
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
    public static bool CheckCollision2D(this LineData2D lineData, Vector2 position, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        Vector2 nearestPoint = GetNearestPoint2D(lineData.originPoint, lineData.endPoint, position);
        float distance = GetDistance2D(position, nearestPoint);

        if (distance <= 0)
        {
            hitPointA = nearestPoint;
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
    public static bool CheckCollision2D(this LineData2D lineData, CircleData2D circleData, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        hitPointA = GetNearestPoint2D(lineData.originPoint, lineData.endPoint, circleData.position);
        float distance = GetDistance2D(circleData.position, hitPointA);

        if (distance <= circleData.radius)
        {
            Vector2 direction = (hitPointA - circleData.position).normalized;
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
    public static bool CheckCollision2D(this LineData2D lineData, CapsuleData2D capsuleData, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        float distance = GetLinesDistance2D(lineData, capsuleData.ToLine());

        if (distance <= capsuleData.radius)
        {
            hitPointA = GetNearestPoint2D(lineData.originPoint, lineData.endPoint, capsuleData.ToLine().originPoint);
            hitPointB = capsuleData.ToLine().originPoint;
            return true;
        }

        hitPointA = default;
        hitPointB = default;
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
    public static bool CheckCollision2D(this LineData2D lineData1, LineData2D lineData2, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        float distance = GetLinesDistance2D(lineData1, lineData2);

        if (distance <= 0)
        {
            hitPointA = lineData1.originPoint; // 仮の値
            hitPointB = lineData2.originPoint; // 仮の値
            return true;
        }

        hitPointA = default;
        hitPointB = default;
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
    public static bool CheckCollision2D(this LineData2D lineData, BoxData2D box, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        Vector2 nearestPoint = GetNearestPoint2D(box.ToLine(), lineData);

        if (GetNearestPointOnLine(box, nearestPoint, out Vector2 point))
        {
            hitPointA = default;
            hitPointB = default;
            return false;
        }

        if (GetDistance2D(point, nearestPoint) <= box.boxWidth)
        {
            hitPointA = nearestPoint;
            hitPointB = point;
            return true;
        }

        hitPointA = default;
        hitPointB = default;
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
    public static bool CheckCollision2D(this BoxData2D boxData, IBaseCollisionData2D collisionData, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        switch (collisionData)
        {
            case CircleData2D circle:
                return boxData.CheckCollision2D(circle, out hitPointA, out hitPointB);
            case CapsuleData2D capsule:
                return boxData.CheckCollision2D(capsule, out hitPointA, out hitPointB);
            case LineData2D line:
                return boxData.CheckCollision2D(line, out hitPointA, out hitPointB);
            case BoxData2D box:
                return boxData.CheckCollision2D(box, out hitPointA, out hitPointB);
        }
        hitPointA = hitPointB = default;
        return false; // 未対応の型の場合
    }

    /// <summary>
    /// ボックスと点の衝突判定を行い、接触点を返します。
    /// </summary>
    public static bool CheckCollision2D(this BoxData2D box, Vector2 position, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        hitPointA = hitPointB = default;

        // 最近傍点をボックスの線分上で取得
        if (!box.GetNearestPointOnLine(position, out Vector2 nearestPoint))
        {
            return false;
        }
            

        // 最近傍点と点の距離がボックスの幅を超える場合は衝突なし
        if (GetDistance2D(nearestPoint, position) > box.boxWidth / 2)
        {
            return false;
        }
            

        hitPointA = nearestPoint;  // ボックス基準の接触点
        hitPointB = position;      // 点基準の接触点
        return true;
    }

    /// <summary>
    /// ボックスと線分の衝突判定を行い、接触点を返します。
    /// 未検証
    /// </summary>
    public static bool CheckCollision2D(this BoxData2D box, LineData2D line, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        hitPointA = hitPointB = default;

        // ボックスと線分の最近傍点を取得
        Vector2 nearestPoint = GetNearestPoint2D(box.ToLine(), line);

        if (!box.GetNearestPointOnLine(nearestPoint, out Vector2 point))
        {
            return false;
        }
            

        // 最近傍点と線分の距離がボックスの幅を超える場合は衝突なし
        if (GetDistance2D(point, nearestPoint) > box.boxWidth / 2)
        {
            return false;
        }
            

        hitPointA = point;        // ボックス基準の接触点
        hitPointB = nearestPoint; // 線分基準の接触点
        return true;
    }

    /// <summary>
    /// ボックスと円の衝突判定を行い、接触点を返します。
    /// 検証済み
    /// </summary>
    public static bool CheckCollision2D(this BoxData2D box, CircleData2D circle, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        hitPointA = hitPointB = default;

        float minDistance = float.MaxValue;
        Vector2 closestPoint = default;

        // ボックスの各エッジと円の最近傍点を計算
        foreach (Vector2[] edge in box.GetEdges())
        {
            if (CheckLineOut(edge[0], edge[1], circle.position, out Vector2 nearestPoint))
            {
                float distance = GetDistance2D(nearestPoint, circle.position);
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
        Vector2 direction = (closestPoint - circle.position).normalized;
        hitPointB = circle.position + direction * circle.radius;
        Debug.Log(hitPointA + " " + hitPointB);
        return true;
    }

    /// <summary>
    /// カプセルとボックスの2D衝突判定を行い、両側の接触点を計算します。
    /// 未検証
    /// </summary>
    /// <param name="capsule">カプセルデータ</param>
    /// <param name="box">ボックスデータ</param>
    /// <param name="hitPointA">カプセル基準の接触点</param>
    /// <param name="hitPointB">ボックス基準の接触点</param>
    ///
    /// <returns>衝突している場合はtrue</returns>
    public static bool CheckCollision2D(this BoxData2D box, CapsuleData2D capsule, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        hitPointA = hitPointB = default;

        // ボックスのエッジリストを取得
        Vector2[][] edges = box.GetEdges();

        float minDistance = float.MaxValue;
        Vector2 closestCapsulePoint = default;
        Vector2 closestBoxPoint = default;

        // ボックスの各エッジについて判定
        foreach (Vector2[] edge in edges)
        {
            Vector2 edgeStart = edge[0];
            Vector2 edgeEnd = edge[1];

            // カプセルの線分とボックスのエッジ間の最近点を計算
            Vector2 capsuleNearestPoint = GetNearestPoint2D(capsule.originPoint, capsule.endPoint, edgeStart);
            Vector2 edgeNearestPoint = GetNearestPoint2D(edgeStart, edgeEnd, capsuleNearestPoint);

            // 最近点間の距離を計算
            float distance = GetDistance2D(capsuleNearestPoint, edgeNearestPoint);

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
        Vector2 direction = (closestBoxPoint - closestCapsulePoint).normalized;
        hitPointB = closestCapsulePoint + direction * capsule.radius; // カプセル基準の接触点
        hitPointA = closestBoxPoint; // ボックス基準の接触点
        return true;
        
    }

    /// <summary>
    /// ボックス同士の衝突判定を行い、接触点を返します。
    /// 未検証
    /// </summary>
    public static bool CheckCollision2D(this BoxData2D box1, BoxData2D box2, out Vector2 hitPointA, out Vector2 hitPointB)
    {
        hitPointA = hitPointB = default;

        // 最近傍点を取得
        Vector2 nearestPoint = GetNearestPoint2D(box1.ToLine(), box2.ToLine());

        if (!box1.GetNearestPointOnLine(nearestPoint, out Vector2 point))
            return false;

        // 最近傍点とボックスの距離がそれぞれの幅の合計を超える場合は衝突なし
        if (GetDistance2D(point, nearestPoint) > (box1.boxWidth + box2.boxWidth) / 2)
            return false;

        hitPointA = point;        // ボックス1基準の接触点
        hitPointB = nearestPoint; // ボックス2基準の接触点
        return true;
    }

    /// <summary>
    /// 点がボックスの線分の外にあるか判別し、最近傍点を計算します。
    /// </summary>
    private static bool GetNearestPointOnLine(this BoxData2D box, Vector2 position, out Vector2 nearestPoint)
    {
        Vector2 direction = (box.endPoint - box.originPoint).normalized;
        Vector2 originToPoint = position - box.originPoint;
        Vector2 endToPoint = position - box.endPoint;

        if (Vector2.Dot(direction, originToPoint) < 0 || Vector2.Dot(direction, endToPoint) > 0)
        {
            nearestPoint = default;
            return false;
        }

        nearestPoint = box.originPoint + direction * Vector2.Dot(direction, originToPoint);
        return true;
    }

   
    private static bool CheckLineOut(Vector2 origin,Vector2 end, Vector2 position, out Vector2 nearestPoint)
    {
        // 線分の方向ベクトル
        Vector2 direction = (end - origin);
        float lineLength = direction.magnitude;   // 線分の長さ
        direction.Normalize();                   // 単位ベクトル化

        // 線分の開始点から対象点へのベクトル
        Vector2 originToPoint = position - origin;

        // 線分上の最近傍点の位置をスカラー値で計算
        float projection = Vector2.Dot(originToPoint, direction);

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
    #endregion


    //vector3のxzをvector2に変換する
    public static Vector2 ToVector2XZ(this Vector3 vector3)
    {

        return new Vector2(vector3.x, vector3.z);

    }


}
