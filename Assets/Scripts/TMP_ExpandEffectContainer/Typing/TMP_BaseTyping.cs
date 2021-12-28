using TMPro;
using UnityEngine;
public interface TMP_ITyping
{
    /// <summary>
    /// 打印开始准备函数
    /// </summary>
    void StartTyping();
    /// <summary>
    /// 打印函数
    /// </summary>
    /// <returns>是否打印完毕</returns>
    bool Typing();
    /// <summary>
    /// 设置TMP_Text
    /// </summary>
    /// <param name="container">container的TMP_Text</param>
    void SetTMP(TMP_Text container);
}
/// <summary>
/// 直接显示，打印类基类
/// </summary>
[CreateAssetMenu(menuName = "TMP_ExpandEffectObject/TMP_Typing/NoneTyping" ,fileName = "NoneTyping")]
public class TMP_BaseTyping : ScriptableObject,TMP_ITyping
{
    protected TMP_Text tmpText;
    public virtual void StartTyping() {}
    public virtual bool Typing()
    {
        return true;
    }
    public void SetTMP(TMP_Text _tmpText)
    {
        tmpText = _tmpText;
    }
    
    /// <summary>
    /// 设置index下标字符的alpha
    /// </summary>
    /// <param name="index">下标</param>
    /// <param name="alpha">alpha值，注意是byte，255才为不透明</param>
    protected void SetCharVertexAlpha(int index, byte alpha)
    {
        int vertexIndex = tmpText.textInfo.characterInfo[index].vertexIndex;
        int marteralIndex =tmpText.textInfo.characterInfo[index].materialReferenceIndex;
        var newVertexColors = tmpText.textInfo.meshInfo[marteralIndex].colors32;
        newVertexColors[vertexIndex + 0].a = alpha;
        newVertexColors[vertexIndex + 1].a = alpha;
        newVertexColors[vertexIndex + 2].a = alpha;
        newVertexColors[vertexIndex + 3].a = alpha;
    }
}
