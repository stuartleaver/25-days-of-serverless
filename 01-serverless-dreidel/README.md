# Challenge 1: A Basic Function

![Spin the Dreidel!](https://res.cloudinary.com/jen-looper/image/upload/v1575132446/images/challenge-1_lriuc2.jpg)

## Serverless Dreidel!

🎶 "I had a little dreidel

I made it out of sand

And when I tried to spin it

It crumbled in my hand!" 🎶

Your first stop is Tel Aviv, Israel, where everybody is concerned about Hanukkah! Not only have all the dreidels been stolen, but so have all of the servers that could replicate spinning a top!

Have no fear, though: you have the capability to spin not only dreidels, but to spin up serverless applications that can spin a dreidel just as well as you can!

Your task for today: create a REST API endpoint that spins a dreidel and randomly returns נ (Nun), ג (Gimmel), ה (Hay), or ש (Shin). This sounds like a great opportunity to use a serverless function to create an endpoint that any application can call!

![dreidel spinning](https://media.giphy.com/media/3oxHQDYNRtgTKiYEBG/giphy.gif)

## Running

The function can be run within  Visual Studio Code using the Functions emulator by calling `http://localhost:7071/api/dreidel/spin`. Not forgetting to replace `localhost:7071` if it is different for you.

Or, it can be deployed to Azure:
1. Create a Resource Group - `az group create --location <a-location-close-to-you> --name DreidelSpinner_rg`
2. Create a Storage Account - `az storage account create --name dreidelspinner --location <a-location-close-to-you> --resource-group DreidelSpinner_rg --sku Standard_LRS`
3. Create a Function App - `az functionapp create --resource-group DreidelSpinner_rg --consumption-plan-location <a-location-close-to-you> --name DreidelSpinner --storage-account dreidelspinner --runtime dotnet`
4. Deploy the function through Visual Studio Code and the Azure Functions extension.
5. Call the function - `https://dreidelspinner.azurewebsites.net/api/dreidel/spin`

## Resources/Tools Used 🚀

A simple Function app should do it for this challenge. Here's how to get started creating on in Azure:

-   **[Visual Studio Code](https://code.visualstudio.com/?WT.mc_id=25daysofserverless-github-cxa)**
-   **[Azure Functions Extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions&WT.mc_id=25daysofserverless-github-cxa)**

## Next Steps 🏃

Learn more about serverless technologies with free training!

-   ✅ **[Serverless Free Courses](https://docs.microsoft.com/learn/browse/?term=azure%20functions&WT.mc_id=25daysofserverless-github-cxa)**

## More Resources ⭐️

-   ✅ **[Azure Functions documentation](https://docs.microsoft.com/azure/azure-functions/?WT.mc_id=25daysofserverless-github-cxa)**
-   ✅ **[Azure SDK for JavaScript Documentation](https://docs.microsoft.com/azure/javascript/?WT.mc_id=25daysofserverless-github-cxa)**
-   ✅ **[Create your first function using Visual Studio Code](https://docs.microsoft.com/azure/azure-functions/functions-create-first-function-vs-code?WT.mc_id=25daysofserverless-github-cxa)**
-   ✅ **[Free E-Book - Azure Serverless Computing Cookbook, Second Edition](https://azure.microsoft.com/resources/azure-serverless-computing-cookbook/?WT.mc_id=25daysofserverless-github-cxa)**
