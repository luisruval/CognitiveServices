using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace LUIS_TestApp
{
    class Program
    {
        // Prediction key must be assigned in LUIS portal
        private static string predictionKey = "PASTE_YOUR_LUIS_PREDICTION_SUBSCRIPTION_KEY_HERE";
   

        // Endpoint URL example value = "https://YOUR-RESOURCE-NAME.cognitiveservices.azure.com"
        private static string predictionEndpoint = "PASTE_YOUR_LUIS_PREDICTION_ENDPOINT_HERE";


        // App Id example value = "df67dcdb-c37d-46af-88e1-8b97951ca1c2"
        private static string appId = "PASTE_YOUR_LUIS_APP_ID_HERE";

        private static string QueryToSend;

        static void Main(string[] args)
        {
            Console.WriteLine("LUIS Console Test App");
            Console.Write("Type the query to send: ");

            QueryToSend = Console.ReadLine();
            //////////
            Console.WriteLine("\n\nUsing the web request");
            MakeRequest(predictionKey, predictionEndpoint, appId, QueryToSend);

            Console.WriteLine("\n\nUsing the SDK");

            var predictionResult = GetPredictionAsync().Result;

            var prediction = predictionResult.Prediction;

            // Display query
            Console.WriteLine("Query:'{0}'", predictionResult.Query);
            Console.WriteLine("TopIntent :'{0}' ", prediction.TopIntent);

            foreach (var i in prediction.Intents)
            {
                Console.WriteLine(string.Format("{0}:{1}", i.Key, i.Value.Score));
            }

            foreach (var e in prediction.Entities)
            {
                Console.WriteLine(string.Format("{0}:{1}", e.Key, e.Value));
            }


            Console.WriteLine("Press ENTER to exit...");
            
            Console.ReadLine();
        }

        static async void MakeRequest(string predictionKey, string predictionEndpoint, string appId, string utterance)
        {          
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // The request header contains your subscription key
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", predictionKey);

            // The "q" parameter contains the utterance to send to LUIS
            queryString["query"] = utterance;

            // These optional request parameters are set to their default values
            // queryString["verbose"] = "true";
            // queryString["show-all-intents"] = "true";
            // queryString["staging"] = "false";
            // queryString["timezoneOffset"] = "0";

            var predictionEndpointUri = String.Format("{0}luis/prediction/v3.0/apps/{1}/slots/production/predict?{2}", predictionEndpoint, appId, queryString);

            // Remove these before updating the article.
            Console.WriteLine("endpoint: " + predictionEndpoint);
            Console.WriteLine("appId: " + appId);
            Console.WriteLine("queryString: " + queryString);
            Console.WriteLine("endpointUri: " + predictionEndpointUri);

            var response = await client.GetAsync(predictionEndpointUri);

            var strResponseContent = await response.Content.ReadAsStringAsync();

            // Display the JSON result from LUIS.
            Console.WriteLine(strResponseContent.ToString());
        }

        static async Task<PredictionResponse> GetPredictionAsync()
        {

            // Get client 
            using (var luisClient = CreateClient())
            {

                var requestOptions = new PredictionRequestOptions
                {
                    DatetimeReference = DateTime.Parse("2019-01-01"),
                    PreferExternalEntities = true
                };

                var predictionRequest = new PredictionRequest
                {
                    Query = QueryToSend,
                    Options = requestOptions
                };

                // get prediction
                return await luisClient.Prediction.GetSlotPredictionAsync(
                    Guid.Parse(appId),
                    slotName: "production",
                    predictionRequest,
                    verbose: true,
                    showAllIntents: true,
                    log: true);
            }
        }

        static LUISRuntimeClient CreateClient()
        {
            var credentials = new ApiKeyServiceClientCredentials(predictionKey);
            var luisClient = new LUISRuntimeClient(credentials, new System.Net.Http.DelegatingHandler[] { })
            {
                Endpoint = predictionEndpoint
            };

            return luisClient;

        }
    }
}
