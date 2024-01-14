using System;
using System.Text;

namespace WPC_Encrypting
{
    public class DecryptingClass
    {
        public static string decryptString(string text)
        {
            if(text != String.Empty)
            {
                byte[] encryptedBytes = Convert.FromBase64String(text);
                byte[] decryptedBytes = new byte[encryptedBytes.Length];
                for(int i = 0; i < encryptedBytes.Length; i++)
                {
                    decryptedBytes[i] = (byte)(encryptedBytes[i] - 1);
                }
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            else
            {
                return text;
            }
        }

        public static string[] decryptString(string[] text)
        {
            if(text.Length != 0)
            {
                for(int i = 0; i < text.Length; i++)
                {
                    decryptString(text[i]);
                }
                return text;
            }
            else
            {
                return null;
            }
        }
    }
}
