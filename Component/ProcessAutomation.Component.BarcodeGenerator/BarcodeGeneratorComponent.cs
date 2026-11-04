using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.DTO.Type;
using ProcessAutomation.Common.Service;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;
using ZXing.Windows.Compatibility;

namespace ProcessAutomation.Component
{
    public class BarcodeGeneratorComponent : TaskComponentBaseBC, ITaskComponent
    {
        public bool ConfigComponent(ComponentInfo componentInfo)
        {
            return true;
        }

        public void EndComponent()
        {
        }

        public bool InitComponent()
        {
            return true;
        }

        public Result ReadBarcode(string imagePath)
        {
            Result decodeResult = null;

#if WINDOWS
            var luminanceSource = (Func<Bitmap, LuminanceSource>)(bitmap => new BitmapLuminanceSource(bitmap));
            BarcodeReader<Bitmap> reader = new BarcodeReader<Bitmap>(null, luminanceSource, null);
            Bitmap barcodeBitmap = null;

            if (File.Exists(imagePath))
            {
                barcodeBitmap = (Bitmap)Image.FromFile(imagePath);
            }
            else
            {
                // LUIS VERA TOMARLA DE LA WEB mediante imagePath
            }

            if (barcodeBitmap != null)
            {
                reader.AutoRotate = true;
                reader.TryInverted = true;
                reader.Options = new DecodingOptions { TryHarder = true };


                decodeResult = reader.Decode(barcodeBitmap);
            }

            //if (result != null)
            //{
            //    txtDecoderType.Text = result.BarcodeFormat.ToString();
            //    txtDecoderContent.Text = result.Text;
            //}

#endif

            return decodeResult;
        }


        //public Result ReadBarcode(string imagePath)
        //{
        //    Result decodeResult = null;
        //    Bitmap image;

        //    if (File.Exists(imagePath))
        //    {
        //        image = (Bitmap)Bitmap.FromFile(imagePath);
        //        using (image)
        //        {
        //            LuminanceSource source;
        //            source = new BitmapLuminanceSource(image);
        //            BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
        //            var reader = new MultiFormatReader();


        //            //decodeResult = new MultiFormatReader().decode(bitmap);

        //            var hints = new Dictionary<DecodeHintType, object>();
        //            hints.Add(DecodeHintType.PURE_BARCODE, "TRUE");
        //            hints.Add(DecodeHintType.POSSIBLE_FORMATS, new List<BarcodeFormat>() { BarcodeFormat.QR_CODE, BarcodeFormat.CODABAR, BarcodeFormat.EAN_8, BarcodeFormat.EAN_13 });
        //            //reader.Hints = hints;

        //            //decodeResult = reader.decode(bitmap, hints);
        //            decodeResult = reader.decode(bitmap);
        //        }
        //    }

        //    return decodeResult;
        //}

        public Bitmap GenerateBarcode(BarcodeFormat barcodeType, string text, int width, int height, bool pureBarcode = false, string outputPath = null)
        {
            Bitmap bitmapResult = null;

#if WINDOWS
            var bw = new BarcodeWriter();

            var encOptions = new ZXing.Common.EncodingOptions
            {
                Width = width,
                Height = height,
                Margin = 0,
                PureBarcode = pureBarcode
            };


            // LUIS VERA FALTA REVISAR ESTE COMPONENT, DEJO DE FUNCIONAR AL ACTUALIZAR LOS NUGET PACKAGES

            encOptions.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);
            bw.Options = encOptions;
            bw.Format = barcodeType;
            bitmapResult = bw.Write(text);

            if (!string.IsNullOrEmpty(outputPath))
            {
                bitmapResult.Save(outputPath);
            }

            //bw.Renderer = new BitmapRenderer();
            //Bitmap bm = bw.Write(text);
            //Bitmap overlay = new Bitmap(@outputPath);

            //int deltaHeigth = bm.Height - overlay.Height;
            //int deltaWidth = bm.Width - overlay.Width;

            //Graphics g = Graphics.FromImage(bm);
            //g.DrawImage(overlay, new Point(deltaWidth / 2, deltaHeigth / 2));
#endif

            return bitmapResult;
        }

        public override IObjectType ProcessStep(RunProcessInfo runProcessInfo)
        {
            ObjectType resultValue = null;
            try
            {
                BarcodeDTO barcodeDTO = processBC.GetProcessedInputValue<BarcodeDTO>();

                if (barcodeDTO != null)
                {
                    string action = barcodeDTO.Action?.ToString();
                    string barcodeType = barcodeDTO.BarcodeType?.ToString();
                    string outputPath = barcodeDTO.OutputPath?.ToString();
                    string value = barcodeDTO.Value?.ToString();

                    switch (action)
                    {

                        case "READ":
                            {
                                Result resultBarcode = ReadBarcode(value);
                                if (resultBarcode != null)
                                {
                                    resultValue = resultBarcode.Text;
                                }
                            }
                            break;
                        case "WRITE":
                            {
                                //int width = barcodeDTO.Width == null ? 0 : Convert.ToInt32(barcodeDTO.Width);
                                //int height = barcodeDTO.Height == null ? 0 : Convert.ToInt32(barcodeDTO.Height);
                                int width = barcodeDTO.Width;
                                int height = barcodeDTO.Height;
                                bool onlyBarcode = barcodeDTO.OnlyBarcode; //Convert.ToBoolean(dynamicValue.OnlyBarcode);

                                resultValue = new ObjectType(GenerateBarcode((BarcodeFormat)Enum.Parse(typeof(BarcodeFormat), barcodeType, true), value, width, height, onlyBarcode, outputPath));
                            }
                            break;
                    }
                }

                StepSuccess();
            }
            catch (Exception ex)
            {
                StepFail(ex.ToString());
            }

            return resultValue;
        }
    }
}
