using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace _4_SpeechToText
{
    class Program
    {
        async static Task Main(string[] args)
        {
            var speechConfig = SpeechConfig.FromSubscription("c532071dea994dc99f463d04a8b38437", "southcentralus");

            Console.WriteLine("Speech to Text using Microphone");
            await FromMic(speechConfig);

            Console.WriteLine("\nSpeech to Text using File");
            await FromFile(speechConfig);

            Console.WriteLine("\nSpeech to Text using Stream");
            await FromStream(speechConfig);

            Console.ReadLine();
        }

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
            using var audioConfig = AudioConfig.FromWavFileInput("test.wav");
            using var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

            var result = await recognizer.RecognizeOnceAsync();
            Console.WriteLine($"RECOGNIZED: Text={result.Text}");
        }

        async static Task FromStream(SpeechConfig speechConfig)
        {
                var reader = new BinaryReader(File.OpenRead("test.wav"));
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
         }        
    }
}
