# TextMeshProEffect说明
以TextMeshPro为基础的文字拓展效果，包含文字打印和自定义富文本效果

# 使用方法
## 脚本调用
场景example中有示例
场景中摄像机中有Test脚本中有使用操作，对TMP_ExpandEffectContainer的操作：
1. 开始显示文本（ShowText(string,float,Action1,Action1)）
  - string是包含富文本的字符串
  - float是完全显示后多久后隐藏文本，默认值为-1即不隐藏
  - Action1是完全显示回调函数
  - Action2是隐藏后回调函数
2. 切换至完全显示（Completed()）
3. 隐藏文本（Sleep()）

## 打印效果
在Project中右键Create/TMP_ExpandEffect/TMP_Typing中创建打印效果<br>
通过切换TMP_ExpandEffectContainer的ExpandTyping来实现打印效果的切换

## 自定义富文本效果
在Project中右键Create/TMP_ExpandEffect/TMP_RichText中创建富文本效果<br>
通过添加或者删除TMP_ExpandEffectContainer的RichText来添加或删除自定义富文本效果<br>
其中要注意富文本效果中的RichTextName定义了富文本标签<br>
如某富文本的RichTextName为1，则<1>abc</1>中abc会有该富文本效果

# 拓展效果
如要拓展打印效果可以创建类继承TMP_BaseTyping覆盖相应方法<br>
如要拓展自定义富文本效果可以创建类继承TMP_BaseRichText覆盖相应方法
