# NCToools

目前可检查出的错误有：
* 重复铆接错误
* 紧固件类型、数量异常错误
* ProcessFeature名称与实际类型不一致错误

能够自动修复的错误：
* 换刀M56 T代码输出异常bug
* 去除After点位输出N/A
* 强制校准输出校准T代码异常bug

其他修饰：
* 自动加入NC机器识别代号：”O”+图号后八位+程序段顺序号
* 程序段首尾加入该程序段开始结束提示。
* 自动转化为机器运行代码后的格式，方便用UltraCompare比对修改

