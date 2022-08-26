using System;
using System.IO;

namespace MyExperiment.Utilities
{
    public static class FileUtilities
    {
        /// <summary>
        /// Retreive localstorage file path location
        /// </summary>
        /// <param name="localfilePath"></param>
        /// <returns></returns>
        public static string GetLocalStorageFilePath(string localFilePath)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                localFilePath);
        }

        /// <summary>
        /// Reading text from file 
        /// </summary>
        /// <param name="localfilePath"></param>
        /// <returns> return content </returns>
        public static string ReadFile(string localFilePath)
        {
            string jsonString = File.ReadAllText(localFilePath);
            return jsonString;
        }

        /// <summary>
        /// Put content in file
        /// </summary>
        public static void WriteDataInFile(string localfilePath, double[] data, int bucketIndex)
        {
            // Creating local file in the ./data/ directory 

            if (!File.Exists(localfilePath))
            {
                File.Create(localfilePath).Close();
            }

            StreamWriter sw = File.AppendText(localfilePath);

            try
            {
                sw.WriteLine();
                sw.WriteLine("*************--Processing started--************");
                sw.Write("For bucket index : " + bucketIndex + " -> ");
                foreach (var d in data)
                {
                    sw.Write(d + "  ");
                }
            }
            finally
            {
                sw.WriteLine();
                sw.WriteLine("*************--Processing ended--************");
                sw.WriteLine();
                sw.Flush();
                sw.Close();
            }


        }
    }
}