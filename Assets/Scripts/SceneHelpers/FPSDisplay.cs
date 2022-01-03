using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    [SerializeField]
    private Text m_FpsText;

    private int m_FrameCounter;
    private int m_FrameCount = 30;
    private void Update()
    {
        float fps = 1 / Time.unscaledDeltaTime;

        if (m_FrameCounter > m_FrameCount)
        {
            m_FpsText.text = "" + fps;
            m_FrameCounter = 0;
        }

        m_FrameCounter++;

    }
}
