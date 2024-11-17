using AzureAI.Translator.Services;

namespace AzureAI.Translator.Tests
{
    /// <summary>
    /// Translation tests for the TranslationService class.
    /// </summary>
    public class TranslationTests
    {
        private readonly TranslationService _translationService;

        /// <summary>
        /// Constructs a new instance of the TranslationTests class.
        /// </summary>
        public TranslationTests()
        {
            // Load the .env file from the solution root directory
            var solutionRoot = Directory.GetCurrentDirectory();
            if (solutionRoot != null)
            {
                var envFilePath = Path.Combine(solutionRoot, ".env");
                DotNetEnv.Env.Load(envFilePath); // Load environment variables from the .env file
            }

            // Initialize the translation service for testing
            _translationService = new TranslationService();
        }

        /// <summary>
        /// Tests the TranslateTextAsync method to ensure that it returns translated text.
        /// </summary>
        /// <returns>
        /// Returns a task that represents the asynchronous operation.
        /// </returns>
        [Fact]
        public async Task TranslateTextAsync_ShouldReturnTranslatedText_WhenValidTextIsGiven()
        {
            // Arrange
            // Input text to be translated
            var inputText = "Hello, how are you?";

            // Act
            // Perform the translation
            var translatedText = await _translationService.TranslateTextAsync(inputText);

            // Assert
            // Verify that the translated text is not null or empty
            Assert.False(string.IsNullOrEmpty(translatedText), "The translated text should not be empty.");
        }

        /// <summary>
        /// Tests the TranslateTextAsync method to ensure that it returns an empty string for empty input.
        /// </summary>
        /// <returns>
        /// Returns a task that represents the asynchronous operation.
        /// </returns>
        [Fact]
        public async Task TranslateTextAsync_ShouldReturnEmpty_WhenTextIsEmpty()
        {
            // Arrange
            // Provide an empty string as input text
            var inputText = string.Empty;

            // Act
            // Perform the translation
            var translatedText = await _translationService.TranslateTextAsync(inputText);

            // Assert
            // Verify that the result is an empty string
            Assert.Equal(string.Empty, translatedText);
        }

        /// <summary>
        /// Tests the ExtractContentFromUrlAsync method to ensure that it returns a non-empty list of paragraphs.
        /// </summary>
        /// <returns>
        /// Returns a task that represents the asynchronous operation.
        /// </returns>
        [Fact]
        public async Task ExtractContentFromUrlAsync_ShouldReturnNonEmptyList_WhenValidUrlIsProvided()
        {
            // Arrange
            // Provide a valid URL for extracting content
            var url = "https://example.com"; // Replace with a valid test URL if needed

            // Act
            // Extract content (paragraphs) from the URL
            var paragraphs = await _translationService.ExtractContentFromUrlAsync(url);

            // Assert
            // Verify that the extracted paragraphs are not empty
            Assert.NotEmpty(paragraphs);
        }

        /// <summary>
        /// Tests the TranslateWordDocumentAsync method to ensure that it creates a translated document.
        /// </summary>
        /// <returns>
        /// Returns a task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="FileNotFoundException"></exception>
        [Fact]
        public async Task TranslateWordDocumentAsync_ShouldCreateTranslatedFile_WhenValidDocumentIsProvided()
        {
            // Arrange
            // Input file path (sample document)
            var inputFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "sample.docx");
            var targetLanguage = "es"; // Example target language: Spanish
            var outputFile = inputFile.Replace(".docx", $"_{targetLanguage}.docx");

            // Ensure the input file exists for the test
            if (!File.Exists(inputFile))
            {
                throw new FileNotFoundException($"The test document '{inputFile}' was not found.");
            }

            // Act
            // Perform the document translation
            await _translationService.TranslateWordDocumentAsync(inputFile, targetLanguage);

            // Assert
            // Verify that the translated document was created
            Assert.True(File.Exists(outputFile), "The translated document was not created.");

            // Clean up: Remove the translated document after the test
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }
        }
    }
}