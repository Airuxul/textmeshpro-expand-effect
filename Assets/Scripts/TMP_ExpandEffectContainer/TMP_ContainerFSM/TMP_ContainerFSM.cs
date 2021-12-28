using UnityEngine;

/// <summary>
/// TMP_ExpandEffectContainer类内状态机部分
/// </summary>
partial class TMP_ExpandEffectContainer
{
  /// <summary>
  /// TMP拓展效果的状态枚举
  /// </summary>
  internal enum TMP_StatusEnum
  {
      /// <summary>
      /// 隐藏中
      /// </summary>
      Sleeping,
      /// <summary>
      /// 依次显示中
      /// </summary>
      Typing,
      /// <summary>
      /// 显示结束，所有的字都显示出来
      /// </summary>
      Completed
  }
  
  /// <summary>
  /// TMP拓展效果状态接口
  /// </summary>
  internal interface ITMPEEState
  {
      void OnEnter();
      void OnUpdate();
  }
  
  /// <summary>
  /// 隐藏状态
  /// </summary>
  internal class SleepingState:ITMPEEState
  {
      private readonly TMP_ExpandEffectContainer container;
      public SleepingState(TMP_ExpandEffectContainer _container)
      {
          container = _container;
      }
      public void OnEnter()
      {
          container.gameObject.SetActive(false);
          container.sleepAction?.Invoke(container);
      }
      public void OnUpdate() {}
  }
  
  /// <summary>
  /// 正在打印状态
  /// </summary>
  internal class TypingState : ITMPEEState
  {
      private readonly TMP_ExpandEffectContainer container;
      public TypingState(TMP_ExpandEffectContainer _container)
      {
          container = _container;
      }
      public void OnEnter()
      {
          container.gameObject.SetActive(true);
          container.expandTyping.StartTyping();
      }
      public void OnUpdate()
      {
          //ForceMeshUpdate()可以强制重新生成网格
          //如果没有该函数则会导致无法获取顶点坐标或者对顶点操作会一直累积
          container.m_TextComponent.ForceMeshUpdate();
          container.AllRichTextEffect();
          if (container.expandTyping.Typing())
          {
              container.TransitionState(TMP_StatusEnum.Completed);
          }
          
      }
  }
  
  /// <summary>
  /// 打印完成状态
  /// </summary>
  internal class CompletedState : ITMPEEState
  {
      private readonly TMP_ExpandEffectContainer container;
      private float afterCompletedTime;
      public CompletedState(TMP_ExpandEffectContainer _container)
      {
          container = _container;
      }
      public void OnEnter()
      {
          afterCompletedTime = 0;
          //完全显示
          container.m_TextComponent.color = new Color(1, 1, 1, 1);
          container.completedAction?.Invoke(container);
      }
      public void OnUpdate()
      {
          container.m_TextComponent.ForceMeshUpdate();
          container.AllRichTextEffect();
          if (container.completedSleepTime == -1)
          {
              return;
          }
          afterCompletedTime += Time.deltaTime;
          if (afterCompletedTime > container.completedSleepTime)
          {
              container.TransitionState(TMP_StatusEnum.Sleeping);
          }
      }
  }
}

