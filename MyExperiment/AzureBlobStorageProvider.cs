using Microsoft.Extensions.Configuration;
using MyCloudProject.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MyExperiment.Utilities;
using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos.Table;

namespace MyExperiment
{
    public class AzureBlobStorageProvider : IFileStorageProvider
    {
        private MyConfig config;
        private string dataFolder;

        public AzureBlobStorageProvider(IConfigurationSection configSection)
        {
            config = new MyConfig();
            configSection.Bind(config);
            dataFolder = FileUtilities.GetLocalStorageFilePath(config.LocalPath);
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }
        }

        /// <summary>
        /// Providing name to the file and download the file from azure blob storage (maily for configuration files)
        /// </summary>
        /// <param name="fileName">name of file</param>
        /// <returns>path url</returns>
        public async Task<string> DownloadInputFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Exception("File name can not be null or empty");
            //string localStorageFilePath = Path.Combine(dataFolder, new FileInfo(fileName).Name);
            BlobServiceClient blobServiceClient = new BlobServiceClient(config.StorageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(config.InputContainer);
            var blobClient = containerClient.GetBlobClient(fileName);
            string localStorageFilePath = Path.Combine(dataFolder, new FileInfo(fileName).Name);
            using (var fileStream = File.Create(localStorageFilePath))
            {
                await blobClient.DownloadToAsync(fileStream);
            }
            
            return localStorageFilePath;
        }

        /// <summary>
        /// Providing name to the file and download the file from azure blob storage into a seperate folder (mainly for data files)
        /// </summary>
        /// <param name="fileName">name of file</param>
        /// <returns>path url</returns>
        public async Task<string> DownloadInputFileInFolder(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Exception("File name can not be null or empty");
            BlobServiceClient blobServiceClient = new BlobServiceClient(config.StorageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(config.InputContainer);
            var blobClient = containerClient.GetBlobClient(fileName);
            string localStorageFileDirectory = Path.Combine(dataFolder, Path.GetFileNameWithoutExtension(fileName));
            if (!Directory.Exists(localStorageFileDirectory))
            {
                Directory.CreateDirectory(localStorageFileDirectory);
            }
            string localFilePath = Path.Combine(localStorageFileDirectory, new FileInfo(fileName).Name);
            using (var fileStream = File.Create(localFilePath))
            {
                await blobClient.DownloadToAsync(fileStream);
            }

            return localFilePath;
        }

        /// <summary>
        /// Providing name to the file and download the file from azure blob storage into a seperate folder (mainly for data files)
        /// </summary>
        /// <param name="fileName">name of file</param>
        /// <returns>path url</returns>
        public async Task<string> DownloadInputTestFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new Exception("File name can not be null or empty");
            BlobServiceClient blobServiceClient = new BlobServiceClient(config.StorageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(config.InputTestContainer);
            var blobClient = containerClient.GetBlobClient(fileName);
            string localStorageFilePath = Path.Combine(dataFolder, new FileInfo(fileName).Name);
            using (var fileStream = File.Create(localStorageFilePath))
            {
                await blobClient.DownloadToAsync(fileStream);
            }

            return localStorageFilePath;
        }

        /// <summary>
        /// uploading of row data in table
        /// </summary>
        /// <param name="result"></param>
        /// <returns>task</returns>
        public async Task UploadExperimentResult(ExperimentResult result)
        {
            await InsertOrMergeEntityAsync(result);
        }

        /// <summary>
        /// Upload file in output container
        /// </summary>
        /// <param name="fileName">name of file</param>
        /// <param name="data">byte of file</param>
        /// <returns>url of uploaded file</returns>
        public async Task<byte[]> UploadResultFile(string fileName, byte[] data)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(config.StorageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(config.OutputContainer);
            if (data != null)
            {
                await File.WriteAllBytesAsync(Path.Combine(dataFolder,Path.GetFileNameWithoutExtension(fileName)), data);
            }
            
            var blobClient = containerClient.GetBlobClient(fileName);
            using (var fileStream = File.OpenRead(Path.Combine(dataFolder, Path.GetFileNameWithoutExtension(fileName))))
            {
                await blobClient.UploadAsync(fileStream, true);
            }
            return Encoding.ASCII.GetBytes(blobClient.Uri.ToString());
        }

        #region table operation
        /// <summary>
        /// create table if not exists and inserts new data
        /// </summary>
        /// <param name="entity">entire row </param>
        /// <returns>task</returns>
        private async Task InsertOrMergeEntityAsync(ExperimentResult entity)
        {
            CloudStorageAccount storageAccount;
            storageAccount = CloudStorageAccount.Parse(config.StorageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tableClient.GetTableReference(config.ResultTable);
            if (await table.CreateIfNotExistsAsync())
            {
                Console.WriteLine("Created Table is called: {0}", config.ResultTable);
            }
            else
            {
                Console.WriteLine("Table name {0} already exists, returning the table", config.ResultTable);
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Insertion of row operation successful. Results uploaded in a table");
                }
            }
            catch (Exception storageException)
            {
                throw new Exception(storageException.Message);
            }
        }
        #endregion
    }
}
