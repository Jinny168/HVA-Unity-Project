using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraContrller : MonoBehaviour
{


    int currentCamIndex = 0;

    WebCamTexture tex;

    public RawImage display;

    public Text startStopText;

    public void SwapCam_Clicked()
    {
        if (WebCamTexture.devices.Length > 0)//WebCamTexture.devices.Length��ʾ������Ե���������������������������0ʱ�����������л�������
        {
            currentCamIndex += 1;
            currentCamIndex %= WebCamTexture.devices.Length;//���д������ѧϰ��ֵ��������˼��˵�趨һ������������δ�ﵽ�����ʱ���������ӣ����������ﵽ�����ʱ�ᱻ����Ϊ0
        }
    }


    public void StartStopCam_Clicked()
    {
        if (tex != null)//stop camera
        {
            StopWebCam();
            startStopText.text = "Start Camera";

        }
        else//start camera
        {
            WebCamDevice device = WebCamTexture.devices[currentCamIndex];
            tex = new WebCamTexture(device.name);//�����豸���ƴ���һ���µ�WebCamTexture���࣬����ֵ��tex����ʱtex�Ѿ�����������ͷ����Ƶ�źš�
            display.texture = tex;//������ͷ����Ƶ�źŴ��ݸ�RawImage�н��л�����ʾ��

            tex.Play();//����
            startStopText.text = "Stop Camera";
        }

    }

    private void StopWebCam()
    {
        display.texture = null;
        tex.Stop();
        tex = null;
    }
}

