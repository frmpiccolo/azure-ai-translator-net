using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DotNetEnv;
using HtmlAgilityPack;
using System.Text;
using System.Text.Json;

namespace AzureAI.Translator.Services
{
    /// <summary>
    /// Translation service that uses the Azure OpenAI API to translate text and documents.
    /// </summary>
    public class TranslationService
    {
        private string apiKey;
        private string endpoint;
        private string defaultTargetLanguage;

        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// Initializes the service with the API key and endpoint from environment variables.
        /// </summary>
        public TranslationService()
        {
            Env.Load();

            apiKey = Env.GetString("AZURE_OPENAI_API_KEY", "");
            endpoint = Env.GetString("AZURE_OPENAI_ENDPOINT", "");
            defaultTargetLanguage = Env.GetString("DEFAULT_TARGET_LANGUAGE", "pt-br");

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(endpoint))
            {
                throw new InvalidOperationException("API key or endpoint is not set in the environment variables.");
            }

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("api-key", apiKey);
        }

        /// <summary>
        /// Extracts content (paragraphs) from the specified URL asynchronously.
        /// </summary>
        /// <param name="url">The URL to extract content from.</param>
        /// <returns>A list of strings containing the content of each paragraph from the URL.</returns>
        public async Task<List<string>> ExtractContentFromUrlAsync(string url)
        {
            var paragraphs = new List<string>();
            try
            {
                var response = await client.GetStringAsync(url);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response);

                var paragraphNodes = htmlDoc.DocumentNode.SelectNodes("//p");
                if (paragraphNodes != null)
                {
                    foreach (var node in paragraphNodes)
                    {
                        paragraphs.Add(node.InnerText);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error accessing the URL: {e.Message}");
            }
            return paragraphs;
        }

        /// <summary>
        /// Translates the specified text using the Azure OpenAI API.
        /// </summary>
        /// <param name="text">The text to translate.</param>
        /// <param name="targetLanguage">The target language for the translation.</param>
        /// <param name="retries">The number of retries in case of failure.</param>
        /// <returns>The translated text.</returns>
        public async Task<string> TranslateTextAsync(string text, string? targetLanguage = null, int retries = 3)
        {
            if (string.IsNullOrEmpty(text))
            {
                Console.WriteLine("No text to translate.");
                return String.Empty;
            }

            targetLanguage ??= defaultTargetLanguage;

            var body = new
            {
                messages = new[]
                {
                new { role = "system", content = $"You are an AI assistant that translates text to {targetLanguage}." },
                new { role = "user", content = $"Translate the text: '{text}' to {targetLanguage}" }
            },
                max_tokens = 1000
            };

            var jsonRequestBody = JsonSerializer.Serialize(body);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            var requestUrl = $"{endpoint}/openai/deployments/gpt-4o-mini/chat/completions?api-version=2024-08-01-preview";

            for (int attempt = 0; attempt < retries; attempt++)
            {
                try
                {
                    var response = await client.PostAsync(requestUrl, content);
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    return ExtractTranslationFromResponse(responseBody);
                }
                catch (HttpRequestException e) when (e.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    Console.WriteLine("Error 429: Too many requests. Retrying...");
                    await Task.Delay(10000); // Wait 10 seconds before retrying
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error making request to the API: {e.Message}");
                    break;
                }
            }
            Console.WriteLine("Failed to translate after several attempts.");
            return "";
        }

        /// <summary>
        /// Extracts the translated text from the API response.
        /// </summary>
        /// <param name="responseBody">The response body from the API.</param>
        /// <returns>The translated text.</returns>
        private string ExtractTranslationFromResponse(string responseBody)
        {
            try
            {
                var responseJson = JsonSerializer.Deserialize<JsonElement>(responseBody);

                if (responseJson.TryGetProperty("choices", out var choices) && choices[0].TryGetProperty("message", out var message))
                {
                    return message.GetProperty("content").GetString()?.Trim() ?? "";
                }
                else
                {
                    Console.WriteLine("No translation found in the response.");
                    return "";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting translation from response: {ex.Message}");
                return "";
            }
        }

        /// <summary>
        /// Translates a Word document asynchronously.
        /// </summary>
        /// <param name="inputFile">The path to the input Word document.</param>
        /// <param name="targetLanguage">The target language for the translation.</param>
        public async Task TranslateWordDocumentAsync(string inputFile, string? targetLanguage = null)
        {
            targetLanguage ??= defaultTargetLanguage;

            try
            {
                using (var doc = WordprocessingDocument.Open(inputFile, true))
                {
                    if (doc.MainDocumentPart == null)
                    {
                        Console.WriteLine($"The document '{inputFile}' does not contain a main part.");
                        return;
                    }

                    var body = doc.MainDocumentPart.Document.Body;

                    if (body == null)
                    {
                        Console.WriteLine($"The document '{inputFile}' does not contain a body.");
                        return;
                    }

                    var newDoc = new Document();
                    foreach (var paragraph in body.Elements<Paragraph>())
                    {
                        var text = paragraph.InnerText;
                        var translatedText = await TranslateTextAsync(text, targetLanguage);

                        if (!string.IsNullOrEmpty(translatedText))
                        {
                            newDoc.AppendChild(new Paragraph(new Run(new Text(translatedText))));
                        }

                        await Task.Delay(2000); // Pause to respect rate limits
                    }

                    string outputFile = $"{inputFile.Replace(".docx", "")}_{targetLanguage}.docx";

                    using (var newDocPackage = WordprocessingDocument.Create(outputFile, WordprocessingDocumentType.Document))
                    {
                        newDocPackage.AddMainDocumentPart();

                        if (newDocPackage.MainDocumentPart == null)
                        {
                            Console.WriteLine($"The new document '{outputFile}' does not contain a main part.");
                            return;
                        }

                        newDocPackage.MainDocumentPart.Document = newDoc;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error processing the document: {e.Message}");
            }
        }
    }
}