<a name="HOLTitle"></a>
# Provision Customer Reviews assets and Reset datas#

---

<a name="Overview"></a>
## Overview ##

Azure Resource Manager helps automate the provisionning of a lot of resources in Azure.

In this lab, you will create the needed assets for to run CustomerReviews web site and his automatic moderation with Cognitives Services. You will also add images to populate CustomerReviews site.

<a name="Objectives"></a>
### Objectives ###

In this hands-on lab, you will learn how to:

- Create Azure Services reuired for [lab 02](../02 - Visual Studio) and [lab 03](../03 - Continuous Delivery)
   + Storage account, DocumentDb, Cognitives Services,  Application Service, Azure Functions 
- Deploy Web site of CutomerReviews
- Set images data in storage and Document

<a name="Prerequisites"></a>
### Prerequisites ###

The following are required to complete this hands-on lab:

- An active Microsoft Azure subscription. If you don't have one, [sign up for a free trial](http://aka.ms/WATK-FreeTrial).
- An active GitHub account. If you don't have one, [sign up for free ](https://github.com/join). (optional)

---

<a name="Exercises"></a>
## Exercises ##

This hands-on lab includes the following exercises:

- [Exercise 1: Create an Azure Cloud Shell (PowerShell)](#Exercise1)
- [Exercise 2: Fork and Clone GitHub repo](#Exercise2)
- [Exercise 3: Create services with Azure Resource Manager template](#Exercise3)
- [Exercise 4: Add images to CustomerReviews site (Reset datas)](#Exercise4)

Estimated time to complete this lab: **30** minutes.

<a name="Exercise1"></a>
## Exercise 1: Create an Azure Cloud Shell (PowerShell) ##

The first step in .... In this exercise, you will create .....

1. Open the [Azure Portal](https://portal.azure.com) in your browser. If asked to log in, do so using your Microsoft account.

2. Click **Cloud Shell** button on top of page

    ![Open Cloud Shell](Images/open-cloud-shell.png)

    > A banner appear on the back of web page 

3. Click **PowerShell (Windows)**

    ![Select PowerShell](Images/select-powershell-windows.png)

4. Select your subscription, and Click on **Create storage**. This will create a new storage account for you and create a file share to persist files.

    ![Create default storage](Images/create-default-storage.png)

5. Your Azure Cloud Shell be ready in a few minutes . Verify taht you choose Powershell. You can swith between Bash and Powershell in the Dropdown

    ![Connecting Cloud Shell](Images/connecting-cloud-powershell.png)

The Azure Cloud Shell is a windows container. A new one will be started with your CloudDrive mapped as a volume. And you will be connected to your Azure current subscription. has been created and you have added three containers to the storage account created for it. The next step is to get code from this repo.



<a name="Exercise2"></a>
## Exercise 2: Fork and Clone GitHub repo ##

Once you have created an Azure Cloud Shell, you can you can use Powershell, commands, and some other SDK or application installed on it. In this exercise, you will clone the git repo into a persistent volume _CloudDrive_ . This will allow you to get all the scripts necessary to create the assets. 

1. Type these commands to navigate to your CloudDrive and clone this git repository.

    > Copy and paste in Azure Cloud Shell
    >
    > - Windows: `Ctrl-insert` to copy and `Shift-insert` to paste. Right-click dropdown can also enable copy/paste.
    >   - FireFox/IE may not support clipboard permissions properly.
    > - Mac OS: `Cmd-c` to copy and `Cmd-v` to paste. Right-click dropdown can also enable copy/paste.

    ```powershell
    cd C:\users\ContainerAdministrator\CloudDrive
    git clone --depth 1 --single-branch https://github.com/azugfr/functions-customer-reviews.git
    ```

    > _-depth 1 --single-branch_ options will limit download to the tip of master branch. The full history will not be downloaded. 

    ![cloning the git repo](Images/git-clone-functions-repo.png)

    ​

2.  Type this command to navigate inside the repo and list content.  

    ```powershell
    cd .\functions-customer-reviews\
    ls
    ```

    ![List content of repo](Images/list-functions-repo-content.png)



The folders doc and Media contains images and instructions for differents labs and demo. Source contains source code of the sample. Provision folder contains the scripts to create assets required for [lab 02](../02 - Visual Studio) and [lab 03](../03 - Continuous Delivery). Reset folder contains the scripts to clean up and pre filled images . The next step is to create the assets that the Customer Reviews solution relies on.

<a name="Exercise3"></a>
## Exercise 3: Create assets with Azure Resource Manager template ##

The`Provision` folder contains a Azure Resource Manager template file [Provision\assets\template.json](). This file describe all the to be created Azure assets:

* Storage account
* DocumentDB  
* Cognitives Services Computer Vision (Image analysis)
* Cognitives Services Content Moderator (Text analysis)
* Azure Web Site
* Application Insights
* Azure Functions 

In this exercise, you will create these assets.

1. In the Azure Cloud Shell, type this command to open the `parameters.jso` file in `vim`. 

    ```powershell
    vim ./Provision/assets/parameters.json
    ```

    > `vim` essentials : [quick reference card](http://users.ece.utexas.edu/~adnan/vimqrc.html)
    >
    > * `x` to delete characters
    > * `i` to insert characters
    > * `Esc` to get out of insertin mode
    > * `:qn` to save and exit
    > * `!:q` to exit without saving

    ![parameters.json before](Images/parameters-json-before.png)

2. Select first letter of **UNIQUE SMALL VALUE**, type `x` to delete letters, repeat to keep only quotation marks.

    ![parameters.json empty value](Images/parameters-json-empty.png)

3. Type `i` and enter a small unique value . This will be used for all asset names (cognitive services, storage accounts, web app and service plan, documentDB) so make sure it's unique, only use lower case characters, type between 4 - 8 alpahnumerical characters.

    > **yourunique2135** will likely not work as another one will probably already used this one. I did.

    ![parameters.json empty value](Images/parameters-json-after.png)

4. Type `Esc` to quit insert mode. Type `:wq` to save and quit `vim`.

5. Type this command, and copy your **SubscriptionId**. Select it and type `Ctrl-insert`

    ```powershell
    ls Azure:
    ```

    > Under Azure: folder you can browse all asset of your current connected subscription like in a filesystem.

    ![copy SubcriptionId](Images/subscriptionId-list-copy.png)

6. Type these commands to launch the script that will create the assets.

    ```powershell
    cd .\functions-customer-reviews\Provision\assets\
    .\Deploy.ps1
    ```

    >  Use this values for each prompted variables:
    >
    > * subscriptionId: Paste your copied **SubscriptionId** value at preceding step. Type `Shift-Insert` 
    > * resourceGroupName: **customerreview**
    > * deploymentName: **customerreview**

    ![Deploy all assets](Images/deploy-assets-parameters.png)

7. After a few seconds, Tpe region: **West Europe**

    >  regions available for all assets:  East US, South Central US, North Europe, West Europe, Southeast Asia, West US 2

    ![Deploy all assets](Images/deploy-assets-region.png)

8. Wait a few minutes. Return to Azure portal, click to
    **Resource groups** > **Overview** > **customerreview** to follow assets provisioning.

    ![asset provisioning in portal](Images/deploy-assets-portal.png)

    At the end, log should look like this one:

    ![Deploy all assets](Images/deploy-assets-log.png)

9. Open a new browser tab. Type this url http://**your_unique_name**site.azurewebsites.net

    > Site can take a few more minutes to be deploy by Kudu from this repo. This [document page](https://docs.microsoft.com/en-us/azure/app-service/app-service-deploy-complex-application-predictably) explain how to it.


![web site deploying ](Images/deploy-assets-websitedeploying.png)

_Web site deploying_

> Web site is ready. But still empty and need some images. 

![web site deploying ](Images/deploy-assets-websiteready.png)

The assets are now all deployed and running. Now it's time to add some samples images to the web site.

<a name="Exercise4"></a>
## Exercise 4: Add images to CustomerReviews site (Reset datas) ##

Your Web site is configured to get images from DocumentDB collections that you created in [Exercise 3](#Exercise3). These collections are not set thus the Error on the web site. The`Reset` folder contains scripts to set access to your DocumentDB and storage account. In this exercise, you will set collections, upload images to the DocumentDB, and reset any previous datas.

1. In the Azure Cloud Shell, Type these commands.

    ```powershell
    cd C:\Users\ContainerAdministrator\CloudDrive\functions-customer-reviews\Reset
    .\SetConfigXmlKeys.ps1 -resourceGroup customerreview -uniqueKey your_unique_name
    vim config.xml
    ```

    > !Warning! replace **your_unique_name** by the value you use in [Exercise 3](#Exercise3)
    >
    > Verify all uppercase values are replaced.

    ![Set config.xml parameters](Images/reset-config-set.png)

2. Type `!:q` to exit `vim` without saving.

3. Type this command

    > There may be some errors during cleaning as there are no collection in
    > DocumentDB set yet.

    ![clean up and add images](Images/reset-script-log.png)

4. Open a new browser tab. Type this url http://**your_unique_name**site.azurewebsites.net

    > You may need to force a refresh of the content, type `Ctrl-F5` .  

    ![Web site ready with images](Images/reset-website-ready.png)


<a name="Summary"></a>
## Summary ##

In this hands-on lab you learned how to:

- Create assets with Azure Resource Manager templates
- Execute PowerShell and Command srcipts in Azure Cloud Shell
- Use Azure Web Apps and deployment engine (Kudu) to automate build and deploy of a Web site from a GitHub repo.

This is just one example of how you can leverage Azure Resource Managemeer and Azure Cloud Shell to automate infrastructure. Experiment with other Azure Resource Manager templates to learn more about Azure Services and provisionning to identify additional ways in which they can aid your research or business.
