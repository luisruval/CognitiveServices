using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace _4_SpeechToText
{
    class Program
    {
        async static Task FromMic(SpeechConfig speechConfig)
        {
            using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            Console.WriteLine("Speak into your microphone.");
            var result = await recognizer.RecognizeOnceAsync();
            Console.WriteLine($"RECOGNIZED: Text={result.Text}");
        }

        async static Task FromFile(SpeechConfig speechConfig)
        {
            using var audioConfig = AudioConfig.FromWavFileInput("PathToFile.wav");
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            var result = await recognizer.RecognizeOnceAsync();
            Console.WriteLine($"RECOGNIZED: Text={result.Text}");
        }

        async static Task FromStream1(SpeechConfig speechConfig)
        {
            var reader = new BinaryReader(File.OpenRead("PathToFile.wav"));
            using var audioInputStream = AudioInputStream.CreatePushStream();
            using var audioConfig = AudioConfig.FromStreamInput(audioInputStream);
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            byte[] readBytes;
            do
            {
                readBytes = reader.ReadBytes(1024);
                audioInputStream.Write(readBytes, readBytes.Length);
            } while (readBytes.Length > 0);

            var result = await recognizer.RecognizeOnceAsync();
            Console.WriteLine($"RECOGNIZED: Text={result.Text}");
        }

        async static Task FromStream2(SpeechConfig speechConfig)
        {
            var reader = new BinaryReader(File.OpenRead("PathToFile.wav"));
            using var audioInputStream = AudioInputStream.CreatePushStream();
            using var audioConfig = AudioConfig.FromStreamInput(audioInputStream);
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            byte[] readBytes;
            do
            {
                readBytes = reader.ReadBytes(1024);
                audioInputStream.Write(readBytes, readBytes.Length);
            } while (readBytes.Length > 0);

            var result = await recognizer.RecognizeOnceAsync();

            switch (result.Reason)
            {
                case ResultReason.RecognizedSpeech:
                    Console.WriteLine($"RECOGNIZED: Text={result.Text}");
                    break;
                case ResultReason.NoMatch:
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                    break;
                case ResultReason.Canceled:
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                    break;
            }

            Console.WriteLine($"RECOGNIZED: Text={result.Text}");
        }

        async static Task FromStream3(SpeechConfig speechConfig)
        {
            var reader = new BinaryReader(File.OpenRead("PathToFile.wav"));
            using var audioInputStream = AudioInputStream.CreatePushStream();
            using var audioConfig = AudioConfig.FromWavFileInput("YourAudioFile.wav");
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);
            var stopRecognition = new TaskCompletionSource<int>();

            recognizer.Recognizing += (s, e) =>
            {
                Console.WriteLine($"RECOGNIZING: Text={e.Result.Text}");
            };

            recognizer.Recognized += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    Console.WriteLine($"RECOGNIZED: Text={e.Result.Text}");
                }
                else if (e.Result.Reason == ResultReason.NoMatch)
                {
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
            };

            recognizer.Canceled += (s, e) =>
            {
                Console.WriteLine($"CANCELED: Reason={e.Reason}");

                if (e.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                    Console.WriteLine($"CANCELED: Did you update the subscription info?");
                }

                stopRecognition.TrySetResult(0);
            };

            recognizer.SessionStopped += (s, e) =>
            {
                Console.WriteLine("\n    Session stopped event.");
                stopRecognition.TrySetResult(0);
            };

            await recognizer.StartContinuousRecognitionAsync();
            // Waits for completion. Use Task.WaitAny to keep the task rooted.
            Task.WaitAny(new[] { stopRecognition.Task });

            // make the following call at some point to stop recognition.
            // await recognizer.StopContinuousRecognitionAsync();

            // Habilitar modo de dictado
            speechConfig.EnableDictation();

            // Cambiar el idioma de origen
            speechConfig.SpeechRecognitionLanguage = "es-MX";

            // Mejora la precision del reconocimiento
            var phraseList = PhraseListGrammar.FromRecognizer(recognizer);
            phraseList.AddPhrase("Supercalifragilisticexpialidocious");
            phraseList.Clear();



            byte[] readBytes;
            do
            {
                readBytes = reader.ReadBytes(1024);
                audioInputStream.Write(readBytes, readBytes.Length);
            } while (readBytes.Length > 0);

            var result = await recognizer.RecognizeOnceAsync();

            switch (result.Reason)
            {
                case ResultReason.RecognizedSpeech:
                    Console.WriteLine($"RECOGNIZED: Text={result.Text}");
                    break;
                case ResultReason.NoMatch:
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                    break;
                case ResultReason.Canceled:
                    var cancellation = CancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you update the subscription info?");
                    }
                    break;
            }

            Console.WriteLine($"RECOGNIZED: Text={result.Text}");
        }

        async static Task Main(string[] args)
        {
            var speechConfig = SpeechConfig.FromSubscription("<paste-your-subscription-key>", "<paste-your-region>");
            await FromMic(speechConfig);
            // await FromStream(speechConfig);
            // await FromFile(speechConfig);

        }


    }
}
