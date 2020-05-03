using System.Collections.Generic;

namespace dkx86weblog.Models
{
    public class BlogViewModel
    {
        public List<Post> Items { get; private set; }
        public PageViewModel PageModel { get; private set; }

        public BlogViewModel(List<Post> items, PageViewModel pageModel)
        {
            Items = items;
            PageModel = pageModel;
        }
    }
}
