using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyCloudProject.Common;
using MyExperiment.Models;
using MyExperiment.SEProject;
using MyExperiment.Utilities;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
//using System.Drawing;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NeoCortexApi;
using NeoCortexApi.Classifiers;
using NeoCortexApi.Entities;
using NeoCortexApi.Network;
using Emgu.CV;

namespace MyExperiment
{
    /// <summary>
    /// This class implements the whole long-running experiment.
    /// </summary>
    public class Experiment : IExperiment
    {
        private IFileStorageProvider storageProvider;

        private ILogger logger;

        private MyConfig config;

        private string expectedProjectName;
        /// <summary>
        /// construct the class
        /// </summary>
        /// <param name="configSection"></param>
        /// <param name="storageProvider"></param>
        /// <param name="expectedPrjName"></param>
        /// <param name="log"></param>
        public Experiment(IConfigurationSection configSection, IFileStorageProvider storageProvider, string expectedPrjName, ILogger log)
        {
            this.storageProvider = storageProvider;
            this.logger = log;
            this.expectedProjectName = expectedPrjName;
            config = new MyConfig();
            configSection.Bind(config);
        }


        /// <summary>
        /// Run Software Engineering project method
        /// </summary>
        /// <param name="inputFile">The input file</param>
        /// <returns>experiment result object</returns>
        public Task<ExperimentResult> Run(string[] inputFiles, string videoConfigFile, string htmConfigFile, string[] testFiles)
        {
            //From Here the code will start to be different
            //Learn configuration from videoConfig.json file
            var inputVideoConfig =
                JsonConvert.DeserializeObject<VideoConfig>(FileUtilities.ReadFile(videoConfigFile));
            int inputBits = inputVideoConfig.FrameWidth * inputVideoConfig.FrameHeight * (int)inputVideoConfig.ColorMode;

            //Learn configuration from htmConfig.json file
            var inputHtmConfig =
               JsonConvert.DeserializeObject<HtmConfig>(FileUtilities.ReadFile(htmConfigFile));
            inputHtmConfig.InputDimensions = new int[] { inputBits };
            ModifyHtmFromCode(ref inputHtmConfig);
            
            //Initiate logging of the whole running process
            this.logger?.LogInformation("Starting of software engineering code");
            ExperimentResult res = new ExperimentResult(this.config.GroupId, Guid.NewGuid().ToString());
            //?
            //res.OutputFiles = new string[2];
            res.StartTimeUtc = DateTime.UtcNow;
            this.logger?.LogInformation("Running software engineering code");
            res.OutputFiles = RunSoftwareEngineeringCode(inputVideoConfig, inputHtmConfig, inputFiles, testFiles).ToArray<string>();
            List<string> resultAccuracies = new List<string>();
            foreach (string path in res.OutputFiles)
            {
                string[] lines = File.ReadAllLines(path);
                resultAccuracies.Add(lines.ToString());
            }
            //res.Accuracy = Encoding.ASCII.GetString(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(resultAccuracies.ToString())));
            res.Accuracy = resultAccuracies.ToString();
            res.EndTimeUtc = DateTime.UtcNow;
            this.logger?.LogInformation("Finished execution of software engineering code");
            return Task.FromResult(res);

            //Old Code which has to be eventually deleted combining with the actual se project 
            //var inputDataList = JsonConvert.DeserializeObject<List<InputModel>>(FileUtilities.ReadFile(inputFile));
        }

