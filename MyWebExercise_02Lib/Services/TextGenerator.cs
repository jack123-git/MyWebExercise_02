using System;
using System.Collections.Generic;
using System.Text;
//using RazorEngine;
//using RazorEngine.Templating;
using JackToolLib.Models;   

namespace JackToolLib.Services
{
    public class TextGenerator
    {
        public async Task<string> GenerateTextAsync(LetterModel model)
        {
            return $@"
親愛的 {model.RecipientName}，
感謝您於 {model.PurchaseDate.ToString("yyyy/MM/dd")}
購買我們的產品 {model.ProductName}
我們希望你滿意，並期待再次能為您服務!

敬上 {model.SenderName}
";
        }

        public async Task<string> GenerateMultiTextAsync(List<LetterModel> models)
        {
            List<string> text = new List<string>();
            foreach (var model in models)
            {
                var txt = await GenerateTextAsync(model);
                text.Add(txt);
            }
            return string.Join("\n", text);
        }


    }
}
