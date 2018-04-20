# HandsOnLabs - Customer Reviews App with Cognitive Services and Azure Functions #

The sample showcases the new Azure Functions tooling. It has a website where customers submit product reviews, stored in Azure storage blobs and CosmosDB; and an Azure Function to perform automated moderation of the customer reviews using Microsoft Cognitive Services. It uses an Azure storage queue to decouple the website from the function.

## Sample Architecture ##

![](Media/Picture20.png)

_View of the architecture of the sample_

## Hands On Labs ##

- [Lab 01:](./doc/01%20-%20Portal) In this lab, you will create an Azure Function that monitors a blob container in Azure Storage for new images, and then performs automated analysis of the images using the Microsoft Cognitive Services Computer Vision.
  - [directly in Azure portal with Csharp script](doc/01%20-%20Portal/Azure%20Functions%20HOL%20(C%23).md)
  - [directly in Azure portal with Javascript](./doc/01%20-%20Portal/Azure%20Functions%20HOL%20(JavaScript).md)

- [Lab 00:](doc/00%20-%20Provision%20resources%20and%20Reset) In this lab, you will create assets listed in the architecture sample above with Azure Cloud Shell (Powershell). The created Azure Services are prerequisites to execute lab 02 and lab 03.
- [Lab 02:](doc/02%20-%20Visual%20Studio) In this lab, you willcreate an Azure Function to perform automated moderation of customer reviews using Microsoft Cognitive Services
  - [with Visual Code and TypeScript](./doc/02%20-%20Visual%20Studio/VS%20Code%20and%20TypeScript) 
  - [with Visual Studio 2017 and Csharp](./doc/02%20-%20Visual%20Studio/VS2017%20and%20C%23) 
- [Lab 03:](./doc/03%20-%20Continuous%20Delivery) In this lab, you will create a Continuous Deployment of an Azure Function and use a custom telemetry for Application Insights. (under construction)

## Deliver as a Demo ##

Please [follow the Setup Instructions](SETUP.md) to get your environment configured.

Here is the [demo script](DEMOSCRIPT.md).

## Contributing ##

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.