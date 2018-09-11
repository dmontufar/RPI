using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZXing;
using ZXing.Common;


namespace PI.Baktun.core
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlString GenerateRelayQrCode(this HtmlHelper html, string qrValue, int height = 250, int width = 250, int margin = 0)
        {
            //var qrValue = "whatever data you want to put in here";
            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = margin
                }
            };

            using (var bitmap = barcodeWriter.Write(qrValue))
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Gif);

                var img = new TagBuilder("img");
                img.MergeAttribute("alt", "sello");
                img.Attributes.Add("src", String.Format("data:image/gif;base64,{0}", Convert.ToBase64String(stream.ToArray())));

                return MvcHtmlString.Create(img.ToString(TagRenderMode.SelfClosing));
            }
        }


        public static IHtmlString GenerateRelayPDF417(this HtmlHelper html, string qrValue, int height = 100, int width = 250, int margin = 0)
        {
            //var qrValue = "whatever data you want to put in here";
            var barcodeWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.PDF_417,
                Options = new EncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = margin
                }
            };

            using (var bitmap = barcodeWriter.Write(qrValue))
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Gif);

                var img = new TagBuilder("img");
                img.MergeAttribute("alt", "firma");
                img.Attributes.Add("src", String.Format("data:image/gif;base64,{0}", Convert.ToBase64String(stream.ToArray())));

                return MvcHtmlString.Create(img.ToString(TagRenderMode.SelfClosing));
            }
        }
 
    }
}