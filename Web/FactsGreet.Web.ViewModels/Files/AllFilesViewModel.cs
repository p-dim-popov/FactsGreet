namespace FactsGreet.Web.ViewModels.Files
{
    using System.Collections.Generic;

    public class AllFilesViewModel
    {
        public int Count { get; set; }

        public ICollection<FileViewModel> Files { get; set; }

        public long UsedSize { get; set; }
    }
}
