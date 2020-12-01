namespace FactsGreet.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using FactsGreet.Data.Models.Enums;
    using TrueCommerce.Shared.DiffMatchPatch;

    using DbDiff = FactsGreet.Data.Models.Diff;
    using DbPatch = FactsGreet.Data.Models.Patch;
    using DmpDiff = TrueCommerce.Shared.DiffMatchPatch.Diff;
    using DmpPatch = TrueCommerce.Shared.DiffMatchPatch.Patch;

    // Edit > Patch > Diff
    public class DiffMatchPatchService : IDiffMatchPatchService
    {
        private readonly DiffMatchPatch dmp;

        public DiffMatchPatchService(DiffMatchPatch dmp)
        {
            this.dmp = dmp;
        }

        public ICollection<DbPatch> CreateEdit(string oldText, string newText)
        {
            return this.dmp.PatchMake(newText, oldText)
                .Select(x => new DbPatch
                {
                    Diffs = x.Diffs
                        .Select(y => new DbDiff
                        {
                            Operation = (DiffOperation)y.Operation,
                            Text = y.Text,
                        })
                        .ToList(),
                    Length1 = x.Length1,
                    Length2 = x.Length2,
                    Start1 = x.Start1,
                    Start2 = x.Start2,
                })
                .ToList();
        }

        public string ApplyEdits(string text, IEnumerable<IEnumerable<DbPatch>> patches)
        {
            // TODO: try long running async
            return patches.Aggregate(text, this.ApplyEdit);
        }

        public string ApplyEdit(string text, IEnumerable<DbPatch> patches)
        {
            return this.dmp.PatchApply(
                    patches
                        .Select(x => new DmpPatch
                        {
                            Diffs = x.Diffs
                                .Select(y => new DmpDiff((Operation)y.Operation, y.Text))
                                .ToList(),
                            Length1 = x.Length1,
                            Length2 = x.Length2,
                            Start1 = x.Start1,
                            Start2 = x.Start2,
                        })
                        .ToList(),
                    text)
                .FirstOrDefault()
                ?.ToString() ?? string.Empty;
        }
    }
}
