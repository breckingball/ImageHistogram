using System.Drawing;
using CorningCodeTest.Services;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace CorningCodeTest.Tests.Services;

public class ImageProcessingTests
{
    /*
     * Tests the "invert" from BGR to HSV
     * checks that the output is not null and the number of channels are correct
     */
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

    /*
     * Tests the grayscale function, filter only
     * checks that the output is not null and the number of channels are correct
     */
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

    /*
     * Tests the threshold function
     * checks that the output is not null and the number of channels are correct
     * and each byte is 255 since every pixel was above the threshold
     */
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

    /*
     * Tests the histogram data generation from grayscale
     * checks that the output data is not null and that the collection
     * has 256 buckets and there is 10000 points in the one bucket of [150]
     */
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