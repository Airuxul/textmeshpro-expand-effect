using TMPro;
using UnityEngine;
public interface TMP_ITyping
{
    /// <summary>
    /// 打印开始准备函数
    /// </summary>
    void StartTyping(ref TMP_TextInfo textInfo);
    /// <summary>
    /// 打印函数
    /// </summary>
    /// <returns>是否打印完毕</returns>
    bool Typing(ref TMP_TextInfo textInfo);
}
/// <summary>
/// 直接显示，打印类基类
/// </summary>
[CreateAssetMenu(menuName = "TMP_ExpandEffectObject/TMP_Typing/NoneTyping" ,fileName = "NoneTyping")]
public class TMP_BaseTyping : ScriptableObject,TMP_ITyping
{
    protected TMP_MeshInfo[] saveMeshInfos;
    protected TMP_CharacterInfo charInfo;
    public virtual void StartTyping(ref TMP_TextInfo textInfo) {}
    public virtual bool Typing(ref TMP_TextInfo textInfo)
    {
        return true;
    }

    protected  void ReleaseMemory()
    {
        saveMeshInfos = null;
    }
}
