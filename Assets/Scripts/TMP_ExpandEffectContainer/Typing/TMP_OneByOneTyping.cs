using TMPro;
using UnityEngine;

/// <summary>
/// 字符依次打印
/// </summary>
[CreateAssetMenu(menuName = "TMP_ExpandEffectObject/TMP_Typing/OneByOneTyping" ,fileName = "neByOneTyping")]
public class TMP_OneByOneTyping :TMP_BaseTyping
{
    [Header("每个字符延时显示间隔时间")]
    public float delayTime=0.1f;
    private float currentTime;
    private bool isOverHalf;
    private int indexVisable;

    /// <summary>
    /// 开始打印前的处理
    /// </summary>
    public override void StartTyping()
    {
        currentTime = 0;
        isOverHalf = false;
        indexVisable = 0;
        tmpText.color = new Color(1, 1, 1, 0);
    }
    
    /// <summary>
    /// 根据当前下标，修改文字的透明度实现依次显示
    /// 如果没有超过一半则TMP的color为透明，将前面应当显示的文字的顶点透明度设置为不透明
    /// 如果超过一半则将TMP的color为不透明，将后面应当不显示的文字顶点设置为不透明
    /// 通过此优化文字透明设置
    /// </summary>
    public override bool Typing()
    {
        currentTime += Time.deltaTime;
        if (currentTime > delayTime)
        {
            currentTime = 0;
            indexVisable++;
            //是否第一次超过一半
            if (!isOverHalf&&indexVisable > (tmpText.text.Length-1)/2)
            {
                isOverHalf = true;
                tmpText.color = new Color(1, 1, 1, 1);
            }
            if (indexVisable > tmpText.text.Length-1)
            {
                return true;
            }
        }

        int startIndex =0;
        int endIndex = indexVisable;
        byte alpha= 255;
        //如果超过一半，则重新赋值设置透明度的三个变量
        if (isOverHalf)
        {
            startIndex = indexVisable +1;
            endIndex = tmpText.text.Length-1;
            alpha = 0;
        }
        for (int i = startIndex; i <= endIndex; i++)
        {
            // 不处理不可见字符，否则可能导致某些位置的字符闪烁
            if (!tmpText.textInfo.characterInfo[i].isVisible)
            {
                continue;
            }
            SetCharVertexAlpha(i, alpha);
        }
        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        return false;
    }
}
