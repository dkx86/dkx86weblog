using System.Collections.Generic;

namespace dkx86weblog.Models
{
    public class DigitalPackageViewModel
    {
        public List<DigitalPackage> Items { get; private set; }
        public PageViewModel PageModel { get; private set; }

        public DigitalPackageViewModel(List<DigitalPackage> items, PageViewModel pageModel)
        {
            Items = items;
            PageModel = pageModel;
        }
    }
}
