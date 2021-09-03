using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Zen.Barcode;

namespace WebApplication1
{
    public class ZenBarcode
    {
        public class BarcodeData
        {
            private int _height { get; set; }
            private int _width { get; set; }
            private int _padding { get; set; }

            [Required]
            [Display(Name = "Barcode type")]
            public BarcodeSymbology type { get; set; }

            [Required]
            [StringLength(512)]
            [Display(Name = "Barcode value")]
            public string value { get; set; }

            [Required]
            [Range(50, 1000)]
            [Display(Name = "Width")]
            public int width
            {
                get
                {
                    if (_width < 50)
                        return 50;
                    else if (_width > 1000)
                        return 1000;
                    else
                        return _width;
                }
                set
                {
                    _width = value;
                }
            }

            [Required]
            [Range(30, 600)]
            [Display(Name = "Height")]
            public int height
            {
                get
                {
                    if (_height < 30)
                        return 30;
                    else if (_height > 600)
                        return 600;
                    else
                        return _height;
                }
                set
                {
                    _height = value;
                }
            }

            [Required]
            [Range(0, 250)]
            [Display(Name = "Padding")]
            public int padding
            {
                get
                {
                    if (_padding < 0)
                        return 0;
                    else if (_padding > 250)
                        return 250;
                    else
                        return _padding;
                }
                set
                {
                    _padding = value;
                }
            }

            [Display(Name = "Foreground color")]
            public string foregroundColor { get; set; }

            [Display(Name = "Background color")]
            public string backgroundColor { get; set; }

            [Display(Name = "Force width/height ratio")]
            public bool forceRatio { get; set; }


            public BarcodeData()
            {
                padding = 10;
            }
            public BarcodeData(BarcodeSymbology barcode_type, string barcode_value)
            {
                type = barcode_type;
                value = barcode_value;
                padding = 10;
            }


            //get all barcode types and define some properties like min and max length
            public List<Barcode> AllBarcodes()
            {
                return new List<Barcode>()
                {
                     new Barcode() {
                        minTextLength = 3,
                        maxTextLength = 32,
                        allowedContentType = ContentType.Numeric,
                        description = "11 C",
                        type = BarcodeSymbology.Code11C
                    },
                    new Barcode() {
                        minTextLength = 3,
                        maxTextLength = 32,
                        allowedContentType = ContentType.Numeric,
                        description = "11 NC",
                        type = BarcodeSymbology.Code11NC
                    },
                    new Barcode() {
                        minTextLength = 3,
                        maxTextLength = 32,
                        allowedContentType = ContentType.Numeric,
                        description = "25 Interleaved C",
                        type = BarcodeSymbology.Code25InterleavedC
                    },
                    new Barcode() {
                        minTextLength = 3,
                        maxTextLength = 32,
                        allowedContentType = ContentType.Numeric,
                        description = "25 Interleaved NC",
                        type = BarcodeSymbology.Code25InterleavedNC
                    },
                    new Barcode() {
                        minTextLength = 3,
                        maxTextLength = 32,
                        allowedContentType = ContentType.Numeric,
                        description = "25 Standard C",
                        type = BarcodeSymbology.Code25StandardC
                    },
                    new Barcode() {
                        minTextLength = 3,
                        maxTextLength = 32,
                        allowedContentType = ContentType.Numeric,
                        description = "25 Standard NC",
                        type = BarcodeSymbology.Code25StandardNC
                    },
                    new Barcode() {
                        minTextLength = 3,
                        maxTextLength = 32,
                        allowedContentType = ContentType.Numeric,
                        description = "39 C",
                        type = BarcodeSymbology.Code39C
                    },
                    new Barcode() {
                        minTextLength = 3,
                        maxTextLength = 64,
                        allowedContentType = ContentType.AlphaNumeric,
                        description = "39 NC",
                        type = BarcodeSymbology.Code39NC
                    },
                    new Barcode() {
                        minTextLength = 3,
                        maxTextLength = 32,
                        allowedContentType = ContentType.Numeric,
                        description = "93",
                        type = BarcodeSymbology.Code93
                    },
                    new Barcode() {
                        minTextLength = 3,
                        maxTextLength = 64,
                        allowedContentType = ContentType.AlphaNumeric,
                        description = "128",
                        type = BarcodeSymbology.Code128
                    },
                    new Barcode() {
                        minTextLength = 7,
                        maxTextLength = 7,
                        allowedContentType = ContentType.Numeric,
                        description = "EAN 8",
                        type = BarcodeSymbology.CodeEan8
                    },
                    new Barcode() {
                        minTextLength = 12,
                        maxTextLength = 12,
                        allowedContentType = ContentType.Numeric,
                        description = "EAN 13",
                        type = BarcodeSymbology.CodeEan13
                    },
                    new Barcode() {
                        minTextLength = 3,
                        maxTextLength = 512,
                        allowedContentType = ContentType.All,
                        description = "QR Code",
                        type = BarcodeSymbology.CodeQr
                    }
                };
            }


