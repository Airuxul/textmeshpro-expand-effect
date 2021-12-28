using UnityEngine;

public class Test : MonoBehaviour
{
    public TMP_ExpandEffectContainer _Contianer;
    [TextArea]
    public string str; 
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            // _Contianer.ShowText(str,2, 
            //     (contianer)=>
            //     {
            //         Debug.Log("完全显示");
            //     },
            //     (container) =>
            //     {
            //         Debug.Log("隐藏文本");
            //     });
            _Contianer.ShowText(str);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _Contianer.Sleep();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            _Contianer.Completed();
        }
    }
}
