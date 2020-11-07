namespace FactsGreet.Common
{
    using System;

    public static class Paginator
    {
        public static (int Skip, int Take) GetPagination(int page, int itemsPerPage)
        {
            return ((Math.Max(1, page) - 1) * itemsPerPage, itemsPerPage);
        }
    }
}