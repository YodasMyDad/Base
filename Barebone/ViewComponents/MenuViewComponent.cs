using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Barebone.ViewModels.Shared.Menu;
using ExtCore.Data.Abstractions;

namespace Barebone.ViewComponents
{
    public class MenuViewComponent : ViewComponentBase
    {
        public MenuViewComponent(IStorage storage_) : base(storage_) { }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(new MenuViewModelFactory(this).Create());
        }
    }
}