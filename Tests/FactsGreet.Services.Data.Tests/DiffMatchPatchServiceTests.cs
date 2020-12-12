namespace FactsGreet.Services.Data.Tests
{
    using TrueCommerce.Shared.DiffMatchPatch;
    using Xunit;

    public class DiffMatchPatchServiceTests : Tests<DiffMatchPatchService>
    {
        [Fact]
        public void Test_CreateEdit_Then_ApplyEdits()
        {
            var service = new DiffMatchPatchService(new DiffMatchPatch());
            const string version1 =
                @"this is some
long ipsum
...
a few lines long

" +
                "word word word word word word word word word word word word word word " +
                "word word word word word word word word word word word word word word " +
                "word word word word word word word word word word word word word word " +
                "word word word word word word word word word word word word word word ";
            const string version2 =
                @"this is some
lorem ipsum
...
a
few
lines longer

" +
                "word word word word word word word word word word word word word word " +
                "word word word word word word word word word word word word word word " +
                "word word word word word word word word word word word word word word " +
                "word word word word word word word word word word word word word word ";
            const string version3 =
                @"this

...
lorem ipsum

" +
                "word word word word word word word word word word word word word word " +
                "word word word word word word word word word word word word word word " +
                "word word word word word word word word word word word word word word " +
                "word word word word word word word word word word word word word word ";
            const string version4 =
                @"this

....

" +
                "word word word word word word word word word word word word word word " +
                "word word word word word word word word word word word word word word " +
                "word word word word word word word word word word word word word word " +
                "word word word word word word word word word word word word word word ";

            var edit1 = service.CreateEdit(version1, version2);
            var edit2 = service.CreateEdit(version2, version3);
            var edit3 = service.CreateEdit(version3, version4);

            var version2FromEdits = service.ApplyEdits(version4, new[] { edit3, edit2 });
            Assert.Equal(version2, version2FromEdits);
            var version1FromVersion2FromEdits = service.ApplyEdits(version2, new[] { edit1 });
            Assert.Equal(version1, version1FromVersion2FromEdits);
        }
    }
}
