namespace FactsGreet.Services
{
    using TrueCommerce.Shared.DiffMatchPatch;

    public class DiffMatchPatchService
    {
        private readonly DiffMatchPatch dmp;

        public DiffMatchPatchService(DiffMatchPatch dmp)
        {
            this.dmp = dmp;
        }

        public string CreatePatch(string oldText, string newText)
        {
            return this.dmp.PatchToText(this.dmp.PatchMake(newText, oldText));
        }
    }
}
