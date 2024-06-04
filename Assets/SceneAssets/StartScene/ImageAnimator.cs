using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageAnimator : MonoBehaviour
{
    public Image m_Image;
    public SpriteRenderer m_Sprite;

    void Update()
    {
        m_Image.sprite = m_Sprite.sprite;    
    }
}
