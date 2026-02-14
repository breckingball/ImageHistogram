# Real Time Image Filtering with Histogram
## Setup
### 1. Install .NET SDK
To check if .NET SDK is already installed, you can use
```
dotnet --version
```
If it does not exist on your system, go to https://dotnet.microsoft.com/download and follow the instructions
### 2. Clone GitHub Repository
In your terminal, switch to the directory where the project will go, then run
```
git clone https://github.com/breckingball/ImageHistogram.git
```
Then switch to the project directory using
```
cd ImageHistogram
```
### 3. Restore Project Dependencies
In the directory you are in, run
```
dotnet restore
```
This will install all the needed dependencies for the project using NuGet
### 4. Build and Run Project
Switch to the directory that contains the application code with
```
cd CorningCodeTest
```
Build the Project using 
```
dotnet build
```
Finally, Run the project using
```
dotnet run
```
You should see the UI Window appear and now the program is running
## Frameworks Used
### Avalonia - Cross-Platform UI
### EmguCV - OpenCV Wrapper for C#
### ScottPlot - Cross-Platform Plotting Library
## Project Structure
```
CorningCodeTest/
├── CorningCodeTest/
│   ├── Services/
│   │   ├── ImageProcessing.cs
│   │   └── SampleGenerator.cs
│   ├── ViewModels/
│   │   ├── MainViewModel.cs
│   │   └── ViewModelBase.cs
│   ├── Views/
│   │   ├── MainWindow.axaml
│   │   └── MainWindow.axaml.cs
│   ├── App.axaml
│   ├── App.axaml.cs
│   └── Program.cs
├── CorningCodeTest.Tests/
│   └── Services/
│       └── ImageProcessingTests.cs
└── SampleOutputs/
    ├── Original/
    ├── Inverted1/
    ├── Inverted2/
    ├── Grayscale/
    ├── Blurred/
    └── Canny/
