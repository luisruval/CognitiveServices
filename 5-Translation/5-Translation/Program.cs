using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;

namespace _5_Translation
{
    public class Program
    {
        static readonly string SPEECH__SUBSCRIPTION__KEY = "YourSuscriptionKey"; // Inserta tu subscription key

        static readonly string SPEECH__SERVICE__REGION = "southcentralus"; // Cambia si utilizas otra region

        static Task Main() => TranslateSpeechAsync();

        // Traduccion de audios
        static async Task TranslateSpeechAsync()
        {
            var translationConfig =
                SpeechTranslationConfig.FromSubscription(SPEECH__SUBSCRIPTION__KEY, SPEECH__SERVICE__REGION);

            var fromLanguage = "es-MX";
            var toLanguages = new List<string> { "en", "fr", "de" };
            translationConfig.SpeechRecognitionLanguage = fromLanguage;
            toLanguages.ForEach(translationConfig.AddTargetLanguage);

            using var recognizer = new TranslationRecognizer(translationConfig);

            Console.Write($"Say something in '{fromLanguage}' and ");
            Console.WriteLine($"we'll translate into '{string.Join("', '", toLanguages)}'.\n");

            var result = await recognizer.RecognizeOnceAsync();
            if (result.Reason == ResultReason.TranslatedSpeech)
            {
                Console.WriteLine($"Recognized: \"{result.Text}\":");
                foreach (var (language, translation) in result.Translations)
                {
                    Console.WriteLine($"Translated into '{language}': {translation}");
                }
            }
        }
    }
}
