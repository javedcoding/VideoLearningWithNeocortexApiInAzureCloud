using System;
using System.Collections.Generic;
using System.Text;

namespace MyCloudProject.Common
{
    public class ExerimentRequestMessage
    {
        /// <summary>
        /// The experiment identifier use to correlate the request with the experiment result.
        /// </summary>
        public string ExperimentId { get; set; }

        /// <summary>
        /// Students who participated in the group.
        /// </summary>
        public string[] Students { get; set; }

        /// <summary>
        /// The name of the input file. You can use instead, the list of string if required.
        /// </summary>
        public string InputFileCircle { get; set; }

        /// <summary>
        /// The name of the input file. You can use instead, the list of string if required.
        /// </summary>
        public string InputFileLine { get; set; }

        /// <summary>
        /// The name of the input file. You can use instead, the list of string if required.
        /// </summary>
        public string InputFileRectangle { get; set; }

        /// <summary>
        /// The name of the input file. You can use instead, the list of string if required.
        /// </summary>
        public string InputFileTriangle { get; set; }

        /// <summary>
        /// The name of the input file. You can use instead, the list of string if required.
        /// </summary>
        public string InputFileVideoConfig { get; set; }

        /// <summary>
        /// The name of the input file. You can use instead, the list of string if required.
        /// </summary>
        public string InputFileHTMConfig { get; set; }

        /// <summary>
        /// The name of the input file. You can use instead, the list of string if required.
        /// </summary>
        public string InputTestCircleFile { get; set; }

        /// <summary>
        /// The name of the input file. You can use instead, the list of string if required.
        /// </summary>
        public string InputTestLineFile { get; set; }

        /// <summary>
        /// The name of the input file. You can use instead, the list of string if required.
        /// </summary>
        public string InputTestRectangleFile { get; set; }

        /// <summary>
        /// The name of the input file. You can use instead, the list of string if required.
        /// </summary>
        public string InputTestTriangleFile { get; set; }

        /// <summary>
        /// The name of the group.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// The name of the project.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Describes the experiment.
        /// </summary>
        public string Description { get; set; }


    }
}
