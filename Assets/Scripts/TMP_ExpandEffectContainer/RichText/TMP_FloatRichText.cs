using System.Collections.Generic;
using TMPro;
using UnityEngine;
[CreateAssetMenu(menuName = "TMP_ExpandEffectObject/TMP_RichText/FloatRichText",fileName ="FloatRichText" )]
public class TMP_FloatRichText : TMP_BaseRichText 
{
    [Header("浮动程度")]
    public float floatDegree = 5f;
    
    [Header("浮动速度")]
    public float floatSpeed = 10f;

    [Header("浮动频率")]
    [Range(1,10)] 
    public float floatFrequency = 4;
    
    public override void RichTextEffect(ref TMP_TextInfo textInfo,List<Vector2> ranges)
    {
        for (int k = 0; k < ranges.Count; k++)
        {
            for (int i = (int)ranges[k].x; i < (int)ranges[k].y + 1; i++)
            {
                charInfo =textInfo.characterInfo[i];
    
                if (!charInfo.isVisible)
                {
                    continue;
                }
    
                int materialIndex =textInfo.characterInfo[i].materialReferenceIndex;
                
                var verts = textInfo.meshInfo[materialIndex].vertices;

                for (int j = 0; j < 4; j++)
                {
                    var orig = verts[charInfo.vertexIndex + j];
                    //根据sin函数实现上下浮动
                    verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time * floatSpeed + orig.x*floatFrequency/100f) * floatDegree, 0);
                }
            } 
        }
    }
}
