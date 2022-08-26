using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCloudProject.Common
{
    /// <summary>
    /// The class that describes the final experiment result.
    /// Feel free to extend it, but do not remove any existing property.
    /// </summary>
    public class ExperimentResult : TableEntity
    {
        public ExperimentResult(string partitionKey, string rowKey)
        {
            this.PartitionKey = partitionKey;
            this.ExperimentId = rowKey;
            this.RowKey = rowKey;
        }

        /// <summary>
        /// Correlates to the ExperimentId of the 'ExerimentRequestMessage' that started the experiment.
        /// </summary>
        public string ExperimentId { get; set; }

        /// <summary>
        /// The name of the group as specified in the ExerimentRequestMessage.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// The list of students as specified in the ExerimentRequestMessage.
        /// </summary>
        public string[] Students { get; set; }

        /// <summary>
        /// Description as specified in the ExerimentRequestMessage.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The time of the experiment start.
        /// </summary>
        public DateTime StartTimeUtc { get; set; }

        /// <summary>
        /// The time of the end of experiment.
        /// </summary>
        public DateTime EndTimeUtc { get; set; }

        /// <summary>
        /// The time of experiment duration.
        /// </summary>
        public long DurationSec { get; set; }

        /// <summary>
        /// The input file (ot files) used to run the experiment.
        /// </summary>
        public string InputFileUrl { get; set; }

        /// <summary>
        /// List of output files.
        /// </summary>
        public string[] OutputFiles { get; set; }
        
        // Add your additional properties related to experiment.

        /// <summary>
        /// TODO. Any other required property that describes the result of experiment.
        /// This is just one example.
        /// </summary>
        public string Accuracy { get; set; }

        /// <summary>
        /// The final status of the experiment.
        /// </summary>
        public ExperimentStatus Status { get; set; }

        /// <summary>
        /// NULL if experiment status is Succedded.
        /// Error message (Exception) if the experiment has failed.
        /// </summary>
        public string Error { get; set; }
        
    }
}
