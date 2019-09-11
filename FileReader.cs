using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace GUI
{
    public static class FileReader
    {
        private static string errorMessage = "Something went wrong.";

        public static string ReadFromUrl(string url)
        {
            try {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead(url);
                StreamReader reader = new StreamReader(stream);
                String fileContent = reader.ReadToEnd();
                stream.Close();
                reader.Close();

                return fileContent;
            }
            catch (Exception) {
                throw new FileNotFoundException(errorMessage);
            }
            
        }
    }
}
