using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Collision3D
{
    #region �v�Z
    private static Vector3 GetNearestPoint3D(Vector3 origin, Vector3 end, Vector3 position)
    {
        //�J�v�Z���̎n�_����I�_�̃x�N�g���𐳋K����������
        Vector3 vector1 = (end - origin).normalized;
        //�n�_����_�ւ̃x�N�g��
        Vector3 originToPoint = position - origin;
        //�I�_����_�ւ̃x�N�g��
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
    /// �J�v�Z���Ɖ~��3D�Փ˔�����s���A�����̐ڐG�_���v�Z���܂��B
    /// </summary>
    /// <param name="capsuleData">�J�v�Z���f�[�^</param>
    /// <param name="circleData">�~�f�[�^</param>
    /// <param name="hitPointA">�J�v�Z����̐ڐG�_</param>
    /// <param name="hitPointB">�~��̐ڐG�_</param>
    /// <returns>�Փ˂��Ă���ꍇ��true</returns>
    public static bool CheckCollision3D(this CapsuleData3D capsuleData, CircleData3D circleData, out Vector3 hitPointA, out Vector3 hitPointB)
    {

        Vector3 point = GetNearestPoint3D(capsuleData.originPoint, capsuleData.endPoint, circleData.position);

        // �~�̒��S�ƍŋߓ_�̃x�N�g������ы������v�Z
        Vector3 direction = circleData.position - point;
        float distance = GetDistance3D(circleData.position, point);
        if (distance - (capsuleData.radius + circleData.radius) <= 0)
        {
            // �J�v�Z����̐ڐG�_
            hitPointA = point + direction.normalized * capsuleData.radius;
            // �~��̐ڐG�_
            hitPointB = circleData.position - direction.normalized * circleData.radius;
            return true;
        }
        // �Փ˂��Ă��Ȃ��ꍇ�̓f�t�H���g�l��Ԃ�
        hitPointA = hitPointB = default;
        return false;
    }
    /// <summary>
    /// �J�v�Z���Ɛ�����3D�Փ˔�����s���A�����̐ڐG�_���v�Z���܂��B
    /// </summary>
    /// <param name="capsuleData">�J�v�Z���f�[�^</param>
    /// <param name="lineData">�����f�[�^</param>
    /// <param name="hitPointA">�J�v�Z����̐ڐG�_</param>
    /// <param name="hitPointB">������̐ڐG�_</param>
    /// <returns>�Փ˂��Ă���ꍇ��true</returns>
    public static bool CheckCollision3D(this CapsuleData3D capsuleData, LineData3D lineData, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        // ������̍ŋߓ_���v�Z
        Vector3 lineNearestPoint = GetNearestPoint3D(lineData.originPoint, lineData.endPoint, capsuleData.originPoint);

        // �J�v�Z���̒��S����̍ŋߓ_���v�Z
        Vector3 capsuleNearestPoint = GetNearestPoint3D(capsuleData.originPoint, capsuleData.endPoint, lineData.originPoint);
        float distance = GetLinesDistance3D(capsuleData.ToLine(), lineData);

        if (distance - (capsuleData.radius + capsuleData.radius) <= 0)
        {
            // �ڐG�_���v�Z
            hitPointA = capsuleNearestPoint;  // �J�v�Z����̐ڐG�_
            hitPointB = lineNearestPoint;    // ������̐ڐG�_
            return true;
        }
        hitPointA = hitPointB = default;
        return false;
    }

    /// <summary>
    /// 2�̃J�v�Z����3D�Փ˔�����s���A�����̐ڐG�_���v�Z���܂��B
    /// </summary>
    /// <param name="capsuleData1">1�ڂ̃J�v�Z���f�[�^</param>
    /// <param name="capsuleData2">2�ڂ̃J�v�Z���f�[�^</param>
    /// <param name="hitPointA">1�ڂ̃J�v�Z����̐ڐG�_</param>
    /// <param name="hitPointB">2�ڂ̃J�v�Z����̐ڐG�_</param>
    /// <returns>�Փ˂��Ă���ꍇ��true</returns>
    public static bool CheckCollision3D(this CapsuleData3D capsuleData1, CapsuleData3D capsuleData2, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        // ���J�v�Z���̐�����̍ŋߓ_���擾
        Vector3 nearestPoint1 = GetNearestPoint3D(capsuleData1.originPoint, capsuleData1.endPoint, capsuleData2.originPoint);
        Vector3 nearestPoint2 = GetNearestPoint3D(capsuleData2.originPoint, capsuleData2.endPoint, capsuleData1.originPoint);
        float distance = GetLinesDistance3D(capsuleData1.ToLine(), capsuleData2.ToLine());
        // �ŋߓ_�Ԃ̋������v�Z
        Vector3 direction = nearestPoint2 - nearestPoint1;
        if (distance - (capsuleData1.radius + capsuleData2.radius) <= 0)
        {
            // 1�ڂ̃J�v�Z����̐ڐG�_
            hitPointA = nearestPoint1 + direction.normalized * capsuleData1.radius;
            // 2�ڂ̃J�v�Z����̐ڐG�_
            hitPointB = nearestPoint2 - direction.normalized * capsuleData2.radius;
            return true;
        }

        // �Փ˂��Ă��Ȃ��ꍇ�̓f�t�H���g�l��Ԃ�
        hitPointA = hitPointB = default;
        return false;
    }

    /// <summary>
    /// �J�v�Z���ƃ{�b�N�X��2D�Փ˔�����s���A�����̐ڐG�_���v�Z���܂��B
    /// </summary>
    /// <param name="capsule">�J�v�Z���f�[�^</param>
    /// <param name="box">�{�b�N�X�f�[�^</param>
    /// <param name="hitPointA">�J�v�Z����̐ڐG�_</param>
    /// <param name="hitPointB">�{�b�N�X��̐ڐG�_</param>
    /// <returns>�Փ˂��Ă���ꍇ��true</returns>
    public static bool CheckCollision3D(this CapsuleData3D capsule, BoxData3D box, out Vector3 hitPointA, out Vector3 hitPointB)
    {
        hitPointA = hitPointB = default;

        // �{�b�N�X�̃G�b�W���X�g���擾
        Vector3[][] edges = box.GetEdges();
        float minDistance = float.MaxValue;
        Vector3 closestCapsulePoint = default;
        Vector3 closestBoxPoint = default;
        // �{�b�N�X�̊e�G�b�W�ɂ��Ĕ���
        foreach (Vector3[] edge in edges)
        {
            Vector3 edgeStart = edge[0];
            Vector3 edgeEnd = edge[1];

            // �J�v�Z���̐����ƃ{�b�N�X�̃G�b�W�Ԃ̍ŋߓ_���v�Z
            Vector3 capsuleNearestPoint = GetNearestPoint3D(capsule.originPoint, capsule.endPoint, edgeStart);
            Vector3 edgeNearestPoint = GetNearestPoint3D(edgeStart, edgeEnd, capsuleNearestPoint);

            // �ŋߓ_�Ԃ̋������v�Z
            float distance = GetDistance3D(capsuleNearestPoint, edgeNearestPoint);

            // �ŒZ�����̍X�V
            if (distance < minDistance)
            {
                minDistance = distance;
                closestCapsulePoint = capsuleNearestPoint;
                closestBoxPoint = edgeNearestPoint;
            }
        }
        // �Փ˔���: �ŒZ�������J�v�Z���̔��a�ȉ���
        if (minDistance <= capsule.radius)
        {
            // �ڐG�_��ݒ�
            Vector3 direction = (closestBoxPoint - closestCapsulePoint).normalized;
            hitPointA = closestCapsulePoint + direction * capsule.radius; // �J�v�Z����̐ڐG�_
            hitPointB = closestBoxPoint; // �{�b�N�X��̐ڐG�_
            return true;
        }

        return false;
    }
    #endregion
    #region circle
    /// <summary>
    /// CircleData2D�ƔC�ӂ̃I�u�W�F�N�g�̏Փ˔���B
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

        // �Ή����Ă��Ȃ��^�̏ꍇ�͏Փ˂Ȃ��Ƃ݂Ȃ�
        hitPointA = hitPointB = default;
        return false;
    }



    public static bool CheckCollision3D(this CircleData3D circleData, CircleData3D circleData1, out Vector3 hitPointA, out Vector3 hitPointB)
    {


        float distance = GetDistance3D(circleData.position, circleData1.position);

        if (distance - (circleData.radius + circleData1.radius) <= 0)
        {
            // ���ꂼ��̐ڐG�_���v�Z
            Vector3 direction = (circleData1.position - circleData.position).normalized;
            hitPointA = circleData.position + direction * circleData.radius; // circleData�
            hitPointB = circleData1.position - direction * circleData1.radius; // circleData1�
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
            hitPointA = circleData.position + direction * circleData.radius; // �~�̐ڐG�_
            hitPointB = point; // �����̐ڐG�_
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
            hitPointA = circleData.position + direction * circleData.radius; // �~�̐ڐG�_
            hitPointB = point - direction * capsuleData.radius; // �J�v�Z���̐ڐG�_
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

        // �{�b�N�X�̊e�G�b�W�Ɖ~�̍ŋߖT�_���v�Z
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

        // �ŋߖT�_���~�̔��a�ȓ��łȂ���ΐڐG�Ȃ�
        if (minDistance > circle.radius)
        {
            return false;
        }


        hitPointB = closestPoint; // �{�b�N�X��̐ڐG�_

        // �~����̐ڐG�_���v�Z
        Vector3 direction = (closestPoint - circle.position).normalized;
        hitPointA = circle.position + direction * circle.radius;

        return true;
    }
    #endregion
    #region line
    /// <summary>
    /// �����ƔC�ӂ̏ՓˑΏۂƂ̏Փ˔�����s���A�o�����̐ڐG�_��Ԃ��܂��B
    /// </summary>
    /// <param name="lineData">�����f�[�^</param>
    /// <param name="collisionData">�ՓˑΏۃf�[�^</param>
    /// <param name="hitPointA">������̐ڐG�_</param>
    /// <param name="hitPointB">�ՓˑΏۊ�̐ڐG�_</param>
    /// <returns>�Փ˂��Ă��邩�ǂ���</returns>
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
    /// �����Ɠ_�̏Փ˔�����s���A�o�����̐ڐG�_��Ԃ��܂��B
    /// </summary>
    /// <param name="lineData">�����f�[�^</param>
    /// <param name="position">�_�̍��W</param>
    /// <param name="hitPointA">������̐ڐG�_</param>
    /// <param name="hitPointB">�_��̐ڐG�_</param>
    /// <returns>�Փ˂��Ă��邩�ǂ���</returns>
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
    /// �����Ɖ~�̏Փ˔�����s���A�o�����̐ڐG�_��Ԃ��܂��B
    /// </summary>
    /// <param name="lineData">�����f�[�^</param>
    /// <param name="circleData">�~�f�[�^</param>
    /// <param name="hitPointA">������̐ڐG�_</param>
    /// <param name="hitPointB">�~��̐ڐG�_</param>
    /// <returns>�Փ˂��Ă��邩�ǂ���</returns>
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
    /// �����ƃJ�v�Z���̏Փ˔�����s���A�o�����̐ڐG�_��Ԃ��܂��B
    /// </summary>
    /// <param name="lineData">�����f�[�^</param>
    /// <param name="capsuleData">�J�v�Z���f�[�^</param>
    /// <param name="hitPointA">������̐ڐG�_</param>
    /// <param name="hitPointB">�J�v�Z����̐ڐG�_</param>
    /// <returns>�Փ˂��Ă��邩�ǂ���</returns>
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
    /// 2�̐����̏Փ˔�����s���A�o�����̐ڐG�_��Ԃ��܂��B
    /// </summary>
    /// <param name="lineData1">1�ڂ̐���</param>
    /// <param name="lineData2">2�ڂ̐���</param>
    /// <param name="hitPointA">����1��̐ڐG�_</param>
    /// <param name="hitPointB">����2��̐ڐG�_</param>
    /// <returns>�Փ˂��Ă��邩�ǂ���</returns>
    public static bool CheckCollision3D(this LineData3D lineData1, LineData3D lineData2, out Vector3 hitPointA, out Vector3 hitPointB)
    {

        float distance = GetLinesDistance3D(lineData1, lineData2);
        if (distance <= 0)
        {
            hitPointA = lineData1.originPoint; // ���̒l
            hitPointB = lineData2.originPoint; // ���̒l
            return true;
        }
        hitPointB = hitPointA = default;
        return false;
    }
    /// <summary>
    /// �����ƃ{�b�N�X�̏Փ˔�����s���A�o�����̐ڐG�_��Ԃ��܂��B
    /// </summary>
    /// <param name="lineData">�����f�[�^</param>
    /// <param name="box">�{�b�N�X�f�[�^</param>
    /// <param name="hitPointA">������̐ڐG�_</param>
    /// <param name="hitPointB">�{�b�N�X��̐ڐG�_</param>
    /// <returns>�Փ˂��Ă��邩�ǂ���</returns>
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
    /// �{�b�N�X�ƔC�ӂ̏ՓˑΏۂƂ̏Փ˔�����s���A�ڐG�_��Ԃ��܂��B
    /// </summary>
    /// <param name="boxData">�{�b�N�X�f�[�^</param>
    /// <param name="collisionData">�ՓˑΏۃf�[�^</param>
    /// <param name="hitPointA">�{�b�N�X��̐ڐG�_</param>
    /// <param name="hitPointB">�ՓˑΏۊ�̐ڐG�_</param>
    /// <returns>�Փ˂��Ă��邩�ǂ���</returns>
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
        return false; // ���Ή��̌^�̏ꍇ
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

        // �ŋߖT�_���{�b�N�X�̐�����Ŏ擾
        if (!box.GetNearestPointOnLine(position, out Vector3 nearestPoint))
        {
            return false;
        }


        // �ŋߖT�_�Ɠ_�̋������{�b�N�X�̕��𒴂���ꍇ�͏Փ˂Ȃ�
        if (GetDistance3D(nearestPoint, position) > box.boxWidth / 2)
        {
            return false;
        }


        hitPointA = nearestPoint;  // �{�b�N�X��̐ڐG�_
        hitPointB = position;      // �_��̐ڐG�_
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

        // �{�b�N�X�Ɛ����̍ŋߖT�_���擾
        Vector2 nearestPoint = GetNearestPoint3D(box.ToLine(), line);

        if (!box.GetNearestPointOnLine(nearestPoint, out Vector3 point))
        {
            return false;
        }


        // �ŋߖT�_�Ɛ����̋������{�b�N�X�̕��𒴂���ꍇ�͏Փ˂Ȃ�
        if (GetDistance3D(point, nearestPoint) > box.boxWidth / 2)
        {
            return false;
        }


        hitPointA = point;        // �{�b�N�X��̐ڐG�_
        hitPointB = nearestPoint; // ������̐ڐG�_
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

        // �{�b�N�X�̊e�G�b�W�Ɖ~�̍ŋߖT�_���v�Z
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
        // �ŋߖT�_���~�̔��a�ȓ��łȂ���ΐڐG�Ȃ�
        if (minDistance > circle.radius)
            return false;

        hitPointA = closestPoint; // �{�b�N�X��̐ڐG�_

        // �~����̐ڐG�_���v�Z
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

        // �{�b�N�X�̃G�b�W���X�g���擾
        Vector3[][] edges = box.GetEdges();

        float minDistance = float.MaxValue;
        Vector3 closestCapsulePoint = default;
        Vector3 closestBoxPoint = default;

        // �{�b�N�X�̊e�G�b�W�ɂ��Ĕ���
        foreach (Vector3[] edge in edges)
        {
            Vector3 edgeStart = edge[0];
            Vector3 edgeEnd = edge[1];

            // �J�v�Z���̐����ƃ{�b�N�X�̃G�b�W�Ԃ̍ŋߓ_���v�Z
            Vector3 capsuleNearestPoint = GetNearestPoint3D(capsule.originPoint, capsule.endPoint, edgeStart);
            Vector3 edgeNearestPoint = GetNearestPoint3D(edgeStart, edgeEnd, capsuleNearestPoint);

            // �ŋߓ_�Ԃ̋������v�Z
            float distance = GetDistance3D(capsuleNearestPoint, edgeNearestPoint);

            // �ŒZ�����̍X�V
            if (distance < minDistance)
            {
                minDistance = distance;
                closestCapsulePoint = capsuleNearestPoint;
                closestBoxPoint = edgeNearestPoint;
            }
        }

        // �Փ˔���: �ŒZ�������J�v�Z���̔��a�ȉ���
        if (minDistance > capsule.radius)
        {
            return false;
        }
        // �ڐG�_��ݒ�
        Vector3 direction = (closestBoxPoint - closestCapsulePoint).normalized;
        hitPointB = closestCapsulePoint + direction * capsule.radius; // �J�v�Z����̐ڐG�_
        hitPointA = closestBoxPoint; // �{�b�N�X��̐ڐG�_
        return true;
    }
    
    public static bool CheckCollision3D(this BoxData3D box1, BoxData3D box2,out Vector3 hitPointA,out Vector3 hitPointB)
    {
        hitPointA = hitPointB = default;

        // �ŋߖT�_���擾
        Vector2 nearestPoint = GetNearestPoint3D(box1.ToLine(), box2.ToLine());

        if (!box1.GetNearestPointOnLine(nearestPoint, out Vector3 point))
            return false;

        // �ŋߖT�_�ƃ{�b�N�X�̋��������ꂼ��̕��̍��v�𒴂���ꍇ�͏Փ˂Ȃ�
        if (GetDistance3D(point, nearestPoint) > (box1.boxWidth + box2.boxWidth) / 2)
            return false;

        hitPointA = point;        // �{�b�N�X1��̐ڐG�_
        hitPointB = nearestPoint; // �{�b�N�X2��̐ڐG�_
        return true;
    }


    
    /// �_�����C���̊O������
    /// </summary>
    /// <param name="box"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private static bool CheckLineOut(Vector3 origin, Vector3 end, Vector3 position, out Vector3 nearestPoint)
    {
        // �����̕����x�N�g��
        Vector3 direction = (end - origin);
        float lineLength = direction.magnitude;   // �����̒���
        direction.Normalize();                   // �P�ʃx�N�g����

        // �����̊J�n�_����Ώۓ_�ւ̃x�N�g��
        Vector3 originToPoint = position - origin;

        // ������̍ŋߖT�_�̈ʒu���X�J���[�l�Ōv�Z
        float projection = Vector3.Dot(originToPoint, direction);

        // �����͈̔͊O���`�F�b�N
        if (projection < 0)
        {
            // �ŋߖT�_�� origin ���̒[�_
            nearestPoint = origin;
            return false; // �͈͊O
        }
        else if (projection > lineLength)
        {
            // �ŋߖT�_�� end ���̒[�_
            nearestPoint = end;
            return false; // �͈͊O
        }

        // ������̍ŋߖT�_���v�Z
        nearestPoint = origin + direction * projection;

        return true; // ������ɍŋߖT�_�����݂���
    }

    /// <summary>
    /// �_���{�b�N�X�̐����̊O�ɂ��邩���ʂ��A�ŋߖT�_���v�Z���܂��B
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
