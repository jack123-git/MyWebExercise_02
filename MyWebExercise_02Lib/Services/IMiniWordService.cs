using System;
using System.Collections.Generic;
using System.Text;
using JackToolLib.Models;

namespace JackToolLib.Services
{
    public interface IMiniWordService
    {
        Task<byte[]> GenerateWordDocumentAsync(string templateFile, Dictionary<string, object> exportData);
    }
}
