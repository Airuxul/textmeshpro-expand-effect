using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface TMP_IRichText
{
    /// <summary>
    /// 为富文本效果初始化
    /// </summary>
    public void StartEffect();
    /// <summary>
    /// 富文本效果函数
    /// </summary>
    /// <param name="textInfo">需要使用的textinfo</param>
    /// <param name="ranges"></param>
    public void RichTextEffect(ref TMP_TextInfo textInfo,List<Vector2> ranges);
}
/// <summary>
/// 富文本基类，不能直接创建和使用
/// </summary>
public class TMP_BaseRichText : ScriptableObject, TMP_IRichText
{
    /// <summary>
    /// 富文本名称
    /// </summary>
    public string richTextName;
    //保存用，仅参数
    protected TMP_CharacterInfo charInfo;
    public virtual void StartEffect() {}
    public virtual void RichTextEffect(ref TMP_TextInfo textInfo, List<Vector2> ranges)
    {
        throw new NotImplementedException();
    }
}