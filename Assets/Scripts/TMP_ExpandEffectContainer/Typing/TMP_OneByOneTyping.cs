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
    private int indexVisable;
    public override void StartTyping(ref TMP_TextInfo textInfo)
    {
        currentTime = 0;
        indexVisable = 0;
        
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
        
        if (currentTime > delayTime)
        {
            currentTime = 0;
            indexVisable++;
            if (indexVisable > textInfo.characterCount-1)
            {
                ReleaseMemory();
                return true;
            }
            #region 处理顶点颜色数据
            //当下标发生改变时修改对应范围透明度
            charInfo =textInfo.characterInfo[indexVisable];
            var materialIndex =textInfo.characterInfo[indexVisable].materialReferenceIndex;
            //根据范围对每个下标进行透明度设置
            for (var i = 0; i < 4; i++)
            {
                saveMeshInfos[materialIndex].colors32[charInfo.vertexIndex+i].a = 255;
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
        return false;
    }
}
