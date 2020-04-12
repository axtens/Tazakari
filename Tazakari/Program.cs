using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuarterMaster.Infrastructure;

namespace Tazakari
{
    class Program
    {
        static void Main(string[] args)
        {
            var Args = QuarterMaster.Configuration.CommandLineArguments.Arguments(keepSlashes:true);
            if (!Args.ContainsKey("/REMOTE") || !Args.ContainsKey("/LOCAL"))
            {
                Console.WriteLine("Need /REMOTE and /LOCAL");
                Environment.Exit(1);
            }
            if (Args.ContainsKey("/D"))
            {
                Debugger.Launch();
            }

            var remotePath = Args["/REMOTE"];
            var localPath = Args["/LOCAL"];

            var remoteList = Directory.GetFiles(remotePath, "*.md5");

            foreach (var remoteName in remoteList)
            {
                var md5data = File.ReadAllText(remoteName).Split('\t');
                var md5 = md5data[0];
                var fil = md5data[1];
                var localFile = Path.Combine(localPath, fil);
                var remoteFile = Path.Combine(remotePath, fil);
                if (File.Exists(localFile))
                {
                    if (Crypto.GetMD5HashFromFile(localFile) != md5)
                    {
                        OverWrite(remoteFile, localFile);
                        Console.WriteLine($"Copied changed file {remoteFile}");
                    }
                    else
                    {
                        Console.WriteLine($"Ignoring unchanged file {remoteFile}");
                    }
                }
                else
                {
                    OverWrite(remoteFile, localFile);
                    Console.WriteLine($"Copied new file {remoteFile}");
                }
            }
        }

        private static void OverWrite(string remoteFile, string localFile)
        {
            try
            {
                File.Copy(remoteFile, localFile, true);
            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
        }
    }
}
/*

for (var i = 0; i < remoteList.Length; i++) {
  var md5data = CSFile.ReadAllText(remoteList[i]).split(/\t/g);
  var md5 = md5data[0];
  var fil = md5data[1];
  // calculate local's md5 if exists, otherwise just copy
  var localFile = CSPath.Combine(localFolder, fil);
  remoteFile = CSPath.Combine(remoteFolder, fil);
  if (CSFile.Exists(localFile)) {
    if (CSCrypto.GetMD5HashFromFile(localFile) !== md5) {
      // copy
      Overwrite(remoteFile, localFile);
      CSConsole.WriteLine("Copied changed file {0}", remoteFile);
    } else {
      CSConsole.WriteLine("Ignoring unchanged file {0}", remoteFile);
    }
  } else {
    // copy
    Overwrite(remoteFile, localFile);
    CSConsole.WriteLine("Copied new file {0}", remoteFile);
  }
}

function Overwrite(src, dst) {
  CSFile.Copy(src, dst, true);
}

 */
