﻿using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace SendSMSHost.ViewModels
{
    public class SyncIndexViewModel
    {
        public string Prefix { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        public string Path { get; set; }

        public byte[] ImageByteArray
        {
            get
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode("The text which should be encoded.", QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);

                // Bitmap omzetten in byte[]
                byte[] ImageByteArray;
                using (var stream = new MemoryStream())
                {
                    qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    ImageByteArray = stream.ToArray();
                }

                return ImageByteArray;
            }
        }


    }
}