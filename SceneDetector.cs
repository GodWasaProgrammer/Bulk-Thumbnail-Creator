namespace BulkThumbnailCreator;

using System.Collections.Concurrent;
using Emgu.CV;
using Emgu.CV.CvEnum;

public class SceneDetector
{
    public async Task DetectAndSaveBestFramesParallelAsync(string videoPath, string outputFolder)
    {
        await Task.Run(() =>
        {
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            using (var capture = new VideoCapture(videoPath))
            {
                if (!capture.IsOpened)
                {
                    Console.WriteLine("[ERROR] Kunde inte öppna video!");
                    return;
                }

                double fps = capture.Get(CapProp.Fps);
                int cooldownFrames = (int)(fps * 2.5);
                Console.WriteLine($"[INFO] Video FPS: {fps} | Cooldown: {cooldownFrames} frames");

                var frameBuffer = new BlockingCollection<FrameData>(boundedCapacity: 10);
                var sceneData = new SceneAnalysisData(cooldownFrames);

                
                var processingTask = Task.Run(() => ProcessFrames(frameBuffer, sceneData, outputFolder));

                try
                {
                    int frameCount = 0;
                    while (true)
                    {
                        using (var currentFrame = new Mat())
                        {
                            if (!capture.Read(currentFrame))
                            {
                                Console.WriteLine("[DEBUG] Slut på video");
                                break;
                            }

                            frameBuffer.Add(new FrameData
                            {
                                Frame = currentFrame.Clone(),
                                FrameNumber = frameCount++
                            });
                        }
                    }
                }
                finally
                {
                    frameBuffer.CompleteAdding();
                    processingTask.Wait();

                    // Spara sista scenen
                    if (sceneData.BestFrame != null)
                    {
                        SaveBestFrame(sceneData, outputFolder);
                    }
                }
            }
        });
    }

    private void ProcessFrames(
        BlockingCollection<FrameData> frameBuffer,
        SceneAnalysisData sceneData,
        string outputFolder)
    {
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        };

        foreach (var frameData in frameBuffer.GetConsumingEnumerable())
        {
            try
            {
                using (frameData.Frame)
                {
                    // Steg 1: Validera bildruta
                    if (frameData.Frame.IsEmpty || IsBlackFrame(frameData.Frame))
                    {
                        Console.WriteLine($"[DEBUG] Skipping frame {frameData.FrameNumber}");
                        continue;
                    }

                    // Steg 2: Beräkna förändring (parallelliserbar)
                    double change = sceneData.PreviousFrame != null
                        ? CalculateFrameDifference(sceneData.PreviousFrame, frameData.Frame)
                        : 0;

                    // Steg 3: Beräkna skärpa (parallelliserbar)
                    double currentScore = CalculateSharpness(frameData.Frame);

                    // Steg 4: Hantera scenbyten (synkroniserad)
                    lock (sceneData.LockObject)
                    {
                        ProcessScene(frameData, change, currentScore, sceneData, outputFolder);
                        sceneData.PreviousFrame?.Dispose();
                        sceneData.PreviousFrame = frameData.Frame.Clone();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Fel vid bearbetning av frame {frameData.FrameNumber}: {ex.Message}");
            }
        }
    }

    private void ProcessScene(
        FrameData frameData,
        double change,
        double currentScore,
        SceneAnalysisData sceneData,
        string outputFolder)
    {
        bool isCooldownOver = (frameData.FrameNumber - sceneData.LastSceneFrame) >= sceneData.CooldownFrames;

        // Scenskifte
        if (change > 15 && isCooldownOver && sceneData.BestFrame != null)
        {
            if (!IsBlackFrame(sceneData.BestFrame))
            {
                SaveScene(sceneData.BestFrame, outputFolder, sceneData.SceneNumber++);
                sceneData.LastSceneFrame = frameData.FrameNumber;
            }
            sceneData.BestFrame?.Dispose();
            sceneData.BestFrame = frameData.Frame.Clone();
        }

        // Uppdatera bästa bilden
        if (sceneData.BestFrame == null || currentScore > CalculateSharpness(sceneData.BestFrame))
        {
            sceneData.BestFrame?.Dispose();
            sceneData.BestFrame = frameData.Frame.Clone();
            Console.WriteLine($"[BEST] Frame {frameData.FrameNumber} | Score: {currentScore:F2}");
        }
    }

    private void SaveScene(Mat frame, string outputFolder, int sceneNumber)
    {
        string outputPath = Path.Combine(outputFolder, $"scene_{sceneNumber}.jpg");
        CvInvoke.Imwrite(outputPath, frame);
        Console.WriteLine($"[SAVE] Scene {sceneNumber} sparad");
    }

    private void SaveBestFrame(SceneAnalysisData sceneData, string outputFolder)
    {
        string outputPath = Path.Combine(outputFolder, $"scene_{sceneData.SceneNumber}.jpg");
        CvInvoke.Imwrite(outputPath, sceneData.BestFrame);
        Console.WriteLine($"[FINAL] Sista scenen sparad");
    }

    private bool IsBlackFrame(Mat frame)
    {
        using (Mat gray = new Mat())
        {
            CvInvoke.CvtColor(frame, gray, ColorConversion.Bgr2Gray);
            double mean = CvInvoke.Mean(gray).V0;
            bool isBlack = mean < 10;
            if (isBlack) Console.WriteLine($"[BLACK] Mean: {mean:F2}");
            return isBlack;
        }
    }

    private double CalculateFrameDifference(Mat a, Mat b)
    {
        using (Mat diff = new Mat())
        {
            CvInvoke.AbsDiff(a, b, diff);
            CvInvoke.CvtColor(diff, diff, ColorConversion.Bgr2Gray);
            CvInvoke.Threshold(diff, diff, 25, 255, ThresholdType.Binary);
            return CvInvoke.Mean(diff).V0;
        }
    }

    private double CalculateSharpness(Mat frame)
    {
        using (Mat gray = new Mat())
        {
            CvInvoke.CvtColor(frame, gray, ColorConversion.Bgr2Gray);
            CvInvoke.Laplacian(gray, gray, DepthType.Cv64F);
            return CvInvoke.Mean(gray).V0;
        }
    }
}

public class FrameData
{
    public Mat Frame { get; set; }
    public int FrameNumber { get; set; }
}

public class SceneAnalysisData
{
    public readonly object LockObject = new object();
    public Mat PreviousFrame { get; set; }
    public Mat BestFrame { get; set; }
    public int SceneNumber { get; set; }
    public int LastSceneFrame { get; set; }
    public readonly int CooldownFrames;

    public SceneAnalysisData(int cooldownFrames)
    {
        CooldownFrames = cooldownFrames;
        LastSceneFrame = -cooldownFrames; // Tillåt första scenen direkt
    }
}
