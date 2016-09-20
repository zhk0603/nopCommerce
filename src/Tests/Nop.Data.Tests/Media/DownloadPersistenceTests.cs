using Nop.Core;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Media
{
    [TestFixture]
    public class DownloadPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_download()
        {
            var download = TestHelper.GetGeDownload();

            var fromDb = SaveAndLoadEntity(download);
            fromDb.ShouldNotBeNull();
            fromDb.DownloadGuid.ShouldEqual(download.DownloadGuid);
            fromDb.UseDownloadUrl.ShouldEqual(true);
            fromDb.DownloadUrl.ShouldEqual("http://www.someUrl.com/file.zip");
            fromDb.DownloadBinary.ShouldEqual(new byte[] { 1, 2, 3 });
            fromDb.ContentType.ShouldEqual(MimeTypes.ApplicationXZipCo);
            fromDb.Filename.ShouldEqual("file");
            fromDb.Extension.ShouldEqual(".zip");
            fromDb.IsNew.ShouldEqual(true);
        }
    }
}
