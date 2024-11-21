using UnityEngine;

public class CameraFollowDrone : MonoBehaviour
{
    public Transform droneTransform; // 无人机模型的Transform组件
    public float distanceFromDrone = 5.0f; // 相机距离无人机的距离
    public float heightOffset = 2.0f; // 相机在无人机上的高度偏移
    public float rotationDamping = 5.0f; // 相机旋转的阻尼

    void LateUpdate()
    {
        // 计算相机目标位置
        Vector3 targetPosition = droneTransform.position - droneTransform.forward * distanceFromDrone + droneTransform.up * heightOffset;
        // 设置相机的目标位置
        transform.position = targetPosition;
        // 计算相机目标旋转
        Quaternion targetRotation = Quaternion.LookRotation(droneTransform.position - transform.position, droneTransform.up);
        // 使用阻尼平滑相机旋转
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationDamping * Time.deltaTime);
    }
}
