using AzureAI.Translator.Services;

try
{
    // Instantiate the translation service
    var translationService = new TranslationService();

    // Input text to be translated
    var inputText = "This text should be translated from English to Brazilian Portuguese.";
    Console.WriteLine($"Input text: {inputText}");

    if (!string.IsNullOrEmpty(inputText))
    {
        // Translate the input text
        var translatedText = await translationService.TranslateTextAsync(inputText);
        Console.WriteLine($"Translated Text: {translatedText}");
    }

    Console.WriteLine();

    // URL to extract content and translate
    var url = "https://azure.microsoft.com/en-us/blog/introducing-o1-openais-new-reasoning-model-series-for-developers-and-enterprises-on-azure/";

    if (!string.IsNullOrEmpty(url))
    {
        // Extract content (paragraphs) from the URL
        var paragraphs = await translationService.ExtractContentFromUrlAsync(url);
        Console.WriteLine($"Extracted {paragraphs.Count} paragraphs from the URL.\n");
        Console.WriteLine("Translating paragraphs...\n");

        // Translate each extracted paragraph
        foreach (var paragraph in paragraphs)
        {
            var translatedParagraph = await translationService.TranslateTextAsync(paragraph);
            Console.WriteLine(translatedParagraph);
        }

        Console.WriteLine("URL translation complete.\n");
    }

    // Path to the Word document in the Resources directory
    var inputFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "sample.docx");
    Console.WriteLine($"Translating Word document: {inputFile}");

    if (File.Exists(inputFile))
    {
        // Translate the Word document
        await translationService.TranslateWordDocumentAsync(inputFile);
        Console.WriteLine("Document translation completed.");
    }
    else
    {
        Console.WriteLine($"The file {inputFile} was not found.");
    }
}
catch (Exception ex)
{
    // Display a detailed error message
    Console.WriteLine($"An error occurred: {ex.Message}");
}
finally
{
    Console.WriteLine();
    Console.WriteLine("Press any key to exit...");
    Console.Read();
}