        /// <inheritdoc/>
        public async Task RunQueueListener(CancellationToken cancelToken)
        {
            // TODO: This code used the deprected version of the Queue package.
            // Remove deprected version and replace it with the new one.
            // You will have to replace all code in this method.

            QueueClient queue = await CreateQueueAsync(config);

            while (cancelToken.IsCancellationRequested == false)
            {
                if (await queue.ExistsAsync())
                {
                    QueueProperties properties = await queue.GetPropertiesAsync();
                    if (properties.ApproximateMessagesCount > 0)
                    {
                        QueueMessage[] retrievedMessage = await queue.ReceiveMessagesAsync(1);
                        string theMessage = retrievedMessage[0].Body.ToString();
                        var base64EncodedBytes = Convert.FromBase64String(theMessage);
                        string message = Encoding.UTF8.GetString(base64EncodedBytes);
                        if (message != null)
                        {
                            try
                            {
                                this.logger?.LogInformation($"Received the message {message}");
                                this.logger?.LogInformation("Deserialize message");
                                ExerimentRequestMessage msg = JsonConvert.DeserializeObject<ExerimentRequestMessage>(message); ;
                                if (msg.ProjectName != this.expectedProjectName)
                                    throw new ApplicationException($"The expected project name is '{this.expectedProjectName}'. The message received is related to the project '{expectedProjectName}'");

                                //For each video files there must be a string in the array
                                string[] localInputFiles = new string[4];
                                localInputFiles[0] = await this.storageProvider.DownloadInputFileInFolder(msg.InputFileCircle);
                                localInputFiles[1] = await this.storageProvider.DownloadInputFileInFolder(msg.InputFileLine);
                                localInputFiles[2] = await this.storageProvider.DownloadInputFileInFolder(msg.InputFileRectangle);
                                localInputFiles[3] = await this.storageProvider.DownloadInputFileInFolder(msg.InputFileTriangle);

                                //For each test image files there must be a string in the array
                                string[] localTestFiles = new string[4];
                                localTestFiles[0] = await this.storageProvider.DownloadInputTestFile(msg.InputTestCircleFile);
                                localTestFiles[1] = await this.storageProvider.DownloadInputTestFile(msg.InputTestLineFile);
                                localTestFiles[2] = await this.storageProvider.DownloadInputTestFile(msg.InputTestRectangleFile);
                                localTestFiles[3] = await this.storageProvider.DownloadInputTestFile(msg.InputTestTriangleFile);

                                string[] inputFilesFromRequestMessage = new string[4];
                                inputFilesFromRequestMessage[0] = msg.InputFileCircle.ToString();
                                inputFilesFromRequestMessage[1] = msg.InputFileLine.ToString();
                                inputFilesFromRequestMessage[2] = msg.InputFileRectangle.ToString();
                                inputFilesFromRequestMessage[3] = msg.InputFileTriangle.ToString();
                                //Read two configuration files
                                var videoConfigFile = await this.storageProvider.DownloadInputFile(msg.InputFileVideoConfig);
                                var htmConfigFile = await this.storageProvider.DownloadInputFile(msg.InputFileHTMConfig);

                                ExperimentResult result = await this.Run(localInputFiles, videoConfigFile, htmConfigFile, localTestFiles);

                                this.logger?.LogInformation("Serialize result");
                                foreach(string path in result.OutputFiles)
                                {
                                    await storageProvider.UploadResultFile(path, System.IO.File.ReadAllBytes(path));
                                }
                                result.Students = msg.Students;
                                List<string> inputURIs = new List<string>();
                                foreach (string path in inputFilesFromRequestMessage) 
                                {
                                    inputURIs.Add(await AzureHelper.GetInputFileUrl(path, config));  
                                }
                                result.InputFileUrl = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(inputURIs.ToString()));
                                result.ExperimentId = msg.ExperimentId;
                                result.GroupName = msg.GroupName;
                                result.Status = ExperimentStatus.Succedded;
                                //result.OutputFiles[1] = Encoding.ASCII.GetString(await storageProvider.UploadResultFile($"result-{Guid.NewGuid()}.txt", Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result))));
                                await storageProvider.UploadExperimentResult(result);
                                this.logger?.LogInformation("Upload result");
                                await queue.DeleteMessageAsync(retrievedMessage[0].MessageId, retrievedMessage[0].PopReceipt);
                            }
                            catch (Exception ex)
                            {
                                this.logger?.LogError(ex, ex.Message);
                            }
                        }
                        else
                        {
                            this.logger?.LogInformation("There are no messages in the queue.");
                            await Task.Delay(500);
                        }
                    }
                }
            }

            this.logger?.LogInformation("Cancel pressed. Exiting the listener loop.");
        }


        #region Private Methods


