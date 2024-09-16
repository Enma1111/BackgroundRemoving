// See https://aka.ms/new-console-template for more information
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;

string input = "C:/Users/Jan/Desktop/2028834250.jpg";
string output = "C:/Users/Jan/Desktop/2028834250cropped.jpg";
RemoveBackgroundándCrop(input, output);

static void RemoveBackgroundándCrop(string inputPath,string OutputPath)
{
    Mat img = CvInvoke.Imread(inputPath, Emgu.CV.CvEnum.ImreadModes.Color);

    Mat gray = new();
    CvInvoke.CvtColor(img, gray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

    Mat thresh = new();
    //CvInvoke.Threshold(gray, thresh, 200, 255, Emgu.CV.CvEnum.ThresholdType.BinaryInv);
    CvInvoke.AdaptiveThreshold(gray, thresh, 255,
        Emgu.CV.CvEnum.AdaptiveThresholdType.MeanC,
        Emgu.CV.CvEnum.ThresholdType.BinaryInv, 15, 10);

    using (VectorOfVectorOfPoint contours = new())
    {
        Mat hierarchy = new();
        CvInvoke.FindContours(thresh, contours, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

        double maxArea = 0;
        int maxIndex = -1;

        for (int i = 0; i < contours.Size; i++)
        {
            double area = CvInvoke.ContourArea(contours[i]);
            if (area > maxArea)
            {
                maxArea = area;
                maxIndex = i;
            }
        }

        if (maxIndex >= 0)
        {
            Rectangle rect = CvInvoke.BoundingRectangle(contours[maxIndex]);

            Mat cropped = new(img, rect);

            CvInvoke.Imwrite(OutputPath, cropped);

        }
    }
}