namespace BulkThumbnailCreator;

using Emgu.CV;
using Emgu.CV.CvEnum;

public class SceneDetector
{
    public static Task DetectAndSaveBestFrames(string videoPath, string outputFolder)
    {
        Directory.CreateDirectory(outputFolder);
        var path = Path.GetRelativePath(Directory.GetCurrentDirectory(), videoPath);

        using (var capture = new VideoCapture(path))
        {
            if (!capture.IsOpened)
            {
                Console.WriteLine("[ERROR] Kunde inte öppna video!");
                return Task.CompletedTask;
            }

            double fps = capture.Get(CapProp.Fps);
            int cooldownFrames = (int)(fps * 2.5); // 2.5 sekunder mellan scener
            Console.WriteLine($"[INFO] Video FPS: {fps} | Cooldown: {cooldownFrames} frames");

            Mat prevFrame = null;
            Mat bestFrame = null;
            int sceneNumber = 0;
            int frameCount = 0;
            int lastSceneFrame = -cooldownFrames; // Tillåt första scenen direkt

            while (true)
            {
                using (Mat currentFrame = new Mat())
                {
                    if (!capture.Read(currentFrame))
                    {
                        Console.WriteLine("[DEBUG] Slut på video");
                        break;
                    }

                    frameCount++;

                    // Hoppa över svarta/tomma bildrutor
                    if (currentFrame.IsEmpty || IsBlackFrame(currentFrame))
                    {
                        Console.WriteLine($"[DEBUG] Skipping frame {frameCount} (black/empty)");
                        continue;
                    }

                    // Första giltiga bilden
                    if (prevFrame == null)
                    {
                        prevFrame = currentFrame.Clone();
                        bestFrame = currentFrame.Clone();
                        Console.WriteLine($"[INIT] Första bilden laddad (Frame {frameCount})");
                        continue;
                    }

                    // Beräkna förändring
                    double change = CalculateFrameDifference(prevFrame, currentFrame);
                    Console.WriteLine($"[ANALYS] Frame {frameCount} | Change: {change:F2}");

                    // Kolla cooldown
                    bool isCooldownOver = (frameCount - lastSceneFrame) >= cooldownFrames;

                    // Scenskifte med cooldown-check
                    if (change > 15 && isCooldownOver)
                    {
                        if (!IsBlackFrame(bestFrame)) // Dubbejkontroll
                        {
                            string outputPath = Path.Combine(outputFolder, $"scene_{sceneNumber}.jpg");
                            CvInvoke.Imwrite(outputPath, bestFrame);
                            Console.WriteLine($"[SAVE] Scene {sceneNumber} sparad (Frame {frameCount})");
                            sceneNumber++;
                            lastSceneFrame = frameCount;
                        }

                        bestFrame = currentFrame.Clone();
                    }

                    // Uppdatera bästa bilden
                    double currentScore = CalculateSharpness(currentFrame);
                    double bestScore = CalculateSharpness(bestFrame);

                    if (currentScore > bestScore)
                    {
                        bestFrame = currentFrame.Clone();
                        Console.WriteLine($"[BEST] Ny bästa bild (Frame {frameCount} | Score: {currentScore:F2})");
                    }

                    prevFrame = currentFrame.Clone();
                }
            }

            // Sista scenen (extra kvalitetskontroll)
            if (bestFrame != null && !bestFrame.IsEmpty && !IsBlackFrame(bestFrame))
            {
                string outputPath = Path.Combine(outputFolder, $"scene_{sceneNumber}.jpg");
                CvInvoke.Imwrite(outputPath, bestFrame);
                Console.WriteLine($"[FINAL] Sista scenen sparad");
            }
            return Task.CompletedTask;
        }
    }

    private static bool IsBlackFrame(Mat frame)
    {
        using (Mat gray = new Mat())
        {
            CvInvoke.CvtColor(frame, gray, ColorConversion.Bgr2Gray);
            double mean = CvInvoke.Mean(gray).V0;
            bool isBlack = mean < 10; // Justera vid behov
            if (isBlack) Console.WriteLine($"[BLACK] Mean: {mean:F2}");
            return isBlack;
        }
    }

    private static double CalculateFrameDifference(Mat a, Mat b)
    {
        using (Mat diff = new Mat())
        {
            CvInvoke.AbsDiff(a, b, diff);
            CvInvoke.CvtColor(diff, diff, ColorConversion.Bgr2Gray);
            CvInvoke.Threshold(diff, diff, 25, 255, ThresholdType.Binary);
            return CvInvoke.Mean(diff).V0;
        }
    }

