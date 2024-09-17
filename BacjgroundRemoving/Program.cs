// See https://aka.ms/new-console-template for more information
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;

string inputFolder = "C:/Users/info_894bsva/Desktop/TestBilder/";
string outputFolder = "C:/Users/info_894bsva/Desktop/TestBilderErgebnis/";

if (!Directory.Exists(outputFolder))
{
    Directory.CreateDirectory(outputFolder);
}

string[] inputPaths = Directory.GetFiles(inputFolder, "*.jpg");

foreach (string input in inputPaths)
{
    string fileName = Path.GetFileNameWithoutExtension(input);
    
    string outputFileName = fileName + "_cropped.jpg";
    
    string outputPath = Path.Combine(outputFolder, outputFileName);
    
    RemoveBackgroundAndCrop(input, outputPath);
}


static void RemoveBackgroundAndCrop(string inputPath,string outputPath)
{
    //Lesen des Bildes in Farbe
    Mat img = CvInvoke.Imread(inputPath, ImreadModes.Color);

    //Umwandeln des Bildes in Graustufen
    Mat gray = new Mat();
    CvInvoke.CvtColor(img, gray, ColorConversion.Bgr2Gray);

    //Hindergrundrauschen verringern
    CvInvoke.GaussianBlur(gray, gray, new Size(5, 5), 0);

    
    Mat thresh = new Mat();
    //Nicht Adaptive Schwellwertbestimmung
    //CvInvoke.Threshold(gray, thresh, 200, 255, Emgu.CV.CvEnum.ThresholdType.BinaryInv);

    //AdaptiveThresholdType Schwellwert bestimmung
    CvInvoke.AdaptiveThreshold(gray, thresh, 255, AdaptiveThresholdType.MeanC, ThresholdType.BinaryInv, 15, 10);


    using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
    {
        //Suche nach Konturen im Binären Bild
        Mat hierarchy = new Mat();
        CvInvoke.FindContours(thresh, contours, hierarchy, RetrType.External, ChainApproxMethod.ChainApproxSimple);

        //Berechnen des Bounding Rectangles
        Rectangle boundingRect = Rectangle.Empty;
        for (int i = 0; i < contours.Size; i++)
        {
            Rectangle contourRect = CvInvoke.BoundingRectangle(contours[i]);

            //boundingRect Rectangles verbinden
            if (boundingRect == Rectangle.Empty)
            {
                boundingRect = contourRect;
            }
            else
            {
                boundingRect = Rectangle.Union(boundingRect, contourRect);
            }
        }

        //Prüfung ob  boundingRect nicht leer ist
        if (!boundingRect.IsEmpty)
        {
            //Ausschneiden des Bereichs aus dem original Bild
            Mat cropped = new Mat(img, boundingRect);

            
            CvInvoke.Imwrite(outputPath, cropped);
        }
    }
}