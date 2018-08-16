using System;
using System.Web;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// 此类已淘汰，里面方法暂留作参考，后续看删除此类。
/// </summary>
namespace NC.Common
{
    public interface IBindesh
    {
        string toEnc(string str);
        string toDec(string str);
    }

    public class Encrypt : IBindesh
    {
        private SymmetricAlgorithm mobjCryptoService;
        private static string _enc_key = "C97EC7AEEEC47DAB2C391ED3D6E1CFAE";

        #region ** 加密方式 **
        public enum SymmProvEnum : int
        {
            DES, RC2, Rijndael
        }
        #endregion


        /// <summary>
        /// 加密解密类
        /// </summary>
        /// <param name="ServiceProvider">加密方式枚举SymmProvEnum.DES/RC2/Rijndael</param>
        public Encrypt(SymmetricAlgorithm ServiceProvider)
        {
            mobjCryptoService = ServiceProvider;
        }

        public Encrypt(SymmProvEnum NetSelected)
        {
            switch (NetSelected)
            {
                case SymmProvEnum.DES:
                    mobjCryptoService = new DESCryptoServiceProvider();
                    break;
                case SymmProvEnum.RC2:
                    mobjCryptoService = new RC2CryptoServiceProvider();
                    break;
                case SymmProvEnum.Rijndael:
                    mobjCryptoService = new RijndaelManaged();
                    break;
            }
        }

        public Encrypt()
        {

        }

        public static Encrypt Instance
        {
            get
            {
                return new Encrypt();
            }
        }

        public static Encrypt DES
        {
            get
            {
                return new Encrypt(Encrypt.SymmProvEnum.DES);
            }
        }

        #region ** 获取密匙 **
        /// <summary>
        /// 获取密钥
        /// </summary>
        private byte[] GetLegalKey(string Key)
        {
            string sTemp = Key;
            mobjCryptoService.GenerateKey();

            byte[] bytTemp = mobjCryptoService.Key;
            int KeyLength = bytTemp.Length;

            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');

            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }

