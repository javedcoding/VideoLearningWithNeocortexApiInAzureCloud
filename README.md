# Video Learning With NeoCortexApi:
Module: ML 21/22 - Video Learning Project migration
Instructor: Damir Dobric, Proffessor Andreas Pech.  
Student: 
Mashnunul Huq, Mtr.1384042 Major:IT
Toan Thanh Truong, Mtr. 1185050 Major: IT Gbr. 23.02.1997

_this readme serves as the submitted projectreport for the registered project Video Learning with HTM implemented on cloud computing.

## 1. Motivation:
This work "Video Learning with HTM CLA" introduces videos data into the Cortical Learning Algorithm in [NeoCortex Api](https://github.com/ddobric/neocortexapi).  
Experiment in current work involves using Temporal Memory to learn binary representation of videos (sequence of bitarrays - each bitarray represents 1 frame).  
Afterwards the result of the learning is tested by giving the trained model an abitrary image, the model then tries to recreate a video with proceeding frame after the input frame. All the
input video files are uploaded in a azure blobstorage container and the result files on accuracy are uploaded in another blobstorage container with a result table. 

## 2. Overview:
In this experiment, Videos are learned as sequences of Frames. The link to the project code can be found in [VideoLearning.cs](https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/tree/mashnunulHuq/Source/MyCloudProjectSample).  

This project references Sequence Learning sample, see [SequenceLearning.cs](https://github.com/ddobric/neocortexapi/tree/master/source/Samples/NeoCortexApiSample).  

Input Videos are currently generated from python scripts, using OpenCV2. See [DataGeneration](https://github.com/ddobric/neocortexapi/tree/SequenceLearning_ToanTruong/DataGeneration) for manual on usage and modification.  

The Reading of Videos are enabled by [VideoLibrary](https://github.com/ddobric/neocortexapi/tree/SequenceLearning_ToanTruong/Project12_HTMCLAVideoLearning/HTMVideoLearning/VideoLibrary), written for this project using OpenCV2. This library requires nuget package [Emgu.CV](https://www.nuget.org/packages/Emgu.CV/), [Emgu.CV.Bitmap](https://www.nuget.org/packages/Emgu.CV.Bitmap/), [Emgu.CV.runtimes.windows](https://www.nuget.org/packages/Emgu.CV.runtime.windows/) version > 4.5.3.  

Learning process include: 
1. downloading and reading videos from input container blobstorage.
2. convert videos to Lists of bitarrays.
3. Spatial Pooler Learning with Homeostatic Plasticity Controller until reaching stable state.
4. Learning with Spatial pooler and Temporal memory, conditional exit.
5. Interactive testing section, output video from frame input and uploading the accuracy result into output container blobstorage.
## 3. Data Generation:
The current encoding mechanism of the frame employs the convert of each pixels into an part in the input bit array. This input bit array is used by the model for training.  
There are currently 1 training set:
- has 4 video, 1 foreach label in {circle line rectangle triangle}.    
- situated in the blob storage codebreakersmashnunulinput
- also the container consists of two configuration json files {htmConfig.json videoConfig.json}  

## 4. Queue Message:
The que message format is this (https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/MyExperiment/Data/QueueMessage.json)
```json
{
  "ExperimentId": "1",
  "InputFileCircle": "circle.mp4",
  "InputFileLine": "line.mp4",
  "InputFileRectangle": "rectangle.mp4",
  "InputFileTriangle": "triangle.mp4",
  "InputFileVideoConfig": "videoConfig.json",
  "InputFileHTMConfig": "htmConfig.json",
  "InputTestCircleFile": "Circle_circle_10.png",
  "InputTestLineFile": "Line_line_20.png",
  "InputTestRectangleFile": "Rectangle_rectangle_1.png",
  "InputTestTriangleFile": "Triangle_triangle_5.png",
  "Description": "Cloud Computing Implementation",
  "ProjectName": "ML21/22-Video Learning Project Migration",
  "GroupName": "codebreakersmashnunul",
  "Students": [ "mashnunul" ]
}
```
If new videos of object circle or line has to be uploaded the name of the file must be put corresponding to the left side keys.
The input test files also can be changed and should be put according to the left side key names.
The crucial thing to follow carefully is the ProjectName. This has to be exactly same as mentioned above as the project name was fixed 
and the program can only run if it matches.

## 5. Videos Reading:
First download of the files in container named "codebreakersmashnunulinput" is done by using the method DownloadInputFileInFolder (https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/MyExperiment/AzureBlobStorageProvider.cs) 
For more examples on how to use the Library, see [VideoLibraryTest](https://github.com/ddobric/neocortexapi/tree/SequenceLearning_ToanTruong/Project12_HTMCLAVideoLearning/HTMVideoLearning/VideoLibraryTest).  
Video Library is seperated into 3 sub classes:
- [**VideoSet**](https://github.com/ddobric/neocortexapi/blob/SequenceLearning_ToanTruong/Project12_HTMCLAVideoLearning/HTMVideoLearning/VideoLibrary/VideoSet.cs):  
Group of multiple **NVideo** put under the same label.  
Read multiple video under a folder, use the folder name as label.  
Get the stored **NFrame** from a given framekey.  
Create converted videos as output from read training set.  
There is a TestClass

- [**NVideo**](https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/MyExperiment/SEProject/NVideo.cs):  
Represent a video, has multiple **NFrame**.  
Read a video in different frame rate. (only equal/lower framerates are possible)
(In case new video codec is used for making the predicted video, add the file extension with Four character codec in the CorrespondingFileExtension enumeration class)

- [**NFrame**](https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/MyExperiment/SEProject/NFrame.cs):  
represent a frame, has converted bitarray from the frame pixel reading.
Can convert a bit array to Bitmap.  
Also includes Framkey parameters, which is used to index the frame and learning with [HTMClassifier](https://github.com/ddobric/neocortexapi/blob/master/source/NeoCortexApi/Classifiers/HtmClassifier.cs).  
**Framkey = (label)\_(VideoName)\_(index)**  e.g. circle_vd1_03  
The current color encoding of each frame when reading videos [includes 3 mode](https://github.com/ddobric/neocortexapi/blob/027ead7a860f1ae115c56583035fc8fe21b97c83/Project12_HTMCLAVideoLearning/HTMVideoLearning/VideoLibrary/NFrame.cs#L12):  
1. ``BLACKWHITE``: binarized frame by reading luminance value:  
```csharp
double luminance = (3 * pixel.R + pixel.B + 4 * pixel.G)>>3; 
```  
2. ``BINARIZEDRGB``: ref [ImageBinarizer](https://github.com/UniversityOfAppliedSciencesFrankfurt/imagebinarizer) binarized each color channel in Red, Green and Blue:
```csharp
imageBinary.AddRange(new List<int>() { (pixel.R > 255 / 2) ? 1 : 0, (pixel.G > 255 / 2) ? 1 : 0, (pixel.B > 255 / 2) ? 1 : 0 });
```
3. ``PURE``: encode all 8 bits for each channel in RGB adding 3 channel x 8bits for each pixel:

```csharp
imageBinary.AddRange(ColorChannelToBinList(pixel.R));
imageBinary.AddRange(ColorChannelToBinList(pixel.G));
imageBinary.AddRange(ColorChannelToBinList(pixel.B));
```
These can be further added to the [ImageBinarizer](https://github.com/UniversityOfAppliedSciencesFrankfurt/imagebinarizer) as custom encoder.  
The current experiment focus on the ``BLACKWHITE`` colorMode, due to its low consumption in memory, which result in faster runtime of experiment.
The lowest dimension of the video is 18*18, test has revealed lower dimension result in codec error of Emgu.CV with auto config mode -1 in code:  
```csharp
VideoWriter videoWriter = new($"{videoOutputPath}.mp4", -1, (int)frameRate, dimension, isColor)
// Whereas dimension is dimension = new Size(18,18);
// -1 means choosing an codec automatically, only on windows
```
**NOTE:**  
The current implementation of VideoLibrary saves all training data into a List of VideoSet, which contains all video information and their contents. For further scaling of the training set. It would be better to only store the index, where to access the video from the training data. This way the data would only be access when it is indexed and save memory for other processes.
## 4.1. Learning Process:
The configuration file for htm learning is downloaded from the inputcontainer named codebreakersmashnunulinput as uploaded htmConfig.json (Changes to the parameters are possible but the file name should be same)
Current HTM Configuration:
```csharp
private static HtmConfig GetHTM(int[] inputBits, int[] numColumns)
{
    HtmConfig htm = new(inputBits, numColumns)
    {
        Random = new ThreadSafeRandom(42),

        CellsPerColumn = 30,
        GlobalInhibition = true,
        //LocalAreaDensity = -1,
        NumActiveColumnsPerInhArea = 0.02 * numColumns[0],
        PotentialRadius = (int)(0.15 * inputBits[0]),
        //InhibitionRadius = 15,

        MaxBoost = 10.0,
        //DutyCyclePeriod = 25,
        //MinPctOverlapDutyCycles = 0.75,
        MaxSynapsesPerSegment = (int)(0.02 * numColumns[0]),

        //ActivationThreshold = 15,
        //ConnectedPermanence = 0.5,

        // Learning is slower than forgetting in this case.
        //PermanenceDecrement = 0.15,
        //PermanenceIncrement = 0.15,

        // Used by punishing of segments.
    };
    return htm;
}
```

After reading the Videos into VideoSets, the learning begins.
## 4.2. Video Configuration:
Current Video Configuration:
```json
  "frameWidth": 18,
  "frameHeight": 18,
  "frameRate": 12,
  "ColorMode": "BLACKWHITE"
``` 
To imply new video sets these configuration in vidoeConfig.json needs to be changed acordingly.
If the video file codec is not mp4 then go to the line where NVideo.CreateVideoFromFrames is initiated and change the argument codec = ['P', 'I', 'M', '1'] or ['H', '2', '6', '4']
The default codec = ['m', 'p', '4', 'v'] 

### 1. SP Learning with HomeoStatic Plasticity Controller (HPA):
This first section of learning use Homeostatic Plasticity Controller:
```csharp
HomeostaticPlasticityController hpa = new(mem, 30 * 150*3, (isStable, numPatterns, actColAvg, seenInputs) =>
{
    if (isStable)
        // Event should be fired when entering the stable state.
        Console.WriteLine($"STABLE: Patterns: {numPatterns}, Inputs: {seenInputs}, iteration: {seenInputs / numPatterns}");
    else
        // Ideal SP should never enter unstable state after stable state.
        Console.WriteLine($"INSTABLE: Patterns: {numPatterns}, Inputs: {seenInputs}, iteration: {seenInputs / numPatterns}");
        // We are not learning in instable state.
        learn = isInStableState = isStable;
        // Clear all learned patterns in the classifier.
        cls.ClearState();
}, numOfCyclesToWaitOnChange: 50);
```
The average number of cycles required for the "smallTrainingSet" is 270 cycles, "training with Spatial Pooler only" in this experiment used a while loop until the model reach true in parameter **IsInStableState**.
### 2. SP+TM Learning of frame sequences in the video set:
HPA will be triggered with the Compute method of the current layer. One problem during the learning is that even after successfull enter to stable state in Learning only with SP, the model can get unstable again after learning the first video or the second video in SP+TM stage. Thus:
```csharp
//Iteration:
foreach (VideoSet vd in videoData)
    {
    foreach (NVideo nv in vd.nVideoList)
        {
            // LOOP1
            // After finished learning in this cycle and move to the next video
            // The model somtimes becomes unstable and trigger cls.ClearState in HPA, making the HTMClassifier forget all what it has learned.  
            // Specificaly It clears the m_ActiveMap2 
            // To cope with this problem and faster debug the learning process, some time the experiment comment out the cls.ClearState() in HPA
        for (int i = 0; i < maxCycles; i++)
        }
    }
``` 
may be changed to:  
```csharp
//Iteration:
for (int i = 0; i < maxCycles; i++)
    {
        foreach (VideoSet vd in videoData)
        {
            foreach (NVideo nv in vd.nVideoList)
            {
                // LOOP2
                // This ensure the spreading learning of all the frames in different videos  
                // this keep cls.ClearState() in hpa and successully run to the end means that Learning process doesn't end in unstable state.
            }
        }
    }
``` 
For the current 2 tests:  
**_Run1: "SP only" runs LOOP2 || SP+TM runs LOOP1_**  
Key to learn with HTMClassifier: **FrameKey**, e.g.  rectangle_vd5_0, triangle_vd4_18, circle_vd1_9.  
Condition to get out from loop:
- Accuracy is calulated from prediction of all videos
- After run on each video, a loop is used to calculate the Predicted nextFrame of each frame in the video, the last frame doesn't have next predicted cells by usage of `tm.Reset(mem)` as indentification for end of video.  
```csharp
// correctlyPredictedFrame increase by 1 when the next FrameKey is in the Pool of n possibilities calculated from HTMClassifier cls.  
var lyrOut = layer1.Compute(currentFrame.EncodedBitArray, learn) as ComputeCycle;
var nextFramePossibilities = cls.GetPredictedInputValues(lyrOut.PredictiveCells.ToArray(), 1);
// e.g. if current frame is rectangle_vd5_0 and rectangle_vd5_1 is in nextFramePossibilities, then correctlyPredictedFrame for this Video increase by 1.  

double accuracy = correctlyPredictedFrame / ((double)trainingVideo.Count-1);
// The accuracy of each video add up to the final average accuracy of the VideoSet
videoAccuracy.Add(accuracy);
...
double currentSetAccuracy = videoAccuracy.Average();
// The accuracy of each VideoSet add up to the total cycleAccurary
setAccuracy.Add(currentSetAccuracy);
...
cycleAccuracy = setAccuracy.Average();
// The Learning is consider success when cycleAccuracy exceed 90% and stay the same more than 40 times
if(stableAccuracyCount >= 40 && cycleAccuracy> 0.9)
// The result is saved in Run1ExperimentOutput/TEST/saturatedAccuracyLog_Run1.txt.  
// In case the Experiment reach maxCycle instead of end condition, the log will be saved under Run1ExperimentOutput/TEST/MaxCycleReached.txt
``` 
After finishing the user will be prompted to input a picture path.  
The trained layer will use this image to try recreate the video it has learned from the training data.  
- The image can be drag into the command window and press enter to confirm input. The model use the input frame to predict the next frame, then continue the process with the output frame if there are still predicted cells from calculation. For the first prediction HTMClassifier takes at most 5 possibilities of the next frame from the input.  
- In case there are at least 1 frame, the codecs will appears and the green lines indicate the next predicted frame from the memory by HTMClassifier. 
- The output video can be found under Run1ExperimentOutput/TEST/ with the folder name (Predicted From "Image name").  
- Usually in this Test, The input image are chosen from the Directory Run1Experiment/converted/(label)/(videoName)/(FrameKey) for easier check if the trained model predict the correct next frame.  

## 6 Test Files:
Test files can be uploaded in the blobstorage container named "codebreakersmashnunultestdata". To work with new .png images change these keys value only in the que message
```json
{
  "InputTestCircleFile": "Circle_circle_10.png",
  "InputTestLineFile": "Line_line_20.png",
  "InputTestRectangleFile": "Rectangle_rectangle_1.png",
  "InputTestTriangleFile": "Triangle_triangle_5.png",
}
```

## 7 Test Outputs:
The test outputs can be seen in two ways. 
1. Azure Storage table:
Here with every run a table instance is created with the date time stamp and accuracy and input file urls can be seen 
2. Azure blobstorage:
The output blob storage is named as "codebreakersmashnunuloutput" where the accuracy, saturated accuracy(if reached) and test file accuracies can be seen in the corresponding file names.

## 8 Running the Project in Azure:
Go to the container instance named "codebrakers" and press restart. The docker image is situated in the container registry named "codebreakersmashnunulregistry". 
It can also be done manually by downloading the docker image from the container registry and run the command in powershell or command promt
docker run mycloudproject
In case of the image not found please see the image is present in docker's local repository by pushing the command
docker image list

[Currently because of the Emgu.CV library missing cuda dll files in the nuget.org, the project is only run by visual studio (see issue number 238 here(https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/issues/238))]


