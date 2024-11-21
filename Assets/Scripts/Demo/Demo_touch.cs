using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ������Ļ��������ƶ���ָ��λ��
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
            //������Ϊ��㣬���Ϊ�м̵�����
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //��¼��ײ����Ϣ
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
