using JackToolLib.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace JackToolLib.Repository
{
    public class TemplateRepository: ITemplateRepository
    {
        public async Task<string> GetTextTestTemplateAsync()
        {
            return @"
親愛的 @Model.RecipientName，
感謝您於 @Model.PurchaseDate.ToString(""yyyy/MM/dd"")
購買我們的產品 @Model.ProductName
我們希望你滿意，並期待再次能為您服務!

敬上 @Model.SenderName
";
        }

    }
}
