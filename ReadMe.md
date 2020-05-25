# ShellcodeLoader

将shellcode用rsa加密并动态编译exe，自带几种反沙箱技术，可以将加密后的数据与私钥都改为网络加载，报毒率会更低。

**如果要用 .NET 2.0编译请手动编译，csc不支持某些代码**

## 反沙箱

* 判断父进程是否为Debugger

这个其实没什么用，.net的程序完全可以直接看逆向出来的代码，这个行为比较明显，建议删掉。
* 判断时间是否加速

一般沙盘内的程序时间都会加速。
* 判断进程是否有黑名单

判断是不是虚拟机，不过有几个vm进程是本机也会存在的，有需求可以删掉。
* 判断MAC地址是否有黑名单

也是判断虚拟机的mac地址，有几个本机也会有，有需求可以删掉。

---
## 截图

VirusTotal(12/72)：
![圖片](https://user-images.githubusercontent.com/40329078/82827231-28f20000-9ee1-11ea-80fd-c061dc5accf4.png)
常见的360、电脑管家、微软、卡巴斯基等都轻松免杀。

哈勃(未发现风险)：
![圖片](https://user-images.githubusercontent.com/40329078/82827419-943bd200-9ee1-11ea-88e5-18b03ae53701.png)

程序：
![圖片](https://user-images.githubusercontent.com/40329078/82827492-bdf4f900-9ee1-11ea-9f9f-fc4b047f03b5.png)
傻瓜式操作，选择raw格式shellcode一键处理。

---
## 参考

[.NET for C# - RSA分段加解密](https://blog.xuite.net/ianan222/wretch/111888771-.NET+for+C%23+-+RSA%E5%88%86%E6%AE%B5%E5%8A%A0%E8%A7%A3%E5%AF%86)

[GhostShell](https://github.com/ReddyyZ/GhostShell)