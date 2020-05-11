using System.Collections.Generic;

namespace dkx86weblog.Models
{
    public class PhotoViewModel
    {
        public List<Photo> Items { get; private set; }
        public PageViewModel PageModel { get; private set; }

        public PhotoViewModel(List<Photo> items, PageViewModel pageModel)
        {
            Items = items;
            PageModel = pageModel;
        }
    }
}
