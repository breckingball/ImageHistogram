using System;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace CorningCodeTest.Services;

/* Image Processing Service
 * Provides ->
 *   Video Capture
 *   Image Filtering
 *      - Options: Invert1, Invert2, Blur, Erode, Dilate, Canny Edge
 *   Conversion from Mat to Avalonia.Bitmap
 */
public class ImageProcessing
{
    private readonly VideoCapture _capture;
    private readonly DispatcherTimer? _timer;

    public int DilateIterations = 1;
    public int ErodeIterations = 1;
    public int GuassianBlurSize = 1;
    public int PostGrayFilters = 0;
    public int ThresholdValue = 0;

    public ImageProcessing()
    {
        _capture = new VideoCapture();

        if (!_capture.IsOpened)
        {
            Console.WriteLine("Camera failed to open");
            return;
        }

        _capture.Set(CapProp.FrameWidth, 640);
        _capture.Set(CapProp.FrameHeight, 480);

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(33)
        };

        _timer.Tick += OnTick;
    }

    public void Start()
    {
        _timer?.Start();
    }

    public event Action<Mat>? PreGrayImageFilters;
    public event Action<Mat>? PostGrayImageFilters;
    public event Action<Mat>? GrayProcess;
    public event Action<Mat>? FrameReady;

    private void OnTick(object? sender, EventArgs e)
    {
        using var frame = new Mat();

        _capture.Read(frame);
        if (frame.IsEmpty) return;

        PreGrayImageFilters?.Invoke(frame);

        using var nonGrayscale = frame.Clone();

        GrayProcess?.Invoke(frame);

        PostGrayImageFilters?.Invoke(frame);

        FrameReady?.Invoke(PostGrayFilters > 0 ? frame : nonGrayscale);
    }

    public Bitmap MatToAvaBitmap(Mat mat)
    {
        using var bgra = new Mat();

        CvInvoke.CvtColor(mat, bgra, ColorConversion.Bgr2Bgra);

        var stride = bgra.Step;

        return new Bitmap(
            PixelFormat.Bgra8888,
            AlphaFormat.Unpremul,
            bgra.DataPointer,
            new PixelSize(bgra.Width, bgra.Height),
            new Vector(96, 96),
            stride);
    }

    public void InvertBgr2Hsv(Mat mat)
    {
        CvInvoke.CvtColor(mat, mat, ColorConversion.Bgr2Hsv);
    }

    public void GaussianBlur(Mat mat)
    {
        CvInvoke.GaussianBlur(mat, mat, new Size(GuassianBlurSize, GuassianBlurSize), 5);
    }

    public void InvertBgr2Rgb(Mat mat)
    {
        CvInvoke.CvtColor(mat, mat, ColorConversion.Bgr2Rgb);
    }

    public void Erode(Mat mat)
    {
        CvInvoke.Erode(mat, mat, null, new Point(-1, -1), ErodeIterations, BorderType.Reflect, new MCvScalar());
    }

    public void Dilate(Mat mat)
    {
        CvInvoke.Dilate(mat, mat, null, new Point(-1, -1), DilateIterations, BorderType.Reflect, new MCvScalar());
    }

    public void CannyEdge(Mat mat)
    {
        using var newMat = new Mat();
        CvInvoke.Canny(mat, newMat, 75, 175);
        newMat.CopyTo(mat);
    }

    public void Threshold(Mat mat)
    {
        CvInvoke.Threshold(mat, mat, ThresholdValue, 255, ThresholdType.Binary);
    }

    public int[] GrayHistogram(Mat mat)
    {
        CvInvoke.CvtColor(mat, mat, ColorConversion.Bgr2Gray);
        var pixelData = mat.GetRawData();
        var histCounts = new int[256];
        foreach (var pixel in pixelData)
            histCounts[pixel]++;
        return histCounts;
    }
}