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

        private static string suscriptionKey = "YourSuscriptionKey";
        private static string serviceRegion = "YourServiceRegion";

        static async Task Main()
        {
            await SynthesizeAudioToSpeakerAsync();
            await SynthesizeAudioToFileAsync();
        }            

 

        static async Task SynthesizeAudioToSpeakerAsync()
        {
            var config = SpeechConfig.FromSubscription(suscriptionKey, serviceRegion);
            using var synthesizer = new SpeechSynthesizer(config);
            await synthesizer.SpeakTextAsync("Synthesizing directly to speaker output.");
        }

        static async Task SynthesizeAudioToFileAsync()
        {
            var config = SpeechConfig.FromSubscription(suscriptionKey, serviceRegion);
            config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Riff24Khz16BitMonoPcm);
  

            using var synthesizer = new SpeechSynthesizer(config, null);           

            var ssml = File.ReadAllText("ssml.xml");
            var resultssml = await synthesizer.SpeakSsmlAsync(ssml);

            using var stream = AudioDataStream.FromResult(resultssml);
            await stream.SaveToWaveFileAsync("output-test.wav");

        }



    }
}
