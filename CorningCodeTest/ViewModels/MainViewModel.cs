using System;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CorningCodeTest.Services;
using Emgu.CV;

namespace CorningCodeTest.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private bool _blur;
    [ObservableProperty] private int _blurValue;

    [ObservableProperty] private bool _canny;

    [ObservableProperty] private bool _dilate;
    [ObservableProperty] private int _dilateValue;

    [ObservableProperty] private bool _erode;
    [ObservableProperty] private int _erodeValue;

    private int[]? _histogramCounts = new int[256];
    [ObservableProperty] private Bitmap? _image;
    private ImageProcessing? _imageProcessing;

    [ObservableProperty] private bool _invert1;

    [ObservableProperty] private bool _invert2;

    [ObservableProperty] private bool _threshold;
    [ObservableProperty] private int _thresholdValue;


    public event Action<int[]>? HistogramUpdated;

    public void StartVideoFeed()
    {
        _imageProcessing = new ImageProcessing();
        _imageProcessing.FrameReady += OnFrameReady;
        _imageProcessing.GrayProcess += GrayProcessFrame;
        _imageProcessing.Start();
    }

    public void GrayProcessFrame(Mat frame)
    {
        if (_histogramCounts is null)
            return;
        _histogramCounts = _imageProcessing?.GrayHistogram(frame);

        HistogramUpdated?.Invoke(_histogramCounts!);
    }

    private void OnFrameReady(Mat frame)
    {
        var bmp = _imageProcessing?.MatToAvaBitmap(frame);
        Dispatcher.UIThread.Post(() =>
        {
            Image?.Dispose();
            Image = bmp;
        });
    }

    private void TogglePreGrayFilter(bool value, Action<Mat> filter)
    {
        if (_imageProcessing == null)
            return;
        if (value)
            _imageProcessing.PreGrayImageFilters += filter;
        else
            _imageProcessing.PreGrayImageFilters -= filter;
    }

    private void TogglePostGrayFilter(bool value, Action<Mat> filter)
    {
        if (_imageProcessing == null)
            return;
        if (value)
        {
            _imageProcessing.PostGrayImageFilters += filter;
            _imageProcessing.PostGrayFilters++;
        }
        else
        {
            _imageProcessing.PostGrayImageFilters -= filter;
            _imageProcessing.PostGrayFilters--;
        }
    }

    partial void OnInvert1Changed(bool value)
    {
        TogglePreGrayFilter(value, _imageProcessing!.InvertBgr2Hsv);
    }

    partial void OnInvert2Changed(bool value)
    {
        TogglePreGrayFilter(value, _imageProcessing!.InvertBgr2Rgb);
    }

    partial void OnBlurChanged(bool value)
    {
        TogglePreGrayFilter(value, _imageProcessing!.GaussianBlur);
    }

    partial void OnBlurValueChanged(int value)
    {
        _imageProcessing?.GuassianBlurSize = value;
    }

    partial void OnErodeChanged(bool value)
    {
        TogglePreGrayFilter(value, _imageProcessing!.Erode);
    }

    partial void OnErodeValueChanged(int value)
    {
        _imageProcessing?.ErodeIterations = value;
    }

    partial void OnDilateChanged(bool value)
    {
        TogglePreGrayFilter(value, _imageProcessing!.Dilate);
    }

    partial void OnDilateValueChanged(int value)
    {
        _imageProcessing?.DilateIterations = value;
    }

    partial void OnCannyChanged(bool value)
    {
        TogglePostGrayFilter(value, _imageProcessing!.CannyEdge);
    }

    partial void OnThresholdChanged(bool value)
    {
        TogglePostGrayFilter(value, _imageProcessing!.Threshold);
    }

    partial void OnThresholdValueChanged(int value)
    {
        _imageProcessing?.ThresholdValue = value;
    }
}