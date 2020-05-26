# ShellcodeLoader

将shellcode用rsa加密并动态编译exe，自带几种反沙箱技术。

**如果要用 .NET 2.0编译请手动编译，csc不支持某些代码**

## 反沙箱

* 判断父进程是否为Debugger

* 判断时间是否加速

一般沙盘内的程序时间都会加速。
* 判断进程是否有黑名单

判断是不是虚拟机，不过有几个vm进程是本机也会存在的，怕误报可以手动修改黑名单列表。
* 判断MAC地址是否有黑名单

判断是不是虚拟机的mac地址，有几个地址只要本机有装虚拟机也会有，怕误报可以手动修改黑名单列表。
* 判断系统盘是否大于50GB

一个正常PC的系统盘都会大于50GB。

---
## 使用

General：

运行生成的exe文件

Argument：

程序会保存加密好的数据到Output.txt

shellcode.exe "私钥" "加密数据"

---
## 截图

VirusTotal(12/72)：

![圖片](https://user-images.githubusercontent.com/40329078/82827231-28f20000-9ee1-11ea-80fd-c061dc5accf4.png)

常见的360、电脑管家、微软、卡巴斯基等都轻松免杀。

哈勃(未发现风险)：

![圖片](https://user-images.githubusercontent.com/40329078/82827419-943bd200-9ee1-11ea-88e5-18b03ae53701.png)

Hybrid Analysis(35/100)：

![圖片](https://user-images.githubusercontent.com/40329078/82829543-1928ea80-9ee6-11ea-8a66-bcfa2e6cdc3e.png)

程序：

![圖片](https://user-images.githubusercontent.com/40329078/82919051-1bec1400-9fa8-11ea-8855-e82f38e23489.png)

---
## 参考

[.NET for C# - RSA分段加解密](https://blog.xuite.net/ianan222/wretch/111888771-.NET+for+C%23+-+RSA%E5%88%86%E6%AE%B5%E5%8A%A0%E8%A7%A3%E5%AF%86)

[GhostShell](https://github.com/ReddyyZ/GhostShell)
