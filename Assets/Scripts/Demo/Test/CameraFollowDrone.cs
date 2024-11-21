using UnityEngine;

public class CameraFollowDrone : MonoBehaviour
{
    public Transform droneTransform; // ���˻�ģ�͵�Transform���
    public float distanceFromDrone = 5.0f; // ����������˻��ľ���
    public float heightOffset = 2.0f; // ��������˻��ϵĸ߶�ƫ��
    public float rotationDamping = 5.0f; // �����ת������

    void LateUpdate()
    {
        // �������Ŀ��λ��
        Vector3 targetPosition = droneTransform.position - droneTransform.forward * distanceFromDrone + droneTransform.up * heightOffset;
        // ���������Ŀ��λ��
        transform.position = targetPosition;
        // �������Ŀ����ת
        Quaternion targetRotation = Quaternion.LookRotation(droneTransform.position - transform.position, droneTransform.up);
        // ʹ������ƽ�������ת
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationDamping * Time.deltaTime);
    }
}
