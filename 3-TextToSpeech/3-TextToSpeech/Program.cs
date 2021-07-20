using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace _3_TextToSpeech
{
 
    public class Program
    {
        static async Task Main()
        {
            await SynthesizeAudioAsync();
        }

        static async Task SynthesizeAudioAsync1()
        {
            var config = SpeechConfig.FromSubscription("YourSubscriptionKey", "YourServiceRegion");
        }
        static async Task SynthesizeAudioAsync2()
        {
            var config = SpeechConfig.FromSubscription("YourSubscriptionKey", "YourServiceRegion");
            using var audioConfig = AudioConfig.FromWavFileOutput("path/to/write/file.wav");
        }

        static async Task SynthesizeAudioAsync3()
        {
            var config = SpeechConfig.FromSubscription("YourSubscriptionKey", "YourServiceRegion");
            using var audioConfig = AudioConfig.FromWavFileOutput("path/to/write/file.wav");
            using var synthesizer = new SpeechSynthesizer(config, audioConfig);
            await synthesizer.SpeakTextAsync("A simple test to write to a file.");
        }

        static async Task SynthesizeAudioAsync4()
        {
            var config = SpeechConfig.FromSubscription("YourSubscriptionKey", "YourServiceRegion");
            using var synthesizer = new SpeechSynthesizer(config);
            await synthesizer.SpeakTextAsync("Synthesizing directly to speaker output.");
        }

        static async Task SynthesizeAudioAsync5()
        {
            var config = SpeechConfig.FromSubscription("YourSubscriptionKey", "YourServiceRegion");
            using var synthesizer = new SpeechSynthesizer(config, null);

            var result = await synthesizer.SpeakTextAsync("Getting the response as an in-memory stream.");
            using var stream = AudioDataStream.FromResult(result);
        }

        static async Task SynthesizeAudioAsync()
        {
            var config = SpeechConfig.FromSubscription("YourSubscriptionKey", "YourServiceRegion");
            config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff24Khz16BitMonoPcm);

            using var synthesizer = new SpeechSynthesizer(config, null);
            var result = await synthesizer.SpeakTextAsync("Customizing audio output format.");

            using var stream = AudioDataStream.FromResult(result);
            await stream.SaveToWaveFileAsync("path/to/write/file.wav");
        }



    }
}
