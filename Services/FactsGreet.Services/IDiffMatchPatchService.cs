namespace FactsGreet.Services
{
    using System.Collections.Generic;

    using DbDiff = FactsGreet.Data.Models.Diff;
    using DbPatch = FactsGreet.Data.Models.Patch;

    public interface IDiffMatchPatchService
    {
        public ICollection<DbPatch> CreateEdit(string oldText, string newText);

        public string ApplyEdits(string text, IEnumerable<IEnumerable<DbPatch>> patches);

        public string ApplyEdit(string text, IEnumerable<DbPatch> patches);
    }
}