        /// <summary>
        /// Create a queue for the sample application to process messages in. 
        /// </summary>
        /// <returns>A CloudQueue object</returns>
        private static async Task<QueueClient> CreateQueueAsync(MyConfig config)
        {
            // Retrieve storage account information from connection string.
            // CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(config.StorageConnectionString);

            // Create a queue client for interacting with the queue service
            QueueClient queue = new QueueClient(config.StorageConnectionString, config.Queue);
            
            Console.WriteLine("1. Create a queue for the demo");

            try
            {
                if (null != await queue.CreateIfNotExistsAsync())
                {
                    Console.WriteLine("The queue was created.");
                }
            }
            catch
            {
                Console.WriteLine("If you are running with the default configuration please make sure you have started the storage emulator.  ess the Windows key and type Azure Storage to select and run it from the list of applications - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            return queue;
        }

        /// <summary>
        /// Method to run software engineering code
        /// </summary>
        /// <param name="inputs">Inputs from azure blob storage</param>
        /// <returns></returns>
        private List<string> RunSoftwareEngineeringCode(VideoConfig videoConfig, HtmConfig htmCfg, string[] videoPaths, string[] testFilePaths)
        {
            //New implementation of actual SE Project Code
            //String array to hold accuracy result files
            List<string> resultFilePaths = new List<string>();

            // Output folder initiation
            string outputFolder = nameof(Experiment.expectedProjectName) + "_" + GetCurrentTime();
            string convertedVideoDir, testOutputFolder;
            CreateTemporaryFolders(outputFolder, out convertedVideoDir, out testOutputFolder);

            // A list of VideoSet object, each has the Videos and the name of the folder as Label, contains all the Data in TrainingVideos,
            // this List will be the core iterator in later learning and predicting
            List<VideoSet> videoData = new List<VideoSet>();

            // Iterate through every folder in TrainingVideos/ to create VideoSet: object that stores video of same folder/label
            foreach (string path in videoPaths)
            {
                VideoSet vs = new VideoSet(path, videoConfig.ColorMode, videoConfig.FrameWidth, videoConfig.FrameHeight, videoConfig.FrameRate);
                videoData.Add(vs);
                // Output converted Videos to Output/Converted/
                vs.ExtractFrames(convertedVideoDir);
            }

            var mem = new Connections(htmCfg);

            HtmClassifier<string, ComputeCycle> cls = new HtmClassifier<string, ComputeCycle>();

            CortexLayer<object, object> layer1 = new CortexLayer<object, object>("L1");

            TemporalMemory tm = new TemporalMemory();

            bool isInStableState = false;

            bool learn = true;

            //This should be 30 minimum
            int maxNumOfElementsInSequence = videoData[0].GetLongestFramesCountInSet();

            int maxCycles = 10;
            int newbornCycle = 0;

            //hpa should hold maxelement in sequence
            HomeostaticPlasticityController hpa = new HomeostaticPlasticityController(mem, maxNumOfElementsInSequence * 150 * 3, (isStable, numPatterns, actColAvg, seenInputs) =>
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
                //cls.ClearState();

            }, numOfCyclesToWaitOnChange: 50);

            SpatialPoolerMT sp = new SpatialPoolerMT(hpa);
            sp.Init(mem);
            tm.Init(mem);
            layer1.HtmModules.Add("sp", sp);

            //
            // Training SP to get stable. New-born stage.
            //
            ///*
            ///normally change it to while, only for less working time use the for loop
            //for (int i = 0; i < maxCycles; i++)
            while (isInStableState == false)
            {
                newbornCycle++;
                Console.WriteLine($"-------------- Newborn Cycle {newbornCycle} ---------------");
                foreach (VideoSet set in videoData)
                {
                    // Show Set Label/ Folder Name of each video set
                    WriteLineColor($"VIDEO SET LABEL: {set.VideoSetLabel}", ConsoleColor.Cyan);
                    foreach (NVideo vid in set.nVideoList)
                    {
                        // Name of the Video That is being trained 
                        WriteLineColor($"VIDEO NAME: {vid.name}", ConsoleColor.DarkCyan);
                        foreach (NFrame frame in vid.nFrames)
                        {
                            Console.Write(".");
                            var lyrOut = layer1.Compute(frame.EncodedBitArray, learn);
                            if (isInStableState)
                                break;
                        }
                        Console.WriteLine();
                    }
                }

                if (isInStableState)
                    break;
            }
            //*/

            layer1.HtmModules.Add("tm", tm);

            // Accuracy Check

            int cycle = 0;
            int matches = 0;

            List<string> lastPredictedValue = new List<string>();

            foreach (VideoSet vs in videoData)
            {
                List<string>accuracyLog = new List<string>();
                // Iterating through every video in a VideoSet
                foreach (NVideo nv in vs.nVideoList)
                {
                    int maxPrevInputs = nv.nFrames.Count - 1;
                    cycle = 0;
                    learn = true;

                    // Now training with SP+TM. SP is pretrained on the provided training videos.
                    // Learning each frame in a video
                    double lastCycleAccuracy = 0;
                    int saturatedAccuracyCount = 0;
                    bool isCompletedSuccessfully = false;

                    for (int i = 0; i < maxCycles; i++)
                    {
                        matches = 0;
                        cycle++;

                        Console.WriteLine($"-------------- Cycle {cycle} ---------------");

                        foreach (var currentFrame in nv.nFrames)
                        {
                            Console.WriteLine($"--------------SP+TM {currentFrame.FrameKey} ---------------");

                            // Calculating SDR from the current Frame
                            var lyrOut = layer1.Compute(currentFrame.EncodedBitArray, learn) as ComputeCycle;

                            Console.WriteLine(string.Join(',', lyrOut.ActivColumnIndicies));
                            // lyrOut is null when the TM is added to the layer inside of HPC callback by entering of the stable state

                            // In the pretrained SP with HPC, the TM will quickly learn cells for patterns
                            // In that case the starting sequence 4-5-6 might have the sam SDR as 1-2-3-4-5-6,
                            // Which will result in returning of 4-5-6 instead of 1-2-3-4-5-6.
                            // HtmClassifier allways return the first matching sequence. Because 4-5-6 will be as first
                            // memorized, it will match as the first one.



                            string key = currentFrame.FrameKey;
                            List<Cell> actCells;

                            WriteLineColor($"WinnerCell Count: {lyrOut.WinnerCells.Count}", ConsoleColor.Cyan);
                            WriteLineColor($"ActiveCell Count: {lyrOut.ActiveCells.Count}", ConsoleColor.Cyan);

                            if (lyrOut.ActiveCells.Count == lyrOut.WinnerCells.Count)
                            {
                                actCells = lyrOut.ActiveCells;
                            }
                            else
                            {
                                actCells = lyrOut.WinnerCells;
                            }

                            //This is where two training functions defers, here FrameKey is used for HTMClassifier learning
                            // Remember the key with corresponding SDR using HTMClassifier to assign the current frame key with the Collumns Indicies array
                            WriteLineColor($"Current learning Key: {key}", ConsoleColor.Magenta);
                            cls.Learn(currentFrame.FrameKey, actCells.ToArray());

                            if (learn == false)
                                Console.WriteLine("Inference mode");

                            Console.WriteLine($"Col  SDR: {Helpers.StringifyVector(lyrOut.ActivColumnIndicies)}");
                            Console.WriteLine($"Cell SDR: {Helpers.StringifyVector(actCells.Select(c => c.Index).ToArray())}");

                            if (lastPredictedValue.Contains(key))
                            {
                                matches++;
                                Console.WriteLine($"Match. Actual value: {key} - Predicted value: {key}");
                                lastPredictedValue.Clear();
                            }
                            else
                            {
                                Console.WriteLine($"Mismatch! Actual value: {key} - Predicted values: {String.Join(',', lastPredictedValue)}");
                                lastPredictedValue.Clear();
                            }

                            // Checking Predicted Cells of the current frame
                            // From experiment the number of Predicted cells increase over cycles and reach stability later.
                            if (lyrOut.PredictiveCells.Count > 0)
                            {
                                var predictedInputValues = cls.GetPredictedInputValues(lyrOut.PredictiveCells.ToArray(), 1);

                                foreach (var item in predictedInputValues)
                                {
                                    Console.WriteLine($"Current Input: {currentFrame.FrameKey} \t| Predicted Input: {item.PredictedInput}");
                                    lastPredictedValue.Add(item.PredictedInput);
                                }
                            }
                            else
                            {
                                Console.WriteLine("NO CELLS PREDICTED for next cycle.");
                                lastPredictedValue.Clear();
                            }
                        }
                        double accuracy;

                        accuracy = (double)matches / ((double)nv.nFrames.Count - 1.0) * 100.0;
                        accuracyLog.Add($"{accuracy}");                        

                        Console.WriteLine($"Cycle: {cycle}\tMatches={matches} of {nv.nFrames.Count}\t {accuracy}%");
                        if (accuracy == lastCycleAccuracy)
                        {
                            // The learning may result in saturated accuracy
                            // Unable to learn to higher accuracy, Exit
                            saturatedAccuracyCount += 1;
                            if (saturatedAccuracyCount >= 10 && lastCycleAccuracy >= 70)
                            {
                                List<string> outputLog = new List<string>();
                                MakeDirectoryIfRequired(Path.Combine(outputFolder, "TEST"));

                                string fileName = Path.Combine(outputFolder, "TEST", $"saturatedAccuracyLog_{nv.label}_{nv.name}");
                                //UpdateAccuracy(vs.VideoSetLabel, nv.name, accuracy, Path.Combine(outputFolder, "TEST"));
                                outputLog.Add($"Result Log for reaching saturated accuracy at {accuracy}");
                                outputLog.Add($"Label: {nv.label}");
                                outputLog.Add($"Video Name: {nv.name}");
                                outputLog.Add($"Stop after {cycle} cycles");
                                //outputLog.Add($"Elapsed time: {sw.ElapsedMilliseconds / 1000 / 60} min.");
                                outputLog.Add($"reaching stable after enter newborn cycle {newbornCycle}.");
                                resultFilePaths.Add(RecordResult(outputLog, fileName));
                                isCompletedSuccessfully = true;

                                break;
                            }
                        }
                        else
                        {
                            saturatedAccuracyCount = 0;
                        }
                        lastCycleAccuracy = accuracy;

                        // Reset Temporal memory after learning 1 time the video/sequence
                        tm.Reset(mem);
                    }
                    if (isCompletedSuccessfully == false)
                    {
                        Console.WriteLine($"The experiment didn't complete successully. Exit after {maxCycles}!");

                    }
                    Console.WriteLine("------------ END ------------");
                }
                MakeDirectoryIfRequired(Path.Combine(outputFolder, "TEST"));
                resultFilePaths.Add(RecordResult(accuracyLog, Path.Combine(outputFolder, "TEST", $"AccuracyLog_{vs.VideoSetLabel}")));
            }
            //Testing Section
            MakeDirectoryIfRequired(testOutputFolder);

            // Test from startupConfig.json
            foreach (var testFilePath in testFilePaths)
            {
                string testOutputPath = PredictImageInput(videoData, cls, layer1, testOutputFolder, testFilePath);
                resultFilePaths.Add(testOutputPath);
            }
        

        //Old code that used in the other CC project
/*        var classifier = new CLAClassifier<double>(new List<int> { inputs[0].Steps },
                inputs[0].Alpha, inputs[0].ActValueAlpha);
            var filename = $"output-{Guid.NewGuid()}.txt";
            int number = 0;
            foreach (var input in inputs)
            {
                
                Dictionary<string, object> classification = new Dictionary<string, object>();
                classification.Add("bucketIdx", input.BucketIndex);
                classification.Add("actValue", input.ActualValueInBucket);
                // After this step bucket values will be updated
                Classification<double> result = classifier.Compute(number++, classification, input.InputFromTemporalMemory, true, true);
                var predictionResult = result.getActualValues();
                var outputModel = new OutputModel(predictionResult);
                var outputAsByte = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(outputModel));
                FileUtilities.WriteDataInFile(Path.Combine(FileUtilities.GetLocalStorageFilePath(config.LocalPath), filename), predictionResult, input.BucketIndex);
            }*/
            return resultFilePaths;
        }

        /// <summary>
        /// Predict series from input Image.
        /// <br>Process:</br>
        /// <br>Binarize input image</br>
        /// <br>Convert the binarized image to SDR via Spatial Pooler</br>
        /// <br>Get Predicted Cells from Compute output </br>
        /// <br>Compare the predicted Cells with learned HTMClassifier</br>
        /// <br>Create predicted image sequence as Video from classifier output and video database videoData </br>
        /// </summary>
        /// <param name="frameWidth">image framewidth</param>
        /// <param name="frameHeight"></param>
        /// <param name="colorMode"></param>
        /// <param name="videoData"></param>
        /// <param name="cls"></param>
        /// <param name="layer1"></param>
        /// <param name="userInput"></param>
        /// <param name="testOutputFolder"></param>
        /// <param name="testNo"></param>
        /// <returns></returns>
        private static string PredictImageInput(List<VideoSet> videoData, HtmClassifier<string, ComputeCycle> cls, CortexLayer<object, object> layer1, string testOutputFolder, string inputfilePath)
        {
            //Question Arise if it is used in program.cs then a normal user can not say which layer it belongs to and currently it's hard coded
            // TODO: refactor video library for easier access to these properties
            (int frameWidth, int frameHeight, ColorMode colorMode) = videoData[0].VideoSetConfig();
            
                string Outputdir = Path.Combine(testOutputFolder, $"Predicted from {Path.GetFileNameWithoutExtension(inputfilePath)}");
                MakeDirectoryIfRequired(Outputdir);

                // Save the input Frame as NFrame
                NFrame inputFrame = new NFrame(SKBitmap.Decode(inputfilePath), "TEST", "test", 0, frameWidth, frameHeight, colorMode);
                inputFrame.SaveFrame(Path.Combine(Outputdir, $"Converted_{Path.GetFileName(inputfilePath)}"));
                // Compute the SDR of the Frame
                var lyrOut = layer1.Compute(inputFrame.EncodedBitArray, false) as ComputeCycle;

                // Use HTMClassifier to calculate 5 possible next Cells Arrays
                var predictedInputValue = cls.GetPredictedInputValues(lyrOut.PredictiveCells.ToArray(), 5);

                WriteLineColor("Predicting for " + Path.GetFileNameWithoutExtension(inputfilePath), ConsoleColor.Red);
                List<string> outputLog = new List<string>();
                foreach (var serie in predictedInputValue)
                {
                    WriteLineColor("Predicted Series:", ConsoleColor.Green);
                    //Here the frame matching accuracy is calculated
                    double objectAccuracy = serie.Similarity;
                    string s = serie.PredictedInput;
                    //Create List of NFrame to write to Video
                    List<NFrame> outputNFrameList = new List<NFrame>();
                    string Label = "";
                    List<string> frameKeyList = s.Split("-").ToList();
                    string[] frameName = frameKeyList[0].Split('_');
                WriteLineColor($"{objectAccuracy}% match found with " + frameName[0]);
                outputLog.Add($"For {Path.GetFileName(inputfilePath)} {objectAccuracy}% match found with " + frameName[0] + "\n" + s);
                //UpdateAccuracy(Path.GetFileNameWithoutExtension(inputfilePath), Path.GetFileNameWithoutExtension(inputfilePath), objectAccuracy, Outputdir, details);
                    Console.WriteLine("\n");
                    foreach (string frameKey in frameKeyList)
                    {
                        foreach (var vs in videoData)
                        {
                            foreach (var vd in vs.nVideoList)
                            {
                                foreach (var nf in vd.nFrames)
                                {
                                    if (nf.FrameKey == frameKey)
                                    {
                                        Label = nf.label;
                                        outputNFrameList.Add(nf);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    // Create output video
                    NVideo.CreateVideoFromFrames(
                        outputNFrameList,
                        Path.Combine(Outputdir, $"testNo_Label{Label}_similarity{serie.Similarity}_No of same bit{serie.NumOfSameBits}"),
                        (int)videoData[0].nVideoList[0].frameRate,
                        new SKSize((int)videoData[0].nVideoList[0].frameWidth, (int)videoData[0].nVideoList[0].frameHeight),
                        true);
                }
                string outputResultPath = RecordResult(outputLog, Outputdir);
            

            return outputResultPath;
        }

        /// <summary>
        /// Writing experiment result to write to a text file
        /// </summary>
        /// <param name="possibleOutcomeSerie"></param>
        /// <param name="inputVideo"></param>
        private static string RecordResult(List<string> result, string fileName)
        {
            string path = $"{fileName}.txt";
            File.WriteAllLines(path, result);
            return path;
        }

        /// <summary>
        /// Optional solution for modifying HTM settings in code
        /// </summary>
        /// <param name="htmConfig">Htm Configuration to be modified</param>
        private static void ModifyHtmFromCode(ref HtmConfig htmConfig)
        {
            htmConfig.Random = new ThreadSafeRandom(42);

            htmConfig.CellsPerColumn = 40;
            htmConfig.GlobalInhibition = true;
            //htmConfig.LocalAreaDensity = -1;
            htmConfig.NumActiveColumnsPerInhArea = 0.02 * htmConfig.ColumnDimensions[0];
            htmConfig.PotentialRadius = (int)(0.15 * htmConfig.InputDimensions[0]);
            //htmConfig.InhibitionRadius = 15;

            htmConfig.MaxBoost = 30.0;
            htmConfig.DutyCyclePeriod = 100;
            htmConfig.MinPctOverlapDutyCycles = 0.75;
            htmConfig.MaxSynapsesPerSegment = (int)(0.02 * htmConfig.ColumnDimensions[0]);
            htmConfig.StimulusThreshold = (int)0.05 * htmConfig.ColumnDimensions[0];
            //ActivationThreshold = 15;
            //ConnectedPermanence = 0.5;

            // Learning is slower than forgetting in this case.
            //PermanenceDecrement = 0.15;
            //PermanenceIncrement = 0.15;

            // Used by punishing of segments.
        }

        /// <summary>
        /// Create folders required for the experiment.
        /// </summary>
        /// <param name="outputFolder">Output folder</param>
        /// <param name="convertedVideoDir">Converted Video directory</param>
        /// <param name="testOutputFolder">Test output folder</param>
        private static void CreateTemporaryFolders(string outputFolder, out string convertedVideoDir, out string testOutputFolder)
        {
            MakeDirectoryIfRequired(outputFolder);

            convertedVideoDir = Path.Combine(outputFolder, "Converted");
            MakeDirectoryIfRequired(convertedVideoDir);

            testOutputFolder = Path.Combine(outputFolder, "TEST");
            MakeDirectoryIfRequired(testOutputFolder);
        }
        
        /// <summary>
        /// If the directory does not exist, it enters the directory
        /// <param name="path">directory path</param>
        private static void MakeDirectoryIfRequired(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        
        /// <summary>
        /// Current timestamp to know when the program was started 
        /// </summary>
        /// <returns>Current time without any slash</returns>
        private static string GetCurrentTime()
        {
            var currentTime = DateTime.Now.ToString();

            var timeWithoutSpace = currentTime.Split();

            var timeWithUnderScore = string.Join("_", timeWithoutSpace);

            var timeWithoutColon = timeWithUnderScore.Replace(':', '-');

            var timeWithoutSlash = timeWithoutColon.Replace('/', '-');

            return timeWithoutSlash;
        }

        /// <summary>
        /// Print a line in Console with color and/or hightlight
        /// <param name="str">string to print</param>
        /// <param name="foregroundColor">Text color</param>
        /// <param name="backgroundColor">Hightlight Color</param>
        /// </summary>
        private static void WriteLineColor(
            string str,
            ConsoleColor foregroundColor = ConsoleColor.White,
            ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.WriteLine(str);
            Console.ResetColor();
        }
        #endregion
    }
}
