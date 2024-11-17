# Azure-AI-Translator-NET

## Project Overview

This project demonstrates the use of Azure OpenAI services in a .NET 8 Console Application. 
The solution consists of a translation service (`TranslationService`) and a test project to validate its functionality. 
The application integrates with Azure OpenAI to provide text and document translation capabilities.

---

> ⚠ **Disclaimer**: This project uses Azure OpenAI services, which may incur costs. Please review Azure's pricing and terms to understand potential charges associated with the use of this service. ⚠

---

## Features

The program provides the following functionalities:

1. **Extract Content from a Web Page**: Given a URL, the program fetches the page content and extracts all paragraphs.
2. **Translate Extracted Text**: Each paragraph obtained from the web page is translated into the target language specified by the user.
3. **Translate Microsoft Word Documents**: Translates the content of a Microsoft Word (.docx) file into the specified target language.

This project demonstrates how to integrate Azure OpenAI services into a .NET 8 application for web content extraction, document translation, and text processing.

---

## Solution Structure

- **AzureAI.Translator**: Main project containing the console application and the translation service.
  - `Services/TranslationService.cs`: Implements the translation logic using Azure OpenAI.
  - `Resources/sample.docx`: Sample document for testing translation features.
  - `Program.cs`: Entry point of the console application.
  
- **AzureAI.Translator.Tests**: Unit test project for testing the functionalities of the `TranslationService`.
  - `TranslationTests.cs`: Contains unit tests for the translation logic.

---

## Prerequisites

- **.NET SDK**: Ensure .NET 8 SDK is installed. You can download it from [dotnet.microsoft.com](https://dotnet.microsoft.com/).
- **Azure Account**: An Azure account with access to OpenAI services.
- **Environment Setup**: The `.env` file must be configured with Azure credentials.

---

## Environment Setup

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/frmpiccolo/azure-ai-translator-net.git
   cd azure-ai-translator-net
   ```

2. **Install .NET SDK**:
   Ensure that .NET 8 is installed on your system. Verify by running:
   ```bash
   dotnet --version
   ```

3. **Configure Environment Variables**:
   - Create a `.env` file in the root directory of the project.
   - Add the following content to the `.env` file, replacing `your_api_key` and `your_endpoint_url` with the actual values:
     ```env
     AZURE_OPENAI_API_KEY=your_api_key
     AZURE_OPENAI_ENDPOINT=your_endpoint_url
     DEFAULT_TARGET_LANGUAGE=pt-br # Set to your preferred target language (e.g., pt-br for Brazilian Portuguese, en for English, es for Spanish)
     ```

4. **Restore Dependencies**:
   Run the following command in the project directory to restore NuGet packages:
   ```bash
   dotnet restore
   ```

---

## Azure Setup

### Step 1: Create a Resource Group in Azure

1. Go to the [Azure Portal](https://portal.azure.com/).
2. In the left-hand menu, select **Resource groups**.
3. Click **Create**.
4. Enter the **Subscription** and **Resource group name** (e.g., `azure-ai-translator-rg`).
5. Choose a **Region** close to your target audience or application.
6. Click **Review + create**, then **Create**.

### Step 2: Create an Azure OpenAI Service

1. In the [Azure Portal](https://portal.azure.com/), go to **Create a resource**.
2. Search for **Azure OpenAI** and select **Azure OpenAI**.
3. Click **Create**.
4. Select the **Subscription** and **Resource group** created in the previous step.
5. Choose a **Region** and enter a **Name** for the service (e.g., `azure-openai-service`).
6. Review and create the service.

### Step 3: Deploy the GPT-4 Model

1. Go to the newly created Azure OpenAI resource.
2. In the left-hand menu, select **Deployments**.
3. Click **+ Create**.
4. Choose the **Model** (e.g., `gpt-4o-mini`) and give the **Deployment** a name (e.g., `gpt-4o-mini`).
5. Set any required configurations and limits as per your project needs.
6. Click **Create** to deploy the model.

### Step 4: Get the Endpoint and API Key

1. Go to the **Keys and Endpoint** section of your Azure OpenAI service.
2. Copy the **Endpoint URL** and **Key**.

---

## Running the Program

1. **Build the Solution**:
   Navigate to the solution directory and build the project:
   ```bash
   dotnet build
   ```

2. **Run the Application**:
   Execute the console application:
   ```bash
   dotnet run --project AzureAI.Translator
   ```

---

## Testing

1. **Run Tests**:
   To run unit tests, use the following command:
   ```bash
   dotnet test
   ```

2. **Test Files**:
   Ensure test files like `sample.docx` and the `.env` file are correctly configured for the tests to execute successfully.

---

## Customization

To modify the default target language or adjust rate limits, edit the `TranslationService` class in the main project (`AzureAI.Translator`).

---

> ⚠ **Disclaimer**: Using Azure OpenAI services may incur costs. Monitor usage and review pricing on the [Azure Portal](https://portal.azure.com/). ⚠

---

Let me know if you need help customizing this further!