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

    public override void StartTyping(ref TMP_TextInfo textInfo)
    {
        currentTime = 0;
        rightIndex= 0;
        saveMeshInfos = textInfo.CopyMeshInfoVertexData();
        
        //初始化saveMesh,将所有char对应的颜色设置为透明
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            charInfo =textInfo.characterInfo[i];

            int materialIndex =textInfo.characterInfo[i].materialReferenceIndex;
            
            for (int j = 0; j < 4; j++)
            { 
                saveMeshInfos[materialIndex].colors32[charInfo.vertexIndex + j].a=0;
            }
        } 
    }
    public override bool Typing(ref TMP_TextInfo textInfo)
    {
        currentTime += Time.deltaTime;

        
        //根据fadeInSpeed来增加rightIndex
        if (currentTime > 1 / fadeInSpeed)
        {
            currentTime = 0;
            rightIndex++;
            
            if ( rightIndex-fadeRange >=textInfo.characterCount-1)
            {
                ReleaseMemory();
                return true;
            }
            #region 处理顶点颜色数据
            //当下标发生改变时修改对应范围透明度
            for (var i = (int)(rightIndex-fadeRange); i>-1&&i<=rightIndex; i++)
            {
                //如果超出范围直接退出循环
                if (i >= textInfo.characterCount)
                {
                    break;
                }
                charInfo =textInfo.characterInfo[i];
                // 不处理不可见字符，否则可能导致某些位置的字符闪烁
                if (!charInfo.isVisible)
                {
                    continue;
                }
                //根据范围对每个下标进行透明度设置
                var alpha = (byte)Mathf.Clamp(255/fadeRange*(rightIndex-i), 0, 255);
                int materialIndex =textInfo.characterInfo[i].materialReferenceIndex;
                for (int j = 0; j < 4; j++)
                {
                    saveMeshInfos[materialIndex].colors32[charInfo.vertexIndex+j].a = alpha;
                }
            }
            #endregion
        }
        #region 传出数据
        //将透明度传入m_TextComponent中
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            charInfo =textInfo.characterInfo[i];
            int materialIndex =textInfo.characterInfo[i].materialReferenceIndex;
            for (int j = 0; j < 4; j++)
            {
                textInfo.meshInfo[materialIndex].colors32[charInfo.vertexIndex+j].a = saveMeshInfos[materialIndex].colors32[charInfo.vertexIndex+j].a;
            }
        }
        #endregion
        //将处理和传出数据的逻辑分开，减少循环次数
        return false;
    }
}
