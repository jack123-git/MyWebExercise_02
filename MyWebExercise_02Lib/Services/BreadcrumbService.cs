using MudBlazor;
using System;
using System.Collections.Generic;
using System.Text;

namespace JackToolLib.Services
{
    public class BreadcrumbService
    {
        public event Action? OnChange;
        public List<BreadcrumbItem> Items { get; private set; } = new();

        public void SetBreadcrumbs(List<BreadcrumbItem> items)
        {
            Items = items;
            OnChange?.Invoke(); // 通知 UI 更新
        }
    }
}
