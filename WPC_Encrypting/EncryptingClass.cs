using System;
using System.Text;

namespace WPC_Encrypting
{
    public class EncryptingClass
    {
        public static string encryptString(string input)
        {
            if (input != String.Empty)
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(input);
                byte[] encryptedBytes = new byte[plainBytes.Length];
                for (int i = 0; i < plainBytes.Length; i++)
                {
                    encryptedBytes[i] = (byte)(plainBytes[i] + 1);
                }
                return Convert.ToBase64String(encryptedBytes);
            }
            else
            {
                return input;
            }
        }

        public static string[] encryptString(string[] input)
        {
            if(input.Length != 0)
            {
                for (int i = 0; i < input.Length; i++)
                {
                    encryptString(input[i]);
                }
                return input;
            }
            else
            {
                return null;
            }
        }
    }
}