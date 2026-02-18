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
 *      - Options: Invert1, Invert2, Blur, Erode, Dilate, Canny Edge, Threshold
 *   Conversion from Mat to Avalonia.Bitmap
 */
public class ImageProcessing
{
    private readonly DispatcherTimer? _timer;
    private VideoCapture? _capture;
    private Mat? _lastMat = new();
    public bool CameraEnabled = true;
    public int DilateIterations = 1;
    public int ErodeIterations = 1;
    public int GuassianBlurSize = 1;
    public int PostGrayFilters = 0;
    public int ThresholdValue = 0;

    public ImageProcessing()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(30)
        };

        _timer.Tick += OnTick;
    }


    public void Start()
    {
        if (_capture != null) return;

        _capture = new VideoCapture();

        if (!_capture.IsOpened)
        {
            Console.WriteLine("Camera failed to open");
            _capture.Dispose();
            _capture = null;
            return;
        }

        _capture.Set(CapProp.FrameWidth, 640);
        _capture.Set(CapProp.FrameHeight, 480);

        _timer?.Start();
    }

    public void Stop()
    {
        _capture?.Dispose();
        _lastMat?.Dispose();
        _timer?.Stop();
    }

    public event Action<Mat>? PreGrayImageFilters;
    public event Action<Mat>? PostGrayImageFilters;
    public event Action<Mat>? GrayProcess;
    public event Action<bool, Mat>? FrameReady;

/* Every Tick, check if the camera is enabled
 * if it is, capture the new frame and send it to the liveview feed
 * if not, use the last available frame
 * apply filters before grayscaling
 * send the gray data to the histogram
 * apply filters after grayscaling
 * send filtered frame to the filtered feed
 */
    private void OnTick(object? sender, EventArgs e)
    {
        Mat frame;
        if (CameraEnabled && _capture is not null)
        {
            frame = new Mat();
            _capture.Read(frame);
            if (frame.IsEmpty)
            {
                frame.Dispose();
                return;
            }

            _lastMat?.Dispose();
            _lastMat = frame.Clone();

            FrameReady?.Invoke(true, frame);
        }
        else
        {
            if (_lastMat == null) return;
            frame = _lastMat.Clone();
        }

        PreGrayImageFilters?.Invoke(frame);

        using var nonGrayscale = frame.Clone();

        GrayProcess?.Invoke(frame);

        PostGrayImageFilters?.Invoke(frame);

        FrameReady?.Invoke(false, PostGrayFilters > 0 ? frame : nonGrayscale);

        frame.Dispose();
    }

    public Bitmap MatToAvaBitmap(Mat mat)
    {
        using var bmt = new Mat();

        CvInvoke.CvtColor(mat, bmt, ColorConversion.Bgr2Bgra);

        return new Bitmap(
            PixelFormat.Bgra8888,
            AlphaFormat.Unpremul,
            bmt.DataPointer,
            new PixelSize(bmt.Width, bmt.Height),
            new Vector(96, 96),
            bmt.Step);
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