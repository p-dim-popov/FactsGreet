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
            const string filler = "word word word word word word word word word word word word word word " +
                                  "word word word word word word word word word word word word word word " +
                                  "word word word word word word word word word word word word word word " +
                                  "word word word word word word word word word word word word word word ";

            const string version1 =
                "this is some\r\nlong ipsum\r\n...\r\na few lines long\r\n\r\n" + filler;
            const string version2 =
                "this is some\r\nlorem ipsum\r\n...\r\na\r\nfew\r\nlines longer\r\n\r\n" + filler;
            const string version3 =
                @"this\r\n\r\n...\r\nlorem ipsum\r\n\r\n" + filler;
            const string version4 =
                @"this\r\n\r\n....\r\n\r\n" + filler;

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
