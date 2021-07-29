using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Speaker;

namespace _6_SpeakerRecognition
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string subscriptionKey = "19807553ff834f218ac29a25f283ce81"; //YourSubscriptionKey
            string region = "southcentralus"; // La region donde esta tu servicio de Speech
            var config = SpeechConfig.FromSubscription(subscriptionKey, region);

            // persist profileMapping if you want to store a record of who the profile is
            var profileMapping = new Dictionary<string, string>();

            await VerificationEnroll(config, profileMapping);

            // await VerificationEnroll2(config, profileMapping);

            /*
            var profileNames = new List<string>() { "Your name", "A friend's name" };

            var enrolledProfiles = await IdentificationEnroll(config, profileNames, profileMapping);
            await SpeakerIdentification(config, enrolledProfiles, profileMapping);

            foreach (var profile in enrolledProfiles)
            {
                profile.Dispose();
            }
            */

            Console.ReadLine();
        }

        public static async Task VerificationEnroll(SpeechConfig config, Dictionary<string, string> profileMapping)
        {
            using (var client = new VoiceProfileClient(config))
            using (var profile = await client.CreateProfileAsync(VoiceProfileType.TextIndependentVerification, "en-us"))
            {
                using (var audioInput = AudioConfig.FromDefaultMicrophoneInput())
                {
                    Console.WriteLine($"Enrolling profile id {profile.Id}.");
                    // give the profile a human-readable display name
                    profileMapping.Add(profile.Id, "Your Name");

                    VoiceProfileEnrollmentResult result = null;
                    while (result is null || result.RemainingEnrollmentsSpeechLength > TimeSpan.Zero)
                    {
                        Console.WriteLine("Continue speaking to add to the profile enrollment sample.");
                        result = await client.EnrollProfileAsync(profile, audioInput);
                        Console.WriteLine($"Remaining enrollment audio time needed: {result.RemainingEnrollmentsSpeechLength}");
                        Console.WriteLine("");
                    }

                    if (result.Reason == ResultReason.EnrolledVoiceProfile)
                    {
                        await SpeakerVerify(config, profile, profileMapping);
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = VoiceProfileEnrollmentCancellationDetails.FromResult(result);
                        Console.WriteLine($"CANCELED {profile.Id}: ErrorCode={cancellation.ErrorCode} ErrorDetails={cancellation.ErrorDetails}");
                    }
                }
            }
        }

        public static async Task VerificationEnroll2(SpeechConfig config, Dictionary<string, string> profileMapping)
        {
            using (var client = new VoiceProfileClient(config))
            using (var profile = await client.CreateProfileAsync(VoiceProfileType.TextDependentVerification, "en-us"))
            {
                using (var audioInput = AudioConfig.FromDefaultMicrophoneInput())
                {
                    Console.WriteLine($"Enrolling profile id {profile.Id}.");
                    // give the profile a human-readable display name
                    profileMapping.Add(profile.Id, "Your Name");

                    VoiceProfileEnrollmentResult result = null;
                    while (result is null || result.RemainingEnrollmentsCount > 0)
                    {
                        Console.WriteLine("Speak the passphrase, \"My voice is my passport, verify me.\"");
                        result = await client.EnrollProfileAsync(profile, audioInput);
                        Console.WriteLine($"Remaining enrollments needed: {result.RemainingEnrollmentsCount}");
                        Console.WriteLine("");
                    }

                    if (result.Reason == ResultReason.EnrolledVoiceProfile)
                    {
                        await SpeakerVerify(config, profile, profileMapping);
                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = VoiceProfileEnrollmentCancellationDetails.FromResult(result);
                        Console.WriteLine($"CANCELED {profile.Id}: ErrorCode={cancellation.ErrorCode} ErrorDetails={cancellation.ErrorDetails}");
                    }
                }
            }
        }


        public static async Task SpeakerVerify(SpeechConfig config, VoiceProfile profile, Dictionary<string, string> profileMapping)
        {
            var speakerRecognizer = new SpeakerRecognizer(config, AudioConfig.FromDefaultMicrophoneInput());
            var model = SpeakerVerificationModel.FromProfile(profile);

            Console.WriteLine("Speak the passphrase to verify: \"My voice is my passport, please verify me.\"");
            var result = await speakerRecognizer.RecognizeOnceAsync(model);
            Console.WriteLine($"Verified voice profile for speaker {profileMapping[result.ProfileId]}, score is {result.Score}");
        }

        public static async Task<List<VoiceProfile>> IdentificationEnroll(SpeechConfig config, List<string> profileNames, Dictionary<string, string> profileMapping)
        {
            List<VoiceProfile> voiceProfiles = new List<VoiceProfile>();
            using (var client = new VoiceProfileClient(config))
            {
                foreach (string name in profileNames)
                {
                    using (var audioInput = AudioConfig.FromDefaultMicrophoneInput())
                    {
                        var profile = await client.CreateProfileAsync(VoiceProfileType.TextIndependentIdentification, "en-us");
                        Console.WriteLine($"Creating voice profile for {name}.");
                        profileMapping.Add(profile.Id, name);

                        VoiceProfileEnrollmentResult result = null;
                        while (result is null || result.RemainingEnrollmentsSpeechLength > TimeSpan.Zero)
                        {
                            Console.WriteLine($"Continue speaking to add to the profile enrollment sample for {name}.");
                            result = await client.EnrollProfileAsync(profile, audioInput);
                            Console.WriteLine($"Remaining enrollment audio time needed: {result.RemainingEnrollmentsSpeechLength}");
                            Console.WriteLine("");
                        }
                        voiceProfiles.Add(profile);
                    }
                }
            }
            return voiceProfiles;
        }

        public static async Task SpeakerIdentification(SpeechConfig config, List<VoiceProfile> voiceProfiles, Dictionary<string, string> profileMapping)
        {
            var speakerRecognizer = new SpeakerRecognizer(config, AudioConfig.FromDefaultMicrophoneInput());
            var model = SpeakerIdentificationModel.FromProfiles(voiceProfiles);

            Console.WriteLine("Speak some text to identify who it is from your list of enrolled speakers.");
            var result = await speakerRecognizer.RecognizeOnceAsync(model);
            Console.WriteLine($"The most similar voice profile is {profileMapping[result.ProfileId]} with similarity score {result.Score}");
        }


    }
}
