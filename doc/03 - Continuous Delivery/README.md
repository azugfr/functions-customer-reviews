<a name="HOLTitle"></a>
# Create a continuous delivery pipeline  

---

<a name="Overview"></a>
## Overview ##

Visual Studio Team Services (VSTS) helps helps automate ans secure the delivery pipeline of your applications. In lab 00, we used the intern deployment engine tool of Azure Kudu. It's a great tool to start with but comes with limitations. 

In this lab, you will create a build pipeline to automatically build, test, package and deploy the Azure Function that check Image and moderate text for the CustomerReviews site. And use Application Insights to follow the health of solution and results of the reviews.

<a name="Objectives"></a>
### Objectives ###

In this hands-on lab, you will learn how to:

- Create a Continuous Deployment of an Azure Function
- Use a custom telemetry for Application Insights 

<a name="Prerequisites"></a>
### Prerequisites ###

The following are required to complete this hands-on lab:

- An active Microsoft Azure subscription. If you don't have one, [sign up for a free trial](http://aka.ms/WATK-FreeTrial).
- An active GitHub account. If you don't have one, [sign up for free ](https://github.com/join). 
- Execute succesfully the [lab 00](../00%20-%20Provision%20resources%20and%20Reset) .

---

<a name="Exercises"></a>
## Exercises ##

This hands-on lab includes the following exercises:

