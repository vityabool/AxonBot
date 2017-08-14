# Axon Partners Bot Platform
## Summary
Axon Partners is looking for the ways to address legal need of developers. The company wants to create a platform for various legal services and community around it. Privacy Policy document generation tool has been identified as a great start of the initiative. Idea of the project has been generated to accelerate emerging Legal Tech trend. 

Axon Partners Privacy Policy Bot is a simple bot aimed to help application developers generate relevant Privacy Policy agreements. Usually this task is quite complicated for individual developers and small organizations creating mobile apps. A developer can generate and download legally correct document by answering questions in 5 minutes. Bot conversations has been identified as the best way of communication with the app. 

To address broader goal of the goal the project has been developed in the way conversations could be extended or changed. It allows improve current process or implement completely new conversations. Axon Partners are looking at this bot as platform to develop more useful legal services for developers and others. 

We encourage other legal companies and developers to leverage this code and make more great solutions. 

## Key technologies
The Bot has been build using [Microsoft Bot Framework](https://dev.botframework.com/). Microsoft Bot Framework consist of 3 main parts: 
- Bot Builder SDK ([available for download from GitHub](https://github.com/Microsoft/BotBuilder)). This is set of classes for .NET and Node.JS allow you to build the Bot logic;
- [Bot Framework portal](https://dev.botframework.com/bots) which is the interface to Bot Framework service allows you to register and publish a bot; 
- Finally, there are many channels available to access your bot like Skype, Slack, Facebook etc. 

## Customer profile

[Axon Partners](http://axon.partners/) is leading legal company based in Ukraine. It works with software development, Telco, Gaming, E-commerce, Media and FinTech industries. In software development industry Axon Partners helps local companies to enter foreign markets by developing their corporate structure in Ukraine and abroad, drafting industry-specific contracts and supporting cross-border software development services.
The company also represent foreign clients who subcontract Ukrainian developers or have R&D offices in Ukraine.



## Solution Overview
Axon Partners Bot Platform is built on Microsoft Bot Framework. The bot logic runs on Azure App service. It is built on ASP.NET MVC. App data is stored in Azure SQL, Blob Storage and Storage Tables. Bot is connected to Skype, Facebook and web via Bot Connector. 

![Solution architecture diagram](https://raw.githubusercontent.com/vityabool/AxonBot/master/Img/AxonBotArchitecture.jpg) 


## Technical delivery
#### Tools required:
- [Visual Studio Community](https://www.visualstudio.com/downloads)
- [Bot Builder SDK](https://www.nuget.org/packages/Microsoft.Bot.Builder)
- [Azure Subscription](https://azure.microsoft.com/en-us/free) 

#### Azure services and configuration

- **Azure App Service**. The bot logic is ASP.NET MVC App.
- **Azure SQL DB**. Mainly to store bot configuration. Majority of tables in the DB are cached in memory when application starts. The only reason to use SQL here is that there are many tools available to edit SQL tables. So, it will be easy to maintain and extend conversations. 
- **Azure Blob Storage** is used for storing generated Privacy Policy in .docx format. Generated links are provided by the bot to end user after the document is generated. 
- **Azure Table Storage** is used to log all conversations raw data. It could be later used to analyze conversations flow.

Requests to SQL and other storages are isolated in the code with Data Access Layer (DAL folder in the solution), so it could be changed any time without the requirement to touch bot logic code. 

Connection strings to all storage databases are in file: 
``` 
AxonPartners.Bot/Web.config
```

You should change the default values for real connection strings before deploying:
```XML
  <appSettings>
    <!-- update these with your BotId, Microsoft App Id and your Microsoft App Password-->
    <add key="BotId" value="{YOUR BOT ID}" />
    <add key="MicrosoftAppId" value="{YOUR APP ID}" />
    <add key="MicrosoftAppPassword" value="{YOUR APP PASSKEY}" />
  </appSettings>
  <connectionStrings>
    <add name="StorageConnectionString" connectionString="{YOUR STORAGE CONNECTION STRING}" />
    <add name="DatabaseConnectionString" connectionString="{YOUR DATABASE CONNECTION STRING}" />
  </connectionStrings>
```

#### SQL Database Tables structure
SQL Table **DialogPipleine** describes dialogs structure. The default dialog is Privacy Policy bot. By adding rows in this table almost any type of dialog could be created.
![Dialog Pipeline Table](https://raw.githubusercontent.com/vityabool/AxonBot/master/Img/DialogPipleine.jpg)
- **Pid** - # of the dialog. 
- **Lang** - Language of the dialog. Multilanguage dialogs could be described here.
- **Id* - Identifier of the step in the dialog
- **NextId** - Next question bot should display. For example, by adding new row *Id 30* and changing *NextId* in the row *Id 2* to *30* bot jumps to the additional question.
- **MessageType** - Could be YesNo, Text and Final.
- **YesNoOption** - Field describes behavior of YesNo.
- **Exit** - Close the dialog. Exit message in in the field **epExitMessage**
    - **Logic** - Describes logical forks. In this case Fields **IpIfYesGoToId** and **IpNoGoToId** holds next question *id*. Fields **IfYesTextId** and **IfNoTextId** have ids of text blocks in the *Texts* table. 
    - **Question** - Displays logical question. 
- **ParamName** - Holds verbose name of the question.

Table **Texts** stores all texts referenced from DialogPipeline table and some standard texts.
![SQL Texts Table](https://raw.githubusercontent.com/vityabool/AxonBot/master/Img/Texts.jpg)

Table **Settings** describes logical commands like Help or Restart of the dialog.

![SQL Settings Table](https://raw.githubusercontent.com/vityabool/AxonBot/master/Img/Settings.jpg)

#### Solution Folders structure
- **DAL** - Abstraction classes to access storages and DB.
- **Docs** - Privacy Policy agreement template (not used during compilation).
- **Services** - Common classes to read and cache settings from the DB and crate Word document.
- **AxonPartners.Bot** - Bot logic.
- **AxonPartners.Models** - Classes to store data models.
- **TestDocGenApp** - Testing utilities.

![Solution Tree](https://raw.githubusercontent.com/vityabool/AxonBot/master/Img/SolutionExplorer.jpg)

## Conclusion

The Privacy Policy Generation Bot has been implemented. Flexible dialog structure allows to use this code as the base for creating new apps and extend community of useful LegalTech services. 

![Bot Conversation Example](https://raw.githubusercontent.com/vityabool/AxonBot/master/Img/Bot.jpg)

## Team

The project has been created by [Viktor Tsykunov](https://github.com/vityabool) (Bot logic) and [Sergiy Poplavskyi](https://github.com/spoplavskiy) (DB abstraction and Singleton configuration).





