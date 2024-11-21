using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cameraDisplay : MonoBehaviour
{
    public RenderTexture rt;
    public Camera cam;
    private RawImage UIImage;
    Toggle isRawCamera;
    void Start()
    {
        if (isRawCamera == null)
        {
            isRawCamera = GetComponent<Toggle>();
        }
        isRawCamera.isOn = false;

        if (UIImage == null)
        {
            UIImage = GameObject.Find("RawImage").GetComponent<RawImage>();
        }
        UIImage.enabled = false;
        UIImage.texture = null;
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            if (isRawCamera.isOn == false)
            {
                isRawCamera.isOn = true;
            }
            else
            {
                isRawCamera.isOn = false;
            }
        }
        if (isRawCamera.isOn == true)
        {
            UIImage.enabled = true;
            UIImage.texture = rt;
        }
        else
        {
            UIImage.enabled = false;
            UIImage.texture = null;
        }
    }
}
