using System;
using System.Collections.Generic;
using System.Text;

namespace JackToolLib.Interfaces
{
    public interface ITemplateRepository
    {
        Task<string> GetTextTestTemplateAsync();
    }
}
