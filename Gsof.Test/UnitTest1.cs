using System.Diagnostics;
using System.Threading.Tasks;
using Gsof.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gsof.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task ProcessExec()
        {
            var process = new Process();

            process.StartInfo.FileName = @"dmidecode.exe";
            process.StartInfo.Arguments = "-s system-uuid";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;

            var result = await process.Exec();
            Assert.AreNotEqual(result.Trim(), string.Empty);
        }

        [TestMethod]
        public async Task ProcessExExec()
        {
            var result = await ProcessEx.Exec(@"dmidecode.exe", "-s system-uuid");
            Assert.AreNotEqual(result.Trim(), string.Empty);
        }
    }
}
