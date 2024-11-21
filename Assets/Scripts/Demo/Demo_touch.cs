using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 触摸屏幕，将物块移动至指定位置
/// </summary>
public class Demo_touch : MonoBehaviour
{
    public float depth;
    public float moveSpeed;
    public Vector3 mouseWorldPosition;
    private Transform currentObject;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            //以主摄为起点，鼠标为中继的射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //记录碰撞的信息
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) 
            {
                currentObject = hit.transform;
            }
            if (currentObject == null) 
            {
                return;
            }
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = depth;
            mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            currentObject.position = Vector3.Lerp(currentObject.position, mouseWorldPosition, moveSpeed * Time.deltaTime);
        }
        if (Input.GetMouseButtonUp(1))
        {
            currentObject = null;
        }


    }
}
