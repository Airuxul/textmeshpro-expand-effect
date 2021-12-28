using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// 晃动效果
/// </summary>
[CreateAssetMenu(menuName = "TMP_ExpandEffectObject/TMP_RichText/ShakeRichText",fileName ="ShakeRichText" )]
public class TMP_ShakeRichText : TMP_BaseRichText
{
    [Header("晃动幅度")]
    public float shakeDegree=1;
    [Header("晃动频率")]
    public float shakeFrequency = 0.1f;
    
    private float currentTime;
    /// <summary>
    /// 当前偏移下标
    /// </summary>
    private int currentVectorIndex;
    
    /// <summary>
    /// 偏移向量存储
    /// </summary>
    private List<Vector2> currentOffset;
    
    public override void StartEffect()
    {
        currentOffset = new List<Vector2>();
        currentTime = shakeFrequency;
    }
    
    public override void RichTextEffect(ref TMP_TextInfo textInfo, List<Vector2> ranges)
    {
        currentTime += Time.deltaTime;
        
        //到达下个时间则重新创建偏移向量组
        if (currentTime > shakeFrequency)
        {
            currentTime = 0;
            currentOffset.Clear();
            for (int k = 0; k < ranges.Count; k++)
            {
                for (int i = (int)ranges[k].x; i < (int)ranges[k].y + 1; i++)
                {
                    currentOffset.Add(Random_UnitVector2());
                }
            }
        }
        //偏移向量下标归零
        currentVectorIndex = 0;
        for (int k = 0; k < ranges.Count; k++)
        {
            for (int i = (int)ranges[k].x; i < (int)ranges[k].y + 1; i++)
            {
                //获取当前字符的characterInfo
                charInfo = textInfo.characterInfo[i];
                
                //如果不可视则跳过该字符，否则会导致某些字符闪烁
                if (!charInfo.isVisible)
                {
                    continue;
                }
                //通过上面获取的characterInfo获取该字符的顶点坐标组
                var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                
                for (int j = 0; j < 4; j++)
                {
                    var orig = verts[charInfo.vertexIndex + j];
                    //根据偏移随机向量调整顶点位置
                    verts[charInfo.vertexIndex + j] = orig + 
                        new Vector3(currentOffset[currentVectorIndex].x, currentOffset[currentVectorIndex].y, 0) * shakeDegree;
                }
                currentVectorIndex++;
            }
        }
    }
    private Vector2 Random_UnitVector2()
    {
        var angle = Random.Range(0, 2 * Mathf.PI);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
    private void OnDisable()
    {
        currentOffset = null;
    }
    private void OnDestroy()
    {
        currentOffset = null;
    }
}
