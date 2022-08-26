using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExperiment.Models
{
    public class OutputModel
    {
        /// <summary>
        /// This class represents the output model of the SE project
        /// </summary>
        private string[] Output { get; set; }

        public OutputModel(string[] output)
        {
            Output = output;
        }
    }
}
