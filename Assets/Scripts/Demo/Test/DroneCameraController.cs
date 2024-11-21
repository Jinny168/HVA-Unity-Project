using UnityEngine;

public class DroneCameraController : MonoBehaviour
{
    public Transform droneTransform; // 无人机模型的Transform组件
    public Camera droneCamera; // 用于显示无人机视野的相机

    public float distanceFromDrone = 5.0f; // 相机距离无人机的距离
    public float heightOffset = 2.0f; // 相机在无人机上的高度偏移
    public float cameraSpeed = 5.0f; // 相机移动速度

    void Update()
    {
        // 计算相机目标位置
        Vector3 targetPosition = droneTransform.position - droneTransform.forward * distanceFromDrone + droneTransform.up * heightOffset;
        // 使用插值平滑相机移动
        transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSpeed * Time.deltaTime);
        // 相机朝向无人机前方
        transform.LookAt(droneTransform.position + droneTransform.forward * 10.0f);
    }
}
