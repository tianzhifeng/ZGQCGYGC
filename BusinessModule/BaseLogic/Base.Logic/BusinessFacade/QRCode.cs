using Formula;
using Formula.Helper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace Base.Logic.BusinessFacade
{
    public static class QRCode
    {
        static List<string> SourceImage = new List<string>();
        #region QR二维码

        public static void NewSourceImage()
        {
            SourceImage = new List<string>();
        }

        private static System.Drawing.Image resizeImage(System.Drawing.Image imgToResize, Size size)
        {
            //获取图片宽度
            int sourceWidth = imgToResize.Width;
            //获取图片高度
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            //计算宽度的缩放比例
            nPercentW = ((float)size.Width / (float)sourceWidth);
            //计算高度的缩放比例
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            //期望的宽度
            int destWidth = (int)(sourceWidth * nPercent);
            //期望的高度
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //绘制图像
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return (System.Drawing.Image)b;
        }

        private static System.Drawing.Image CreateImage(string text, string correctionLevel = "M")
        {
            var encoder = new QRCodeEncoder();
            String encoding = text;
            if (encoding == "Byte")
            {
                encoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            }
            else if (encoding == "AlphaNumeric")
            {
                encoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.ALPHA_NUMERIC;
            }
            else if (encoding == "Numeric")
            {
                encoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.NUMERIC;
            }
            try
            {
                int scale = Convert.ToInt16(text.Length);
                encoder.QRCodeScale = scale;
            }
            catch (Exception ex)
            {
                throw new Exception("大小参数错误!");
            }
            try
            {
                int version = 8;
                encoder.QRCodeVersion = version;
            }
            catch (Exception ex)
            {
                throw new Exception("版本参数错误 !");
            }


            if (correctionLevel == "L")
                encoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.L;
            else if (correctionLevel == "M")
                encoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            else if (correctionLevel == "Q")
                encoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
            else if (correctionLevel == "H")
                encoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;

            String data = text;
            System.Drawing.Image image = encoder.Encode(data, System.Text.Encoding.UTF8);
            return image;
        }


        public static string CodeDecoder(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                return null;
            Bitmap myBitmap = new Bitmap(System.Drawing.Image.FromFile(filePath));
            QRCodeDecoder decoder = new QRCodeDecoder();
            string decodedString = decoder.decode(new QRCodeBitmapImage(myBitmap));
            return decodedString;
        }
        #endregion

        #region 生成带图片的二维码

        /// <summary>
        ///     合并图片
        /// </summary>
        /// <param name="imgBack"></param>
        /// <param name="img"></param>
        /// <returns></returns>
        public static System.Drawing.Image CombinImage(System.Drawing.Image imgBack, System.Drawing.Image img)
        {
            if (img.Height != 50 || img.Width != 50)
            {
                img = ResizeImage(img, 50, 50, 0);
            }
            Graphics g = Graphics.FromImage(imgBack);

            g.DrawImage(imgBack, 0, 0, imgBack.Width, imgBack.Height); //g.DrawImage(imgBack, 0, 0, 相框宽, 相框高);   

            //g.FillRectangle(System.Drawing.Brushes.White, imgBack.Width / 2 - img.Width / 2 - 1, imgBack.Width / 2 - img.Width / 2 - 1,1,1);//相片四周刷一层黑色边框  

            //g.DrawImage(img, 照片与相框的左边距, 照片与相框的上边距, 照片宽, 照片高);  

            g.DrawImage(img, imgBack.Width / 2 - img.Width / 2, imgBack.Width / 2 - img.Width / 2, img.Width, img.Height);
            GC.Collect();
            return imgBack;
        }

        /// <summary>
        ///     调用此函数后使此两种图片合并，类似相册，有个
        ///     背景图，中间贴自己的目标图片
        /// </summary>
        /// <param name="imgBack">粘贴的源图片</param>
        /// <param name="destImg">粘贴的目标图片</param>
        public static System.Drawing.Image CombinImage(System.Drawing.Image imgBack, string destImg)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(destImg);
            return CombinImage(imgBack, img);
        }

        /// <summary>
        ///     Resize图片
        /// </summary>
        /// <param name="bmp">原始Bitmap</param>
        /// <param name="newW">新的宽度</param>
        /// <param name="newH">新的高度</param>
        /// <param name="mode">保留着，暂时未用</param>
        /// <returns>处理以后的图片</returns>
        public static System.Drawing.Image ResizeImage(System.Drawing.Image bmp, int newW, int newH, int mode)
        {
            try
            {
                System.Drawing.Image b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);

                // 插值算法的质量  
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new System.Drawing.Rectangle(0, 0, newW, newH), new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                            GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region 生成二维码
        public static void CreateQRCode(string folder,Dictionary<string, string> dic, string logoPath, bool isImg = false, bool isRQ = false)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            if(dic.Count > 0)
            {
                string text = Convert.ToString(dic["Text"]);
                string title = Convert.ToString(dic["Title"]);
                System.Drawing.Image image = null;
                if (isRQ)
                {
                    image = CreateImage(text);
                    if (isImg)
                    {
                        System.Drawing.Image logo = System.Drawing.Image.FromFile(logoPath);
                        image = CombinImage(image, logo);
                    }
                }
                else
                {
                    int cType = 0; object cObject = null;
                    if (isImg)
                    {
                        cType = 1;
                        cObject = System.Drawing.Image.FromFile(logoPath);
                    }
                    else
                    {
                        cObject = title.Trim();
                    }
                    if (!string.IsNullOrEmpty(text))
                        image = GenerateQrWithTitleText(
                           text,
                           title,
                           cType, cObject, 512, string.IsNullOrEmpty(title) ? 512 : 760);

                    //image = GenerateQrWithCentreImage(text, System.Drawing.Image.FromFile(logoPath));
                }
                if (image != null)
                {
                    Bitmap bitmap = new Bitmap(image);
                    image = resizeImage(bitmap, new Size(135, 150));
                    string filePath = folder + dic["ID"] + ".jpg";
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                    if (!SourceImage.Contains(filePath))
                        SourceImage.Add(filePath);
                    System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                    image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                    fs.Close();
                    image.Dispose();
                }
            }

        }
        #endregion

        #region 导出PDF

        //固定高宽，便于后续处理
        const int WWidth = 600;
        const int HHeight = 800;
        private static float GetY(int i)
        {
            int c = 0;

            if ((i + 1) / 5 > 0 && (i + 1) % 5 != 0)
            {
                c = (i + 1) / 5 + 1;
            }
            else if ((i + 1) / 5 > 0 && (i + 1) % 5 == 0)
            {
                c = (i + 1) / 5;
            }
            else
            {
                c = (i + 1) / 5 + 1;
            }

            if (c / 8 > 0)
            {
                if ((i % 40 + 1) / 5 > 0 && (i % 40 + 1) % 5 != 0)
                {
                    c = (i % 40 + 1) / 5 + 1;
                }
                else if ((i % 40 + 1) / 5 > 0 && (i % 40 + 1) % 5 == 0)
                {
                    c = (i % 40 + 1) / 5;
                }
                else
                {
                    c = (i % 40 + 1) / 5 + 1;
                }
            }

            return HHeight - c * 100;
        }
       
        public static void TurnTheImageToPdf(string fileName)
        {
            if (SourceImage.Count > 0)
            {
                Document document = new Document();
                document.SetPageSize(new iTextSharp.text.Rectangle(WWidth, HHeight));
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(fileName, FileMode.Create));
                document.Open();
                iTextSharp.text.Image jpg;
                for (int i = 0; i < SourceImage.Count; ++i)
                {
                    jpg = iTextSharp.text.Image.GetInstance(SourceImage[i].ToString());
                    jpg.ScaleToFit(100, 100);

                    jpg.SetAbsolutePosition((i + 1) % 5 == 1 ? 15 : (i + 1) % 5 == 2 ? 135 : (i + 1) % 5 == 3 ? 255 : (i + 1) % 5 == 4 ? 375 : 495,
                        GetY(i));

                    document.Add(jpg);
                    if ((i + 1) % 40 == 0)
                        document.NewPage();
                }
                if (document != null && document.IsOpen())
                {
                    document.Close();
                }
            }
        }
      
        #endregion


        #region zxing二维码
        static ZXing.BarcodeWriter WT;
        public static System.Drawing.Color FirstColor = System.Drawing.Color.FromArgb(210, 210, 114, 108);
        public static System.Drawing.Color FontColor = System.Drawing.Color.FromArgb((int)byte.MaxValue, 58, 178, 194);
        public static System.Drawing.Color StartingColor = System.Drawing.Color.FromArgb(170, 18, 69, 160);
        public static System.Drawing.Color EndingColor = System.Drawing.Color.FromArgb((int)byte.MaxValue, 148, 161, 99);

        private static bool initedWriter = false;
  
        private static void InitWriter()
        {
            WT = new ZXing.BarcodeWriter();
            WT.Format = ZXing.BarcodeFormat.QR_CODE;
            var options = new QrCodeEncodingOptions
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.H,
            };
            WT.Options = options;

            initedWriter = true;
        }        

        public static Bitmap GenerateSimple(string strCode, int width = 512, int height = 512)
        {
            if(initedWriter==false )
            {
                InitWriter();
            }

            WT.Options.Width = width;
            WT.Options.Height = height;   
                            
            return WT.Write(strCode);
        }

    
        public static Bitmap GenerateQrWithCentreImage(string strCode, System.Drawing.Image centreImg, int width = 512, int height = 512)
        {
            if (string.IsNullOrEmpty(strCode))
            {
                return null;
            }
            if (centreImg == null)
            {
                return GenerateSimple(strCode, width, height);
            }


            MultiFormatWriter mutiWriter = new MultiFormatWriter();
            Dictionary<EncodeHintType, object> hint = new Dictionary<EncodeHintType, object>();
            hint.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            hint.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);


            BitMatrix bm = mutiWriter.encode(strCode, BarcodeFormat.QR_CODE, width, height, hint);
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            Bitmap bitmap = barcodeWriter.Write(bm);

            int[] rectangle = bm.getEnclosingRectangle();

            int middleImgW = Math.Min((int)(rectangle[2] / 3.5), centreImg.Width);
            int middleImgH = Math.Min((int)(rectangle[3] / 3.5), centreImg.Height);
            int middleImgL = (bitmap.Width - middleImgW) / 2;
            int middleImgT = (bitmap.Height - middleImgH) / 2;

            Bitmap bmpimg = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmpimg))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.DrawImage(bitmap, 0, 0);
            }

            Graphics myGraphic = Graphics.FromImage(bmpimg);
  
            myGraphic.FillRectangle(Brushes.White, middleImgL, middleImgT, middleImgW, middleImgH);
            myGraphic.DrawImage(centreImg, middleImgL, middleImgT, middleImgW, middleImgH);
            return bmpimg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strCode"></param>
        /// <param name="title"></param>
        /// <param name="centreType">0:string 1:Image</param>
        /// <param name="centreObject">string or Image</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap GenerateQrWithTitleText(string strCode, string title, int centreType, object centreObject, int width = 512, int height = 512)
        {
            MultiFormatWriter mutiWriter = new MultiFormatWriter();
            Dictionary<EncodeHintType, object> hint = new Dictionary<EncodeHintType, object>();
            hint.Add(EncodeHintType.CHARACTER_SET, "UTF-8");
            hint.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
      
            BitMatrix bm = mutiWriter.encode(strCode, BarcodeFormat.QR_CODE, width, height, hint);
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            Bitmap oriBitmap = barcodeWriter.Write(bm);

            Bitmap result = new Bitmap(oriBitmap.Width, oriBitmap.Height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage((System.Drawing.Image)result);
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphics.DrawImage(oriBitmap, 0, 0);

            DrawTitleStr(graphics, title, result.Width, result.Height);

            if (centreType == 0)
            {
                DrawCentreStr(graphics, centreObject, result.Width, result.Height);
            }
            else
            {
                DrawCentreImg(
                    graphics, 
                    centreObject, 
                    bm.getEnclosingRectangle(), 
                    oriBitmap.Width, oriBitmap.Height
                    );
            }

            SetColorFull(result, strCode);

            return result;
        }

        private static void DrawTitleStr(Graphics graphics, string title, int w, int h)
        {
            if (string.IsNullOrEmpty(title))
            {
                return;
            }

            Brush recBrush = (Brush)new SolidBrush(System.Drawing.Color.White);
            Brush fontBrush = (Brush)new SolidBrush(FontColor);

            System.Drawing.Font fontTitle = new System.Drawing.Font("Arial",  (float)30, FontStyle.Regular);
            SizeF sizeF2 = graphics.MeasureString(title, fontTitle);
            float num4 = (float)(((double)w - (double)sizeF2.Width) / 2.0);

            //int y1 = 52;
            //graphics.FillRectangle(recBrush, new System.Drawing.Rectangle((int)num4, (int)y1, (int)sizeF2.Width, (int)sizeF2.Height));
            RectangleF rec = new RectangleF(0, h - 90, w, h);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            graphics.DrawString(title, fontTitle, fontBrush, rec, sf);
        }

        private static void DrawCentreStr(Graphics graphics, object centreObject, int w, int h)
        {
            string centreStr = centreObject as string;
            if (!string.IsNullOrEmpty(centreStr))
            {
                Brush fontBrush = (Brush)new SolidBrush(FontColor);

                System.Drawing.Font fontCentre = new System.Drawing.Font("Arial", 32f - (float)(centreStr.Length - 4) * 3.5f, FontStyle.Bold);
                SizeF sizeFont = graphics.MeasureString(centreStr, fontCentre);

                float x = (float)(((double)w - (double)sizeFont.Width) / 2.0 );
                float y = (float)(((double)h - (double)sizeFont.Height) / 2.0 );

                graphics.FillRectangle(fontBrush, x-5, y-5, sizeFont.Width+10, sizeFont.Height+10);
                graphics.FillRectangle(Brushes.White, x, y, sizeFont.Width, sizeFont.Height);

                graphics.DrawString(centreStr, fontCentre, fontBrush, x, y);

                graphics.Dispose();
            }
        }

        private static void DrawCentreImg(Graphics graphics,  object centreObject, int[] rectangle,int w,int h)
        {
            System.Drawing.Image centreImg = centreObject as System.Drawing.Image;
            if (centreImg != null)
            {
                int imgW = Math.Min((int)(rectangle[2] / 3.5), centreImg.Width);
                int imgH = Math.Min((int)(rectangle[3] / 3.5), centreImg.Height);
                int imgL = (w - imgW) / 2;
                int imgT = (h - imgH) / 2;

                graphics.FillRectangle(Brushes.White, imgL, imgT, imgW, imgH);
                graphics.DrawImage(centreImg, imgL, imgT, imgW, imgH);
                graphics.Dispose();
            }
        }

        private static void SetColorFull(Bitmap source,string strCode)
        {
            Bitmap colorFullBitmap = GetSPBMP(source.Width, source.Height);
            int num1 = 140;
            try
            {
                num1 -= (Encoding.UTF8.GetBytes(strCode).Length - 20) / 2;
            }
            catch { }

            int num2 = num1;
            int num3 = num2;
            for (int x = 0; x < source.Width; ++x)
            {
                for (int y = 0; y < source.Height; ++y)
                {
                    System.Drawing.Color pixel = source.GetPixel(x, y);
                    System.Drawing.Color newColor = x >= num2 || y >= num3 ? ((int)pixel.A != (int)byte.MaxValue || (int)pixel.B != 0 ? pixel : colorFullBitmap.GetPixel(x, y)) : ((int)pixel.A != (int)byte.MaxValue || (int)pixel.B != 0 ? pixel : FirstColor);
                    source.SetPixel(x, y, newColor);
                }
            }
        }

        private static  Bitmap GetSPBMP(int w, int h)
        {
            Bitmap bitmap = new Bitmap(w, h, PixelFormat.Format32bppArgb);           
            using (Graphics graphics = Graphics.FromImage((System.Drawing.Image)bitmap))
            {
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, w, h);
                LinearGradientBrush linearGradientBrush = new LinearGradientBrush(rect, 
                    StartingColor, 
                    EndingColor, 
                    LinearGradientMode.Horizontal);

                graphics.FillRectangle((Brush)linearGradientBrush, rect);
            }

            return bitmap;
        }

        #endregion
    }
}
