# Real Time Image Filtering with Histogram
## Setup
### 1. Install .NET SDK 10.0+
To check if .NET SDK is already installed, you can use
```
dotnet --version
```
If it does not exist on your system, or is outdated, go to https://dotnet.microsoft.com/download and follow the instructions to install the newest version
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
```
## Samples 
Sample Images can be found in the SampleOutputs directory  
Contains the original photo, filtered image, and histogram for each filter
## Development Environment
### IDE  
JetBrains Rider
### SDK
.NET SDK 10.0
### Runtimes
AvaloniaUI
EmguCV
ScottPlot
### Tested On
Windows 11
Ubuntu 24.04  
Using xUnit

## Example Application
![Example UI Window with Pikachu](ApplicationExample.png)

## Decisions 
### No Use of CUDA for EmguCV
EmguCV allows the use of CUDA to do the computer vision algorithms, and it is much faster since that runs on the GPU. Instead of opting to use it, I stuck with just CPU processing to maintain portability to all platforms independent of whether there is a graphics card present or not.

### MVVM Architecture with Avalonia
I wanted to use a mature UI Framwork that is used in industry today, while being totally cross-platform. Avalonia provides this exactly, allowing for data binding which connects your XAML objects to variables in your code and updates them as they change, needing you to do nothing to handle updating. MVVM is for Model-View-ViewModel which seperates UI, Logic, and Assets for the program, making the code more maintainable and testable.

### Image Processing as a Service 
Instead of doing all the processing in the ViewModel, I decided to create Processing service where the filtering and Image functions would reside. This created cleaner code, which then I could reference the service from the ViewModel and keep the respective functions seperate.



