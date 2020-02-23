using MvcAdapter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ThoughtWorks.QRCode.Codec;

namespace MvcConfig.Controllers
{
    public class QrCodeController : Controller
    {
        public ActionResult Get(string id, string type)
        {
            var ip = System.Configuration.ConfigurationManager.AppSettings["WeChatIP"];
            var key = System.Configuration.ConfigurationManager.AppSettings["QrCodeKey"];
            var content = type + "|" + id;
            content = DesEncrypt(content, GetLegalKey(key));

            ip = (ip.StartsWith("http://") || ip.StartsWith("https://")) ? ip : "http://" + ip;
            content = (ip.EndsWith("/") ? ip : ip + "/") + "QrCode/Show?k=" + content;
            content = content.Replace("+", "%2b");
            int strLength = Encoding.GetEncoding("UTF-8").GetByteCount(content);
            
            ImageActionResult result = null;

            QRCodeEncoder endocder = new QRCodeEncoder();
            //二维码背景颜色
            endocder.QRCodeBackgroundColor = System.Drawing.Color.White;
            //二维码编码方式
            endocder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            //每个小方格的宽度
            endocder.QRCodeScale = 3;
            //二维码版本号
            endocder.QRCodeVersion = 11;

            //纠错等级
            endocder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;

            Bitmap bitmap = endocder.Encode(content, System.Text.Encoding.UTF8);

            result = new ImageActionResult(bitmap, ImageFormat.Bmp);
            return result;
        }

        //加密
        private static string DesEncrypt(string encryptString, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 8));
            byte[] keyIV = keyBytes;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, provider.CreateEncryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }

        //解密
        private static string DesDecrypt(string decryptString, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key.Substring(0, 8));
            byte[] keyIV = keyBytes;
            byte[] inputByteArray = Convert.FromBase64String(decryptString);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, provider.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }

        private static string GetLegalKey(string key)
        {
            if (key.Length < 8)
                key = key.PadRight(8, ' ');
            if (key.Length > 8)
                key = key.Substring(0, 8);
            return key;
        }
    }
}
