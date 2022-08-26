# My Exercises
RENAME THIS FILE TO "Exercises - Firsname Lastname.md" and put the content of this file into it.
FILES WITH OTHER NAMES WILL BE REJECTED!

#### Exercise I 

Provide script samples used in this exercise. What did you do , why, how, what was the result.
If you have your scrips somwhere on the git, provide us als the URL here to them.

Answer:
az --version 
This command is used to check if azure desktop is installed in the pc and for current version information

az login
This command is used to login to the azure account
https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/az_1.png

az group create --location australiaeast --resource-group Demo_tobeDeleted1
This command is used to create resource group in a location server which can also be done by web dashboard


az group list
This command is used to show the list of current groups in the account
https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/az_2_groupList.png

az group list --tag key1=America
This command is used to show the groups containing key1 value America
https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/az_3_grouListWithKey.png

az group exists -n Demo_tobeDeleted
This command is used to check the existance of the group
https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/az_4_groupExistanceAndExporting.png

az group export --n Demo_tobeDeleted1
This will print the group information as a jason format

az group list --query "[?location=='australiaeast']"
This will print the groups information matching location Australia East
https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/az_5_groupQuey.png

az group delete --name Demo_tobeDeleted1
This will delete the group from resource groups

#### Exercise 2 - Docker in Azure

1. Provide URL to the docker file. I.e.: %giturl%\Source\MyCloudProjectSample\MyCloudProject\Dockerfile

Answer:
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DockerConsoleApp/DockerConsoleApp.csproj", "DockerConsoleApp/"]
RUN dotnet restore "DockerConsoleApp/DockerConsoleApp.csproj"
COPY . .
WORKDIR "/src/DockerConsoleApp"
RUN dotnet build "DockerConsoleApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DockerConsoleApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockerConsoleApp.dll"]

2. Provide the URL to the public image in the Docker Hub.

Answer:

https://hub.docker.com/repository/docker/mashnunul/cloud_tut

3. Provide the URL to the private(public??) image in the Azure Registry.

Answer:
Creating Azure container in a resource group
https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/azure_container_resourceGroup.png

Pushing the docker image in the azure container
https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/docker%20pushing.png

Proof of successful docker image pushing
https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/docker_image_in_azure_repository.png


#### Exercise 3 - Host a web application with Azure App service

1. Provide the public URL of the webapplication.

Answer: 
https://bestcalculatorapp.azurewebsites.net/

2. Provide the URL to the source code of the hosted application. (Source code somwhere or the the docker image, or ??)

Answer:
{
  "schemaVersion": 2,
  "mediaType": "application/vnd.docker.distribution.manifest.v2+json",
  "config": {
    "mediaType": "application/vnd.docker.container.image.v1+json",
    "size": 4294,
    "digest": "sha256:5d36a17aa6e3e6276c8e7bcfb4300f52deeb77c0c3d17906be3a4101b9f316e6"
  },
  "layers": [
    {
      "mediaType": "application/vnd.docker.image.rootfs.diff.tar.gzip",
      "size": 31379476,
      "digest": "sha256:214ca5fb90323fe769c63a12af092f2572bf1c6b300263e09883909fc865d260"
    },
    {
      "mediaType": "application/vnd.docker.image.rootfs.diff.tar.gzip",
      "size": 14966772,
      "digest": "sha256:562f2b48c06c589de5b8b2f72eb9a0040bc6eab0d479295085ad96036c1da020"
    },
    {
      "mediaType": "application/vnd.docker.image.rootfs.diff.tar.gzip",
      "size": 31620678,
      "digest": "sha256:bdd7874d464602a566b7f5087754c80ca1f5d01d279b251aceec9836d66e717e"
    },
    {
      "mediaType": "application/vnd.docker.image.rootfs.diff.tar.gzip",
      "size": 154,
      "digest": "sha256:8aa9b64f5310895753358c2676ade6be98179e6b05dadd87adf51f8571250802"
    },
    {
      "mediaType": "application/vnd.docker.image.rootfs.diff.tar.gzip",
      "size": 9453033,
      "digest": "sha256:e4e0aa40bc94f2509482211198a5deacb2f5882123462b128867a95c5774db64"
    },
    {
      "mediaType": "application/vnd.docker.image.rootfs.diff.tar.gzip",
      "size": 92,
      "digest": "sha256:6eddc99c708d4d28c4e82f752fddcfc1ba507b64cb13f26c56fd15f1e9811707"
    },
    {
      "mediaType": "application/vnd.docker.image.rootfs.diff.tar.gzip",
      "size": 1666773,
      "digest": "sha256:9c620c62346054aa5b24d1f3ed7d540ec53a7109ff6aa1e799985f0ccb91e154"
    }
  ]
}

3. Provide AZ scripts used to bublish the application.

Answer:

 az login

 az webapp devployment source config-zip --src .\site.zip --resource-group CalculatorApp --name calculatorapp

 docker login

 docker tag bestcalculatorapp:dev bestcalculatorappacr.azurecr.io\bestcalculatorapp:v1

 docker push bestcalculatorappacr.azurecr.io/bestcalculatorapp:v1

 az acr build --file Dockerfile --registry bestcalculatorappacr --image bestcalculatorapp:v2 .
 (This one works only)

#### Exercise 4 - Deploy and run the containerized app in AppService

1. Provide URL to the docker image of your application (Docker Hub / Azure Registry)

Answer: https://portal.azure.com/#blade/Microsoft_Azure_ContainerRegistries/RepositoryBlade/id/%2Fsubscriptions%2Fa16e4873-014a

2. Provide the public URL to the running application. 

Answer: https://calculatorappacr.azurewebsites.net


#### Exercise 5 - Blob Storage

Provide the URL to to blob storage container under your account.
We should find some containers and blobs in there.
Following are mandatory:
- Input
Contains training files

https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/BlobStorage_FileUploaded.png

- Output
Contains output of the traned models

https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/BlobStorage_FileDownloaded.png

- 'Test' for playing and testing.
you should provide here SAS Url to 2-3 files in this container with time expire 1 year.

https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/BlobStorage_FileDownloaded.png
https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/BlobStorage_DownloadFileDeleted.png

#### Exercise 6 - Table Storage

Provide us access to the account which you have used for table exersises.

Answer:
https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/StorageTable_demoTableCreatedWithKey_task2.png
https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/StorageTable_demoTableCreated_task1.png
https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/StorageTable_demoTableQueries_task2.png
https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/StorageTable_demoTableUpdated_task2.png

#### Exercise 7 - Queue Storage

Provide us access to the account which you have used for queue exersises.

Answer:
![QueStorage_qued message](https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/QueStorage_qued%20message.png)
https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/QueStorage_qued%20message%20deleted.png
https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/QueStorage_que%20itself%20deleted.png
https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/QueStorage_qued%20many%20messages%20to%20send.png
https://github.com/UniversityOfAppliedSciencesFrankfurt/se-cloud-2021-2022/blob/mashnunulHuq/Source/MyCloudProjectSample/Documentation/Exercise%20Pictures/QueStorage_receiving%20qued%20many%20messages.png
