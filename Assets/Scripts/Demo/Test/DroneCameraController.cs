using UnityEngine;

public class DroneCameraController : MonoBehaviour
{
    public Transform droneTransform; // ���˻�ģ�͵�Transform���
    public Camera droneCamera; // ������ʾ���˻���Ұ�����

    public float distanceFromDrone = 5.0f; // ����������˻��ľ���
    public float heightOffset = 2.0f; // ��������˻��ϵĸ߶�ƫ��
    public float cameraSpeed = 5.0f; // ����ƶ��ٶ�

    void Update()
    {
        // �������Ŀ��λ��
        Vector3 targetPosition = droneTransform.position - droneTransform.forward * distanceFromDrone + droneTransform.up * heightOffset;
        // ʹ�ò�ֵƽ������ƶ�
        transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSpeed * Time.deltaTime);
        // ����������˻�ǰ��
        transform.LookAt(droneTransform.position + droneTransform.forward * 10.0f);
    }
}
