using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// 效果结构体，用于栈提取富文本范围
/// </summary>
internal readonly struct EffectStruct
{
    /// <summary>
    /// 效果下标
    /// </summary>
    public readonly int index;
    /// <summary>
    /// 效果名字
    /// </summary>
    public readonly string textEffectStr;
    public EffectStruct(int _index, string _textEffectStr)
    {
        index = _index;
        textEffectStr = _textEffectStr;
    }
}

/// <summary>
/// TMP文字效果拓展，包含文字打印方式和以富文本为载体的文字效果
/// 包含三个函数
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public partial class TMP_ExpandEffectContainer : MonoBehaviour
{
    #region 成员变量
    //TextMeshPro组件
    private TMP_Text m_TextComponent;

    /// <summary>
    /// 状态机注册
    /// </summary>
    private Dictionary<TMP_StatusEnum, ITMPEEState> states;
    private ITMPEEState currentState;
    
    [Header("拓展打印类型")]
    public TMP_BaseTyping expandTyping;
    
    /// <summary>
    /// 拓展富文本
    /// </summary>
    public List<TMP_BaseRichText> expandEffects=new List<TMP_BaseRichText>();
    /// <summary>
    /// 该栈用来提取富文本范围
    /// </summary>
    private readonly Stack<EffectStruct> effectStack = new Stack<EffectStruct>();
    
    /// <summary>
    /// 存储key值为富文本，value为该富文本效果的范围字典
    /// </summary>
    private Dictionary<string, List<Vector2>> richTextRangeDic;

    /// <summary>
    /// 改变后的TextInfo
    /// </summary>
    private TMP_TextInfo afterChangeInfo;

    /// <summary>
    /// 输出完所有字符后多久文本消失
    /// </summary>
    private float completedSleepTime=-1;
    
    /// <summary>
    /// 输出完所有字符回调函数
    /// </summary>
    private Action<TMP_ExpandEffectContainer> completedAction;

    private Action<TMP_ExpandEffectContainer> sleepAction;

    /// <summary>
    /// 是否初始化
    /// </summary>
    private bool isInitialization;
    
    #if UNITY_EDITOR
    [Header("是否打印富文本范围")]
    public bool isDebugRichTextRange;
    #endif
    #endregion
    
    /// <summary>
    /// 初始化
    /// </summary>
    private void Initialization()
    {
        if (isInitialization)
        {
            return;
        }
        //获取TMP
        if (m_TextComponent == null)
        {
            m_TextComponent = GetComponent<TMP_Text>();
            expandTyping.SetTMP(m_TextComponent);
        }
        //初始化根据富文本字符串集初始化
        if (richTextRangeDic == null)
        {
            richTextRangeDic = new Dictionary<string, List<Vector2>>();
            for (int i = 0; i < expandEffects.Count; i++)
            {
                //初始化所有Effects
                expandEffects[i].StartEffect();
                richTextRangeDic.Add(key: expandEffects[i].richTextName,value: new List<Vector2>());
            }
        }
        if (states == null)
        {
            states = new Dictionary<TMP_StatusEnum, ITMPEEState>();
            states.Add(TMP_StatusEnum.Sleeping,new SleepingState(this));
            states.Add(TMP_StatusEnum.Typing,new TypingState(this));
            states.Add(TMP_StatusEnum.Completed,new CompletedState(this));
        }
        currentState ??= new SleepingState(this);
        isInitialization = true;
    }

    /// <summary>
    /// 显示文本
    /// </summary>
    /// <param name="str">包含富文本的文本内容</param>
    /// <param name="_completedToSleepTime">完成输出后消失时间间隔，-1为默认即永不消失</param>
    /// <param name="_completedAction">完成输出后的回调函数</param>
    /// <param name="_sleepAction">隐藏后的回调函数</param>
    public void ShowText(string str,float _completedToSleepTime=-1,
        Action<TMP_ExpandEffectContainer>_completedAction=null,Action<TMP_ExpandEffectContainer> _sleepAction=null)
    {
        Initialization();
        //清空栈，保证即使之前富文本输出错误，也不会导致之后的文本错误
        effectStack.Clear();

        m_TextComponent.text=GetRichTextIndex(str);
        completedSleepTime = _completedToSleepTime;
        completedAction = _completedAction;
        sleepAction = _sleepAction;
        
        TransitionState(TMP_StatusEnum.Typing);
    }

    /// <summary>
    /// 切换至输出完毕状态即所有字符显现
    /// </summary>
    public void Completed()
    {
        Initialization();
        TransitionState(TMP_StatusEnum.Completed);
    }
    
    /// <summary>
    /// 切换至睡眠状态即隐藏该文本
    /// </summary>
    public void Sleep()
    {
        Initialization();
        TransitionState(TMP_StatusEnum.Sleeping);
    }
    
    private void Update()
    {
        currentState.OnUpdate();
    }
    
    /// <summary>
    /// 将所有的富文本效果实现并将textinfo提交到TMP中更新Mesh
    /// </summary>
    void AllRichTextEffect()
    {
        //没有效果
        if (expandEffects.Count == 0)
        {
            return;
        }
        
        //获取Info并根据expandEffects对afterChangeInfo进行处理
        afterChangeInfo = m_TextComponent.textInfo;

        foreach (var effect in  expandEffects)
        {
            effect.RichTextEffect(ref afterChangeInfo,richTextRangeDic[effect.richTextName]);
        }
        
        //将afterChangeInfo提交到TMP中更新
        for (var i = 0; i < m_TextComponent.textInfo.meshInfo.Length; i++)
        {
            var meshInfo = m_TextComponent.textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            m_TextComponent.UpdateGeometry(meshInfo.mesh, i);
        }
    }
    
    /// <summary>
    /// 根据带有富文本的字符串获取效果的对应下标范围
    /// </summary>
    /// <param name="s">包含富文本的文本</param>
    /// <returns>去除了富文本的文本</returns>
    private string GetRichTextIndex(string s)
    {
        foreach (KeyValuePair<string,List<Vector2>> keyValuePair in richTextRangeDic)
        {
            keyValuePair.Value.Clear();
        }
        effectStack.Clear();
        int strlength = s.Length;
        int currentIndex = 0;
        int signCount = expandEffects.Count;
        int currentSignLength;
        StringBuilder stringBuilder = new StringBuilder(s);
        while (currentIndex < strlength)
        {
            if (stringBuilder[currentIndex] == '<')
            {
                for (int i = 0; i < signCount; i++)
                {
                    currentSignLength = expandEffects[i].richTextName.Length;
                    if (currentIndex + 2 + currentSignLength > strlength)
                    {
                        //这里如果要使用break则需要richTextSign按照字符串由小到大排序，否则会出现问题
                        break;
                    }
                    if (stringBuilder.ToString().Substring(currentIndex, 2 + currentSignLength) == "<"+ expandEffects[i].richTextName +">")
                    {
                        effectStack.Push(new EffectStruct(currentIndex, expandEffects[i].richTextName));
                        stringBuilder.Remove(currentIndex, 2 + currentSignLength);
                        strlength =strlength- 2 - currentSignLength;
                        //避免出for循环后在while循环中多加一
                        currentIndex--;
                        break;
                    }

                    if (currentIndex + 3 +currentSignLength  > strlength)
                    {
                        //这里如果要使用break则需要richTextSign按照字符串由小到大排序，否则会出现问题
                        Debug.Log(currentSignLength);
                        break;
                    }

                    if (stringBuilder.ToString().Substring(currentIndex, 3 + currentSignLength) == "</" + expandEffects[i].richTextName + ">")
                    {
                        if (effectStack.Peek().textEffectStr == expandEffects[i].richTextName)
                        {
                            richTextRangeDic[expandEffects[i].richTextName].Add(new Vector2(effectStack.Pop().index, currentIndex - 1));
                            stringBuilder.Remove(currentIndex, 3 + currentSignLength);
                            strlength = strlength - 3 - currentSignLength;
                            //避免出for循环后在while循环中多加一
                            currentIndex--;
                        }
                        else
                        {
                            Debug.LogError(expandEffects[i].richTextName + "富文本有误");
                            return null;
                        }
                        break;
                    }
                }
            }
            currentIndex++;
        }
        
        #if UNITY_EDITOR
        if(isDebugRichTextRange)
        {
            //打印出每个富文本的效果下标
            foreach (KeyValuePair<string,List<Vector2>> keyValuePair in richTextRangeDic)
            {
                string debugStr = keyValuePair.Key+"富文本下标范围 : ";
                foreach (Vector2 range in keyValuePair.Value)
                {
                    debugStr+="{"+range.x+","+range.y+"}\n";
                }
                Debug.Log(debugStr);
            }
        }
        #endif

        if (effectStack.Count!=0)
        {
            Debug.LogError("检查富文本内容是否有误");
        }
        return stringBuilder.ToString();
    }
    
    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="statusEnum">要切换状态</param>
    private void TransitionState(TMP_StatusEnum statusEnum)
    {
        currentState = states[statusEnum];
        currentState.OnEnter();
    }
}


