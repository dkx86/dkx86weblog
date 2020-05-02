using System;

namespace dkx86weblog.Models
{
    public class PageViewModel
    {
        public static readonly int PAGE_SIZE = 12;
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }

        public PageViewModel(int itemsCount, int pageNumber)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(itemsCount / (double)PAGE_SIZE);
        }

        public bool HasPreviousPage
        {
            get
            {
                return PageNumber > 1;
            }
        }

        public bool HasNextPage
        {
            get
            {
                return PageNumber < TotalPages;
            }
        }
    }
}