    private static double CalculateSharpness(Mat frame)
    {
        using (Mat gray = new Mat())
        {
            CvInvoke.CvtColor(frame, gray, ColorConversion.Bgr2Gray);
            CvInvoke.Laplacian(gray, gray, DepthType.Cv64F);
            return CvInvoke.Mean(gray).V0;
        }
    }
}

//public class SceneDetector
//{
//    public static void DetectAndSaveBestFrames(
//        string videoPath,
//        string outputFolder,
//        double sceneChangeThreshold = 30, // Högre värde = färre scener
//        int minFramesBetweenScenes = 30     // Minsta antal bildrutor mellan scener
//    )
//    {

//        using (var capture = new VideoCapture(videoPath))
//        {
//            Mat prevFrame = null;
//            int sceneNumber = 0;
//            double bestFrameScore = 0;
//            Mat bestFrame = null;
//            int framesSinceLastScene = minFramesBetweenScenes; // Tvinga initial fördröjning

//            while (true)
//            {
//                Mat currentFrame = new Mat();
//                if (!capture.Read(currentFrame) || currentFrame.IsEmpty)
//                    break;

//                if (prevFrame != null)
//                {
//                    // Beräkna skillnad mellan bildrutor
//                    Mat diff = new Mat();
//                    CvInvoke.AbsDiff(prevFrame, currentFrame, diff);
//                    CvInvoke.CvtColor(diff, diff, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
//                    CvInvoke.Threshold(diff, diff, 25, 255, Emgu.CV.CvEnum.ThresholdType.Binary);
//                    double change = CvInvoke.Mean(diff).V0;

//                    // Kolla om tillräckligt många rutor har passerat sedan senaste scenen
//                    framesSinceLastScene++;

//                    // Scendetektering (endast om förändring > tröskel OCH tillräcklig fördröjning)
//                    if (change > sceneChangeThreshold && framesSinceLastScene >= minFramesBetweenScenes)
//                    {
//                        if (bestFrame != null)
//                        {
//                            string outputPath = Path.Combine(outputFolder, $"scene_{sceneNumber}.jpg");
//                            CvInvoke.Imwrite(outputPath, bestFrame);
//                            sceneNumber++;
//                        }
//                        bestFrameScore = 0;
//                        framesSinceLastScene = 0; // Återställ fördröjning
//                    }

//                    // Bedöm bildkvalitet (högre Laplacian = skarpare bild)
//                    Mat gray = new Mat();
//                    CvInvoke.CvtColor(currentFrame, gray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
//                    CvInvoke.Laplacian(gray, gray, Emgu.CV.CvEnum.DepthType.Cv64F);
//                    double score = CvInvoke.Mean(gray).V0;

//                    // Spara bästa bilden i nuvarande scen
//                    if (score > bestFrameScore)
//                    {
//                        bestFrameScore = score;
//                        bestFrame?.Dispose();
//                        bestFrame = currentFrame.Clone();
//                    }
//                }
//                prevFrame?.Dispose();
//                prevFrame = currentFrame.Clone();
//            }

//            // Sista scenen (om någon)
//            if (bestFrame != null)
//                CvInvoke.Imwrite(Path.Combine(outputFolder, $"scene_{sceneNumber}.jpg"), bestFrame);
//        }
//    }
//}

//public class SceneDetector
//{
//    public static void DetectScenes(string videoPath, string outputFolder)
//    {
//        using (var capture = new VideoCapture(videoPath))
//        {
//            Mat prevFrame = null;
//            int frameCount = 0;

//            while (true)
//            {
//                Mat currentFrame = new Mat();
//                capture.Read(currentFrame);
//                if (currentFrame.IsEmpty) break;

//                if (prevFrame != null)
//                {
//                    // Beräkna skillnaden mellan bildrutor
//                    Mat diff = new Mat();
//                    CvInvoke.AbsDiff(prevFrame, currentFrame, diff);

//                    // Konvertera till gråskala och applicera tröskelvärde
//                    Mat grayDiff = new Mat();
//                    CvInvoke.CvtColor(diff, grayDiff, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
//                    CvInvoke.Threshold(grayDiff, grayDiff, 25, 255, Emgu.CV.CvEnum.ThresholdType.Binary);

//                    // Beräkna hur många pixlar som förändrats
//                    double change = CvInvoke.Mean(grayDiff).V0;

//                    // Om förändringen är stor nog, spara bildrutan
//                    if (change > 30) // Justera tröskelvärdet efter behov
//                    {
//                        string outputPath = $"{outputFolder}/scene_{frameCount}.jpg";
//                        CvInvoke.Imwrite(outputPath, currentFrame);
//                    }
//                }

//                prevFrame = currentFrame.Clone();
//                frameCount++;
//            }
//        }
//    }
//}
