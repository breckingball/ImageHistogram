using System.Drawing;
using CorningCodeTest.Services;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace CorningCodeTest.Tests.Services;

public class ImageProcessingTests
{
    [Fact]
    public void InvertBgr2HsvTest()
    {
        var ip = new ImageProcessing();
        var mat = new Mat(100, 100, DepthType.Cv8U, 3);
        CvInvoke.Rectangle(mat, new Rectangle(0, 0, 100, 100), new MCvScalar(0, 0, 255), -1);

        ip.InvertBgr2Hsv(mat);

        Assert.NotNull(mat);
        Assert.Equal(3, mat.NumberOfChannels);
    }

    [Fact]
    public void GrayScaleTest()
    {
        var ip = new ImageProcessing();
        var mat = new Mat(100, 100, DepthType.Cv8U, 3);
        CvInvoke.Rectangle(mat, new Rectangle(0, 0, 100, 100), new MCvScalar(0, 255, 255), -1);

        ip.GrayHistogram(mat);

        Assert.NotNull(mat);
        Assert.Equal(1, mat.NumberOfChannels);
    }

    [Fact]
    public void ThresholdTest()
    {
        var ip = new ImageProcessing();
        var mat = new Mat(100, 100, DepthType.Cv8U, 1);
        mat.SetTo(new MCvScalar(150));

        ip.ThresholdValue = 100;
        ip.Threshold(mat);

        Assert.NotNull(mat);
        Assert.Equal(1, mat.NumberOfChannels);
        foreach (byte pixel in mat.GetData()) Assert.Equal(255, pixel);
    }

    [Fact]
    public void HistogramTest()
    {
        var ip = new ImageProcessing();
        var mat = new Mat(100, 100, DepthType.Cv8U, 3);
        mat.SetTo(new MCvScalar(150, 150, 150));

        var grayCounts = ip.GrayHistogram(mat);

        Assert.NotNull(grayCounts);
        Assert.Equal(256, grayCounts.Length);
        Assert.Equal(100 * 100, grayCounts[150]);
    }
}