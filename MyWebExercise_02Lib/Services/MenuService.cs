using JackToolLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using MudBlazor;

namespace JackToolLib.Services
{
    public class MenuService
    {
        public async Task<List<NavMenuItem>> GetMenus()
        {
            // 模擬從資料庫讀取
            return new List<NavMenuItem>
            {
                new NavMenuItem { Title = "首頁", Icon = Icons.Material.Filled.Home, Href = "/" }
                ,new NavMenuItem
                {
                    Title = "資料庫作業",
                    SubMenu = new List<NavMenuItem>
                    {
                        new NavMenuItem { Title = "資料庫連線字串", Href = "/generate-connectionstring", Icon = @Icons.Material.Filled.Storage }
                        ,new NavMenuItem { Title = "TableSchema處理", Href = "/exportdbschema", Icon = Icons.Material.Filled.ContactEmergency }
                        ,new NavMenuItem { Title = "Sp處理", Href = "/generatesp", Icon = Icons.Material.Filled.ContactEmergency }
                        //, new NavMenuItem { Title = "Template測試", Href = "/generate-letter", Icon = Icons.Material.Filled.Business }
                        //, new NavMenuItem { Title = "職稱代碼設定", Href = "/posts", Icon = Icons.Material.Filled.Work }
                    }
                }
                //,new NavMenuItem
                //{
                //    Title = "報表查詢作業",
                //    SubMenu = new List<NavMenuItem>
                //    {
                //        new NavMenuItem { Title = "接收刷卡資料", Href = "/receiverecords", Icon = Icons.Material.Filled.Devices }
                //        ,new NavMenuItem { Title = "刷卡資料查詢", Href = "/cardrecords", Icon = Icons.Material.Filled.Newspaper }
                //        //, new NavMenuItem { Title = "操作紀錄查詢", Href = "/operationrecords", Icon = Icons.Material.Filled.Receipt }
                //    }
                //}
                //,new NavMenuItem
                //{
                //    Title = "系統設定作業",
                //    SubMenu = new List<NavMenuItem>
                //    {
                //        new NavMenuItem { Title = "使用者管理", Href = "/users", Icon = Icons.Material.Filled.ManageAccounts },
                //        new NavMenuItem { Title = "刷卡位置設定", Href = "/doors", Icon = Icons.Material.Filled.PhonelinkSetup }
                //    }
                //}
            };
        }
    }
}
