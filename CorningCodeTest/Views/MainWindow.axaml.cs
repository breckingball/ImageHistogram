using System;
using Avalonia.Controls;
using CorningCodeTest.ViewModels;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.Statistics;

namespace CorningCodeTest.Views;

public partial class MainWindow : Window
{
    private BarPlot? _bars;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();

        Opened += MainWindow_Opened;

        InitializePlot();
    }

    private void InitializePlot()
    {
        var hist = Histogram.WithBinSize(1, 0, 255);
        _bars = AvaPlot1.Plot.Add.Bars(hist.Bins, hist.Counts);
        _bars.Color = new Color(200, 200, 200);
        _bars.Horizontal = true;

        foreach (var bar in _bars.Bars)
        {
            bar.Size = hist.FirstBinSize;
            bar.LineWidth = 0;
            bar.FillStyle.AntiAlias = false;
        }

        SetupPlot();
        AvaPlot1.Refresh();
    }

    private void SetupPlot()
    {
        AvaPlot1.Plot.FigureBackground.Color = new Color(25, 25, 50);
        AvaPlot1.Plot.Grid.MajorLineColor = new Color(50, 50, 50);
        AvaPlot1.Plot.Axes.Color(new Color(200, 200, 200));
        AvaPlot1.Plot.Axes.SetLimitsY(0, 255);
        AvaPlot1.Plot.XLabel("Count");
        AvaPlot1.Plot.YLabel("Gray Value");
    }

    private void UpdatePlot(int[] counts)
    {
        for (var i = 0; i < _bars?.Bars.Count; i++) _bars.Bars[i].Value = counts[i];
        AvaPlot1.Plot.Axes.AutoScaleX();
        AvaPlot1.Refresh();
    }

    private void MainWindow_Opened(object? sender, EventArgs eventArgs)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.StartVideoFeed();
            vm.HistogramUpdated += UpdatePlot;
        }
    }
}