using TMPro;
using UnityEngine;

/// <summary>
/// 淡入打印
/// </summary>
[CreateAssetMenu(menuName = "TMP_ExpandEffectObject/TMP_Typing/FadeInTyping" ,fileName = "FadeInTyping")]
public class TMP_FadeInTyping :TMP_BaseTyping
{
    [Header("淡入速度")]
    public float fadeInSpeed = 1;//单位为字符
    [Header("淡入范围")]
    public float fadeRange=5;//单位为字符

    private float currentTime;
    //范围最右边的下标
    private int rightIndex;
    
    public override void StartTyping()
    {
        currentTime = 0;
        rightIndex= 0;
        tmpText.color = new Color(1, 1, 1, 0);
    }
    public override bool Typing()
    {
        currentTime += Time.deltaTime;
        //根据fadeInSpeed来增加rightIndex
        if (currentTime > 1 / fadeInSpeed)
        {
            currentTime = 0;
            rightIndex++;
            if ( rightIndex-fadeRange >= tmpText.textInfo.characterCount-1)
            {
                return true;
            }
        }
        for (var i = 0; i>-1&&i<=rightIndex; i++)
        {
            //如果超出范围直接退出循环
            if (i >= tmpText.textInfo.characterCount)
            {
                break;
            }
            // 不处理不可见字符，否则可能导致某些位置的字符闪烁
            if (!tmpText.textInfo.characterInfo[i].isVisible)
            {
                continue;
            }
            //根据范围对每个下标进行透明度设置
            var alpha = (byte)Mathf.Clamp(255/fadeRange*(rightIndex-i), 0, 255);
            SetCharVertexAlpha(i, alpha);
        }
        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        return false;
    }
}
