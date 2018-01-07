<a name="HOLTitle"></a>
# Azure Functions #

---

<a name="Overview"></a>
## Overview ##

In this lab, you will create an Azure Function to perform automated moderation of the customer reviews using Microsoft Cognitive Services.
It monitors a storage queue where the website puts alerts for my function to know that there's a new review; and binds to blob storage where the review picture is, and to a CosmosDB document where the review text and other metadata are. 
It performs then an automated analysis of the image using the Microsoft Cognitive Services [Computer Vision API](https://azure.microsoft.com/en-us/services/cognitive-services/computer-vision/) and the text using [Content Moderator API](https://azure.microsoft.com/en-us/services/cognitive-services/content-moderator/).

<a name="Objectives"></a>
### Objectives ###

In this hands-on lab, you will learn how to:

- Create an Azure Function App from Azure Functions CLI and Visual Studio Code
- Write an Azure Function that uses a queue trigger, a blob storage input and a Cosmos DB document input.
- Add application settings to an Azure Function App
- Use Microsoft Cognitive Services to analyze a text and an image and store the results in the Cosmos DB document

<a name="Prerequisites"></a>
### Prerequisites ###

The following are required to complete this hands-on lab:

- An active Microsoft Azure subscription. If you don't have one, [sign up for a free trial](http://aka.ms/WATK-FreeTrial).
- [Visual Studio Code](https://code.visualstudio.com/)
- [Node.js 8.5+](https://nodejs.org/en/)

For local debugging:
- [.NET Core 2.0 SDK](https://www.microsoft.com/net/download/core)
- Azure Core Function Tools 2.0

	```
	npm install --global azure-functions-core-tools@core
	```

---

<a name="Exercises"></a>
## Exercises ##

This hands-on lab includes the following exercises:

- [Exercise 1: Setup the environment](#Exercise1)
- [Exercise 2: Write the Azure Function](#Exercise2)

Estimated time to complete this lab: **60** minutes.

<a name="Exercise1"></a>
## Exercise 1: Setup the environment ##

In this exercise, you will create an Azure Function App using Azure Functions CLI and Visual Studio Code.

1. Create a new folder and initialize an Azure Function by doing the following from a command prompt:

	```
	mkdir redtshirtour
	cd redtshirtour
	func init
	```

**host.json** and **appsettings.json** are created.	
	
2. Let's create a simple queue triggered function:

	```
	func new
	```

Select **JavaScript** as language, **QueueTrigger** as template and enter the name **ContentModeratorFunction** for the function name.

![Creating a function from Powershell](../../../Media/create_function_from_ps.png)

A JavaScript template has now been created.

3. Time to setup our TypeScript environment:

	```
	cd ContentModeratorFunction
	npm init
	tsc -- init
	mv index.js index.ts
	```

![Setup of the TypeScript environment](../../../Media/setup_typescript_environment.png)	
	
4. We are now ready to work in Visual Studio Code. Let's open up VS Code:

	```
	cd ..
	code .
	```

The next step is to implement our function.

<a name="Exercise2"></a>
## Exercise 2: Write the Azure Function ##

In this exercise, you will add write TypeScript code that uses the [Computer Vision API](https://www.microsoft.com/cognitive-services/en-us/computer-vision-api) to analyze images added to the "input-images" container and the [Content Moderator API](https://azure.microsoft.com/en-us/services/cognitive-services/content-moderator/) to analyse the review text of the Cosmos DB document.