            //validate the input. some barcodes are numeric only, some alphanumeric etc
            public ContentError Validate()
            {
                var regex = new Regex("^[a-zA-Z0-9-]*$");
                var barcode = AllBarcodes().Where(x => x.type == type).FirstOrDefault();

                if (barcode == null || barcode.type == BarcodeSymbology.Unknown)
                    return ContentError.NoBarcodeType;
                else if (string.IsNullOrEmpty(value))
                    return ContentError.NoData;
                if (value.Length < barcode.minTextLength)
                    return ContentError.InputTooShort;
                else if (value.Length > barcode.maxTextLength)
                    return ContentError.InputTooLong;
                else if (barcode.allowedContentType == ContentType.Numeric && !value.All(char.IsDigit))
                    return ContentError.NotNumeric;
                else if (barcode.allowedContentType == ContentType.AlphaNumeric && !regex.IsMatch(value))
                    return ContentError.NotAlphaNumeric;
                else
                    return ContentError.None;
            }


            //generate the barcode
            public byte[] Generate()
            {
                BarcodeDraw barcodeDraw = BarcodeDrawFactory.GetSymbology(type);
                BarcodeMetrics barcodeMetrics = barcodeDraw.GetDefaultMetrics(height);

                //calculate the scale
                double schaal = Math.Ceiling((double)height / 100);
                if (schaal < 1)
                {
                    schaal = 1;
                }

                //when qr code double the scale
                if (type == BarcodeSymbology.CodeQr)
                {
                    schaal = schaal * 2;
                }

                //set the scale to the barcode
                barcodeMetrics.Scale = (int)schaal;

                //generate the barcode
                var image = barcodeDraw.Draw(value, barcodeMetrics);

                //make a bitmap from the image and make it transparent by default
                var barcodeImage = new Bitmap(image);
                barcodeImage.MakeTransparent(Color.White);

                Bitmap paddedImage = null;
                Bitmap filledImage = null;
                Bitmap coloredImage = null;

                //add the padding
                if (padding > 0)
                {
                    paddedImage = new Bitmap(barcodeImage.Width + (padding * 2), barcodeImage.Height + (padding * 2));
                    using (var gfx = Graphics.FromImage(paddedImage))
                    {
                        //also add the background color while were at it
                        if (!string.IsNullOrEmpty(backgroundColor))
                        {
                            gfx.Clear(ColorTranslator.FromHtml(backgroundColor));
                        }

                        //calculate the offset for the barcode image
                        int x = (paddedImage.Width - barcodeImage.Width) / 2;
                        int y = (paddedImage.Height - barcodeImage.Height) / 2;

                        gfx.DrawImage(barcodeImage, x, y);
                    }

                    barcodeImage = paddedImage;
                }
                else if (!string.IsNullOrEmpty(backgroundColor)) //add the background color
                {
                    filledImage = new Bitmap(barcodeImage.Width, barcodeImage.Height);
                    using (var gfx = Graphics.FromImage(filledImage))
                    {
                        gfx.Clear(ColorTranslator.FromHtml(backgroundColor));
                        gfx.DrawImage(barcodeImage, 0, 0);
                    }

                    barcodeImage = filledImage;
                }

                //replace the color of the barcode foreground with a new color. must be with this type of code since the library does not seem to be able to specify a color for the barcode
                if (!string.IsNullOrEmpty(backgroundColor))
                {
                    coloredImage = new Bitmap(barcodeImage.Width, barcodeImage.Height);
                    using (var gfx = Graphics.FromImage(coloredImage))
                    {
                        //make a color map
                        var colorMap = new ColorMap[1];
                        colorMap[0] = new ColorMap()
                        {
                            OldColor = Color.Black,
                            NewColor = ColorTranslator.FromHtml(foregroundColor)
                        };

                        //set the attributes
                        var attr = new ImageAttributes();
                        attr.SetRemapTable(colorMap);

                        //replace the color
                        Rectangle rect = new Rectangle(0, 0, barcodeImage.Width, barcodeImage.Height);
                        gfx.DrawImage(barcodeImage, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, attr);

                        barcodeImage = coloredImage;
                    }
                }

                //force ratio bij resizing the image except qr codes
                if (forceRatio && type != BarcodeSymbology.CodeQr)
                {
                    barcodeImage = Resize(barcodeImage, width, height);
                }

                //return the barcode as a bye array
                using (var stream = new MemoryStream())
                {
                    barcodeImage.Save(stream, ImageFormat.Png);

                    //cleanup
                    if (paddedImage != null)
                    {
                        paddedImage.Dispose();
                    }
                    if (filledImage != null)
                    {
                        filledImage.Dispose();
                    }
                    if (coloredImage != null)
                    {
                        coloredImage.Dispose();
                    }
                    barcodeImage.Dispose();

                    //return the barcode
                    return stream.ToArray();
                }
            }
        }


        private static Bitmap Resize(Bitmap image, int width, int height)
        {
            var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            using (image)
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.CompositingMode = CompositingMode.SourceCopy;
                gfx.CompositingQuality = CompositingQuality.HighQuality;
                gfx.SmoothingMode = SmoothingMode.HighQuality;
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;

                gfx.DrawImage(image, new Rectangle(0, 0, width, height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            }

            return bmp;
        }


        public class Barcode
        {
            public BarcodeSymbology type { get; set; }
            public string description { get; set; }
            public int minTextLength { get; set; }
            public int maxTextLength { get; set; }
            public ContentType allowedContentType { get; set; }
        }


        public enum ContentType
        {
            Numeric,
            AlphaNumeric,
            All
        }


        public enum ContentError
        {
            None,
            NoData,
            NoBarcodeType,
            InputTooShort,
            InputTooLong,
            NotNumeric,
            NotAlphaNumeric
        }
    }
}