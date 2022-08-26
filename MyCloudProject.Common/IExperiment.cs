using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyCloudProject.Common
{
    /// <summary>
    /// Defines your experiment.
    /// </summary>
    public interface IExperiment
    {
        /// <summary>
        /// Runs experiment.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        Task<ExperimentResult> Run(string[] inputFiles, string videoConfigFile, string htmConfigFile, string[] testfiles);

        /// <summary>
        /// Starts the listening for incomming messages, which will trigger the experiment.
        /// </summary>
        /// <param name="cancelToken">Token used to cancel the listening process.</param>
        /// <returns></returns>
        Task RunQueueListener(CancellationToken cancelToken);
    }
}
