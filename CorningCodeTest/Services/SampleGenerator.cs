using Emgu.CV;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.Statistics;

namespace CorningCodeTest.Services;

public class SampleGenerator
{
    private readonly string? _filePath;
    private readonly ImageProcessing? _imageProcessing;

    public SampleGenerator(string path)
    {
        _imageProcessing = new ImageProcessing();
        _filePath = path;
        GenerateSamples();
    }

    private void GenerateSamples()
    {
        var plt = new Plot();
        var hist = Histogram.WithBinSize(1, 0, 255);
        var bars = plt.Add.Bars(hist.Bins, hist.Counts);
        bars.Color = new Color(200, 200, 200);

        plt.XLabel("Gray Value");
        plt.YLabel("Count");
        plt.Title("Grayscale Values");

        var mat = CvInvoke.Imread(_filePath);
        var frame = mat.Clone();
        _imageProcessing?.InvertBgr2Hsv(frame);
        CvInvoke.Imwrite("../SampleOutputs/Inverted1/Inverted1.png", frame);
        var counts = _imageProcessing?.GrayHistogram(frame);
        updateplot(counts, bars, plt);
        plt.SavePng("../SampleOutputs/Inverted1/Inverted1Plot.png", 600, 200);
        frame = mat.Clone();
        _imageProcessing?.InvertBgr2Rgb(frame);
        CvInvoke.Imwrite("../SampleOutputs/Inverted2/Inverted2.png", frame);
        counts = _imageProcessing?.GrayHistogram(frame);
        updateplot(counts, bars, plt);
        plt.SavePng("../SampleOutputs/Inverted2/Inverted2Plot.png", 600, 200);

        frame = mat.Clone();
        _imageProcessing.GuassianBlurSize = 11;
        _imageProcessing?.GaussianBlur(frame);
        CvInvoke.Imwrite("../SampleOutputs/Blurred/Blurred.png", frame);
        counts = _imageProcessing?.GrayHistogram(frame);
        updateplot(counts, bars, plt);
        plt.SavePng("../SampleOutputs/Blurred/BlurredPlot.png", 600, 200);

        frame = mat.Clone();
        counts = _imageProcessing?.GrayHistogram(frame);
        CvInvoke.Imwrite("../SampleOutputs/Grayscale/Grayscale.png", frame);
        updateplot(counts, bars, plt);
        plt.SavePng("../SampleOutputs/Grayscale/GrayscalePlot.png", 600, 200);

        frame = mat.Clone();
        counts = _imageProcessing?.GrayHistogram(frame);
        _imageProcessing?.CannyEdge(frame);
        CvInvoke.Imwrite("../SampleOutputs/Canny/Canny.png", frame);

        updateplot(counts, bars, plt);
        plt.SavePng("../SampleOutputs/Canny/CannyPlot.png", 600, 200);
    }

    private void updateplot(int[] counts, BarPlot blt, Plot plt)
    {
        for (var i = 0; i < blt?.Bars.Count; i++) blt.Bars[i].Value = counts[i];
        plt.Axes.AutoScaleY();
    }
}