        /// <summary>
        /// 获得初始向量IV
        /// </summary>
        private byte[] GetLegalIV()
        {
            string sTemp = "A34C3D45B6018D3FD5560B103C2A00E2";  //allen 的 Md5
            mobjCryptoService.GenerateIV();

            byte[] bytTemp = mobjCryptoService.IV;
            int IVLength = bytTemp.Length;

            if (sTemp.Length > IVLength)
                sTemp = sTemp.Substring(0, IVLength);
            else if (sTemp.Length < IVLength)
                sTemp = sTemp.PadRight(IVLength, ' ');

            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        #endregion


        #region ** 可逆加密、解密 **
        /// <summary>
        /// 可逆加密
        /// </summary>
        public string ToEnc(string txt)
        {
            return Encrypting(txt, _enc_key);
        }
        public string Encrypting(string Source, string Key) //这里的Key需要修改，最好使用属性
        {
            try
            {
                byte[] bytIn = System.Text.ASCIIEncoding.ASCII.GetBytes(Source);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                byte[] bytKey = GetLegalKey(Key);

                mobjCryptoService.Key = bytKey;
                mobjCryptoService.IV = bytKey;
                ICryptoTransform encrypto = mobjCryptoService.CreateEncryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);

                cs.Write(bytIn, 0, bytIn.Length);
                cs.FlushFinalBlock();

                byte[] bytOut = ms.GetBuffer();
                int i = 0;
                for (i = 0; i < bytOut.Length; i++)
                    if (bytOut[i] == 0)
                        break;

                return System.Convert.ToBase64String(bytOut, 0, i);
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 可逆解密
        /// </summary>
        public string ToDec(string txt)
        {
            return Decrypting(txt, _enc_key);
        }
        public string Decrypting(string Source, string Key)
        {
            try
            {
                byte[] bytIn = System.Convert.FromBase64String(Source);
                System.IO.MemoryStream ms = new System.IO.MemoryStream(bytIn, 0, bytIn.Length);
                byte[] bytKey = GetLegalKey(Key);

                mobjCryptoService.Key = bytKey;
                mobjCryptoService.IV = bytKey;

                ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
                System.IO.StreamReader sr = new System.IO.StreamReader(cs);

                return sr.ReadToEnd();
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion


        #region ** 不可逆加密 **

        /// <summary>
        /// SHA1加密
        /// </summary>
        public static string SHA1(string s)
        {
            s += "GUN(B226!3E09F^5AAEA0$01F~8E55_4B29E-79C50==";
            byte[] data = Encoding.UTF8.GetBytes(s);
            byte[] result;
            SHA1 sha = new SHA1CryptoServiceProvider();
            // This is one implementation of the abstract class SHA1.
            result = sha.ComputeHash(data);

            string strResult = BitConverter.ToString(result);
            //BitConverter转换出来的字符串会在每个字符中间产生一个分隔符，需要去除掉
            strResult = strResult.Replace("-", "");
            return strResult;

            //return Security.FormsAuthentication.HashPasswordForStoringInConfigFile(s, "SHA1");//此方法nf4.5后淘汰
        }
        #endregion


        #region ** 无密钥的加密解密 **
        /// <summary>
        /// 字符串加密
        /// </summary>
        public static string Enc(string str)
        {
            try
            {
                char[] Base64Code = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '|', '/', '-' };
                byte empty = (byte)0;
                System.Collections.ArrayList byteMessage = new System.Collections.ArrayList(System.Text.Encoding.Default.GetBytes(str));
                System.Text.StringBuilder outmessage;
                int messageLen = byteMessage.Count;
                int page = messageLen / 3;
                int use = 0;
                if ((use = messageLen % 3) > 0)
                {
                    for (int i = 0; i < 3 - use; i++)
                        byteMessage.Add(empty);
                    page++;
                }
                outmessage = new System.Text.StringBuilder(page * 4);
                for (int i = 0; i < page; i++)
                {
                    byte[] instr = new byte[3];
                    instr[0] = (byte)byteMessage[i * 3];
                    instr[1] = (byte)byteMessage[i * 3 + 1];
                    instr[2] = (byte)byteMessage[i * 3 + 2];
                    int[] outstr = new int[4];
                    outstr[0] = instr[0] >> 2;

                    outstr[1] = ((instr[0] & 0x03) << 4) ^ (instr[1] >> 4);
                    if (!instr[1].Equals(empty))
                        outstr[2] = ((instr[1] & 0x0f) << 2) ^ (instr[2] >> 6);
                    else
                        outstr[2] = 64;
                    if (!instr[2].Equals(empty))
                        outstr[3] = (instr[2] & 0x3f);
                    else
                        outstr[3] = 64;
                    outmessage.Append(Base64Code[outstr[0]]);
                    outmessage.Append(Base64Code[outstr[1]]);
                    outmessage.Append(Base64Code[outstr[2]]);
                    outmessage.Append(Base64Code[outstr[3]]);
                }
                return outmessage.ToString();
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 字符串解密
        /// </summary>  
        public static string Dec(string str)
        {
            try
            {
                if ((str.Length % 4) != 0)
                {
                    throw new ArgumentException("不是正确的BASE64编码，请检查。", "str");
                }
                if (!System.Text.RegularExpressions.Regex.IsMatch(str, "^[A-Z0-9/|-]*$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    throw new ArgumentException("包含不正确的BASE64编码，请检查。", "str");
                }
                string Base64Code = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789|/-";
                int page = str.Length / 4;
                System.Collections.ArrayList outMessage = new System.Collections.ArrayList(page * 3);
                char[] message = str.ToCharArray();
                for (int i = 0; i < page; i++)
                {
                    byte[] instr = new byte[4];
                    instr[0] = (byte)Base64Code.IndexOf(message[i * 4]);
                    instr[1] = (byte)Base64Code.IndexOf(message[i * 4 + 1]);
                    instr[2] = (byte)Base64Code.IndexOf(message[i * 4 + 2]);
                    instr[3] = (byte)Base64Code.IndexOf(message[i * 4 + 3]);

                    byte[] outstr = new byte[3];
                    outstr[0] = (byte)((instr[0] << 2) ^ ((instr[1] & 0x30) >> 4));
                    if (instr[2] != 64) outstr[1] = (byte)((instr[1] << 4) ^ ((instr[2] & 0x3c) >> 2)); else outstr[2] = 0;
                    if (instr[3] != 64) outstr[2] = (byte)((instr[2] << 6) ^ instr[3]); else outstr[2] = 0;

                    outMessage.Add(outstr[0]);
                    if (outstr[1] != 0) outMessage.Add(outstr[1]);
                    if (outstr[2] != 0) outMessage.Add(outstr[2]);
                }

                byte[] outbyte = (byte[])outMessage.ToArray(Type.GetType("System.Byte"));
                return System.Text.Encoding.Default.GetString(outbyte);
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion


        #region
        public string toEnc(string str)
        {
            string htext = "";

            for (int i = 0; i < str.Length; i++)
            {
                htext = htext + (char)(str[i] + 10 - 1 * 2);
            }
            return htext;
        }

        public string toDec(string str)
        {
            string dtext = "";

            for (int i = 0; i < str.Length; i++)
            {
                dtext = dtext + (char)(str[i] - 10 + 1 * 2);
            }
            return dtext;
        }
        #endregion

    }
}
