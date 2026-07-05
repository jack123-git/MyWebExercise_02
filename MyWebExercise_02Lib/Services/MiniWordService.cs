using System;
using System.Collections.Generic;
using System.Text;
using JackToolLib.Models;
using MiniSoftware;

namespace JackToolLib.Services
{
    public class MiniWordService : IMiniWordService
    {
        public async Task<byte[]> GenerateWordDocumentAsync(string templateFile, Dictionary<string, object> exportData)
        {
            if (File.Exists(templateFile))
            {
                string tempFile="";
                try
                {
                    tempFile = Path.GetTempPath()+ Path.GetRandomFileName();
                    MiniWord.SaveAsByTemplate(tempFile, templateFile, exportData);
                    var binaryData = File.ReadAllBytes(tempFile);
                    var fileStream = new MemoryStream(binaryData);
                    return fileStream.ToArray();
                }
                catch (Exception)
                {
                    throw;
                }
                finally 
                {
                    //if (File.Exists (tempFile)) 
                    //    File.Delete(tempFile);
                }

                //byte[] templateBytes = await File.ReadAllBytesAsync(templateFile);

                //using (MemoryStream outputStream = new MemoryStream())
                //{
                //    MiniWord.


                //    // Create a MiniWord document from the template stream
                //    MiniWordDocument miniWordDoc = new MiniWordDocument(templateStream);
                //    // Prepare the data for replacement
                //    Dictionary<string, object> dataDictionary = exportModel.ToTemplateDictionary();
                //    // Replace placeholders in the document with actual data
                //    foreach (var key in dataDictionary.Keys)
                //    {
                //        miniWordDoc.ReplacePlaceholder(key, dataDictionary[key]?.ToString() ?? string.Empty);
                //    }
                //    // Save the modified document to a new memory stream
                //    MemoryStream outputStream = new MemoryStream();
                //    miniWordDoc.Save(outputStream);
                //    outputStream.Position = 0; // Reset the position to the beginning of the stream
                //    return outputStream;
                //}
            }

            // Implementation for generating Word document
            //throw new NotImplementedException();

            //MiniWord.

            return null;
        }
    }
}