- [Exercise 1: Create an VSTS account and Project](#Exercise1)
- [Exercise 2: Fork GitHub repository and create a PAT](#Exercise2)
- [Exercise 3: Create a pipeline to Continuous Delivery of an Azure Function](#Exercise3)
- [Exercise 4: Create custom query and use dashboard of Application Insights](#Exercise4)

Estimated time to complete this lab: **60** minutes.

<a name="Exercise1"></a>
## Exercise 1: Create an VSTS account and Project ##

The first step in creting a continuous pipeline is to create an VSTS account and a Team Project. In this exercise, you will create the a new account and through Azure Portal.

1. Open the [Azure Portal](http://portal.azure.com) in your browser. If you are asked to log in, do so using your Microsoft account.

2. Click **New** , enter **Team**, and click **Team project**

    ![create Team Project](Images/vsts-account-new.png)

3. Click on **Create** 

    ![create team project](Images/vsts-account-create.png)

4. Enter `MyFirstProject` in **Name**, click **Account**, click **Create a new account**, enter a new name in  **URL**, click **OK**, and click **Create**

    > If  you want to change the de deployment region or the name of team project created, click on **Location** .


![create team project](Images/vsts-account-project-create.png)

1. Click on **go to resource groupe**

    ![go to resource group](Images/vsts-account-go-to-resourcegroup.png)

2. Click on your VSTS account

    ![click VSTS account](Images/vsts-account-team-open.png)

3. Click on **Url**

    ![build and deployment](Images/vsts-account-build-deployment.png)

4. Click on  **MyFirstProject**

    ![open projects](Images/vsts-account-project-open.png)



4. Check you project MyFirstProject is created. 

    ![first project created](Images/vsts-project-created.png)

Now you have a team project ready to help you Code, Build, Release, Test your apps. Next, you need your own Github repository.

<a name="Exercise2"></a>
## Exercise 2: Fork GitHub repository and create a PAT ##

Once you have created an VSTS account and team project, you can build a project from any git repository. To enable continuous trigger for each commit on GitHUb repository, you need to be an owner on the repository. In this exercise, you will fork this repository to your account on which you will be an owner. 

1. Open the repository root on new tab https://github.com/azugfr/functions-customer-reviews, and then click on **Fork** or **Sign in**

    ![Fork the git repo](Images/git-fork-functions-repo.png)

2. Click on Enter your **username** and **password**, and then click **Sign in**  

    >  If you don't have an GitHub account, click on **Create an account** to create one

    ![GitHub sign in](Images/git-sign-in-or-up.png)

3. If you signed in, you need to click a second time on **Fork**

    ![Fork in progress](Images/git-fork-in-progress.png)

4. Click on **Clone an download**. Take note of your Git Url, you will need it in [Exercise 3](*Exercise3)


![Git repository url](Images/git-fork-clone-url.png)

The forked GitHub repo allows you to commit your changes. The next step is to add an build pipeline to integrate these changes.

<a name="Exercise3"></a>
## Exercise 3: Create a pipeline to set Continuous Delivery of an Azure Function ##

The VSTS team project you created in [Exercise 1](#Exercise1) and the GitHub repo you forked in [Exercise 2](#Exercise2) will helps you build, test and deploy the Azure Function in [ContentModerator solution](../Source/csharp/ContentModerator.sln) to the Azure Function App you provisionned in [lab 00](../00 - Provision resources and Reset). In this exercise, you will create a minimal pipeline with a build and a release definitions to continuously deploy your change on the Azure Function.

1. Navigate to your VSTS Team project, and click on **Build & Release** > **Builds**

   ![Navigate to Build & Release](Images/vsts-build-and-release.png)

2. Click **+New** button 

   ![create new build](Images/vsts-build-new.png)

3. Select **ASP.NET**, click on **Apply**

   ![select ASP.NET](Images/vsts-build-select-aspnet.png)

4. Click on **Get sources**, Select **GitHub**, enter connection of your fork GitHub repository created in [exercise 2](*Exercise2), and then click on **Authorize using OAuth** 

   > if nothing happen, your browser may blocking popup windows, authorize them to continue. And click on **Authorize using OAuth**

   ![Github repo](Images/vsts-build-github-repo-url.png)

5. Click on **Authorize vsonline**

   ![Authorize vsts to access repos](Images/vsts-build-github-OAuth-authorize.png)

6. Return to the Build Definiition, click on **Triggers**, and then select **Enable continuous integration** 

   ![enable continuous integration](Images/vsts-build-github-continuous-integration.png)

7. Click on **Tasks**, and then click **Process** , and enter `**\ContentModerator.sln` in **Path to solution or packages.config**

   > if **Agent queue** is not set, choose `Hosted VS2017`

   ![edit solutions path](Images/vsts-build-edit-path-solution.png)

8. Click Save & queue > **Save**, and then **Save**

   ![Save build definition](Images/vsts-build-save.png)![build folder](Images/vsts-build-save-folder.png)

9. Click on **Releases**, and then **+ New definition **

   ![new release](Images/vsts-release-new.png)

10. Select **Azure App Service Deployment**, and click **Apply**

   ![select app service deployment](Images/vsts-release-select-app-service-deployment.png) 

11. Enter `Dev` in **Environment**

    ![environment name](Images/vsts-release-environment.png)

12. Click on **Add artifact**, select the build you just created in **Source**, and then click **Add**

    ![add build artifact](Images/vsts-release-add-build.png)

13. Click on **Continuous deployment trigger**, swith On **Enabled**, and click on **1 phase, 1 task** link 

    ![enable continuous deployment](Images/vsts-release-enable-continuous-deployment.png)

14. Click **Tasks**, click **+ New** on the right of Azure Subscription 

    ![New Azure subscription](Images/vsts-release-new-azure-subscription.png)

15. Click on **use the automated version of the endpoint dialog**

    ![use automated view](Images/vsts-release-subscription-use-automated.png)

16. Enter `AzureFunction` in **Name**, and click **OK**

    > if dropdown Subscrition is empty if you do not see the subcription used for [lab 00](../00%20-%20Provision%20resources%20and%20Reset ), your vsts account is not linked to the Azure subscription. You can [link your VSTS account to Azure subscription](https://docs.microsoft.com/en-us/vsts/billing/set-up-billing-for-your-account-vs) 
    > Or use preceding dialog and follow instructions of creating a [service principal](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-create-service-principal-portal) and [Service endpoints](https://go.microsoft.com/fwlink/?LinkID=623000&clcid=0x409). Add `Contributor` role to your Service principal on Azure Function App

    ![create Azure end point](Images/vsts-release-subscription-create-endpoint.png)

17.  Click **Authorize**

    > A popup may appear. Login to proceed.

![authorize subscription](Images/vsts-release-subscription-authorize.png)

18. Select **Function App** in **App type** dropdown, select your `<unique_name>function` in **App service name** dropdown

    > you may have to refresh **App service name** after selecting **App type**

    ![select function](Images/vsts-release-select-function.png)

19. Click **Save**, and then **OK**

    ![Save release](Images/vsts-release-save.png)![Save release](Images/vsts-release-save-folder.png)

20. Click **Builds**, and click on **...**, click  **Queue new build...**, and click **Queue**

    ![queue build](Images/vsts-build-queue-build.png)![queue build](Images/vsts-build-queue-build-confirm.png)

21. Open Build build 

    ![open build](Images/vsts-build-open-current.png)

    > When the Build has succeeded

    ![Build succeeded](Images/vsts-build-succeeded.png)

22. Click on **Releases**, and click **...** , and **Open**

    ![open release](Images/vsts-release-open.png)

    > the release has been automatically triggered on `Dev` environment and hopefully succeeded

    ​

    ![Release succeeded on Dev](Images/vsts-release-succeeded-on-dev.png)

    You have now a minimal pipeline to update your Azure Function. You can test your function work by adding some image to CustomerReviews site.


### Expand build to run tests 

You can expand the build to also run tests

1. Click **Tasks**, and then **Test Assemblies**, and enter `**\$(BuildConfiguration)\**\*test*.dll` in **Test Assemblies**

2. [Make sure you have Colin's ALM Corner Build & Release Tools installed on your VSTS account](https://marketplace.visualstudio.com/items?itemName=colinsalmcorner.colinsalmcorner-buildtasks)

3. Create the following variables for your new build definitions:

   - AzureWebJobsStorage  from your resource group > <unique_name>
   - MicrosoftVisionApiKey
   - ContentModerationApiKey
   - customerReviewDataDocDB
   - APPINSIGHTS_INSTRUMENTATIONKEY

4. Add the `Replace Tokens` task between the `Build Solution` and `Test Assemblies` tasks. This task will replace the values in `local.settings.json` with the values in the variables. Configure the settings in this task with the following values:

   - sourcePath: ContentModeratorFunction/bin/$(BuildConfiguration)

   - filePattern: local.settings.json

   - tokenRegex: __(\\w+)__

   - Queue a new build run - the build summary should show the two tests passed:

     ![](../../Media/Setup6.png)

###  (Advanced) Expand pipeline to automate resource deployment

You can expand the release to also create or update all the assets before deploying Azure Function App.

1. In Build definition, add a new `Publish Build Artifacts` task. Set it to copy the `template.json` and `parameters.json` available in `../Provision/assets`
2. In Release definition, add a new `Azure Resource Group Deployment`task. Set it to update your assets as detailed in [automate resource deployment of Azure Function](https://docs.microsoft.com/en-us/azure/azure-functions/functions-infrastructure-as-code)


The work of writing, configuring and deploy the Azure Function is complete.


<a name="Summary"></a>
## Summary ##

In this hands-on lab you learned how to:

- Create a VSTS account and a new Team project
- Create a fork of a Github repository and a new PAT to access this repository
- Create a new pipeline with a Build and a Release definitions to continuously deliver an Azure Function

This is just one example of how you can leverage VSTS and Application Insights. Experiment with other application deployment, you may want to add CustomerReviews site alonside the Azure Function. You could also leverage Azure Web apps capabilities like Slots to launch integrations tests before, or add an intermediate environment before production one like in this [blog post](https://blogs.msdn.microsoft.com/visualstudioalmrangers/2017/10/04/azure-function-ci-cd-devops-pipeline/).
You could also explore [Azure DevOps Projects](https://go.microsoft.com/fwlink/?linkid=862126) to rapidly kickstart your project.

