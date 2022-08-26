using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCloudProject.Common
{
    /// <summary>
    /// Defines the file related operations in the project.
    /// </summary>
    public interface IFileStorageProvider
    {
        /// <summary>
        /// Downloads the input file.
        /// </summary>
        /// <param name="fileName">The name of the local file where the input is downloaded.</param>
        /// <returns>The full pathname of the local input file, that was downloaded from the storage. </returns>
        Task<string> DownloadInputFile(string fileName);

        /// <summary>
        /// Downloads the input file into a folder.
        /// </summary>
        /// <param name="fileName">The name of the local file where the input is downloaded.</param>
        /// <returns>The full pathname of the local input file, that was downloaded from the storage. </returns>
        Task<string> DownloadInputFileInFolder(string fileName);


        /// <summary>
        /// Downloads the input file into a folder.
        /// </summary>
        /// <param name="fileName">The name of the local file where the input is downloaded.</param>
        /// <returns>The full pathname of the local input file, that was downloaded from the storage. </returns>
        Task<string> DownloadInputTestFile(string fileName);

        /// <summary>
        /// Uploads the result file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<byte[]> UploadResultFile(string fileName, byte[] data);

        /// <summary>
        /// Uploads results related to experiment.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        Task UploadExperimentResult(ExperimentResult result);
    }
}
