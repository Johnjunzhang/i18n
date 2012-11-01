using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace i18n
{
    /// <summary>
    /// A post-build task for building a localization message database using GNU xgettext
    /// <see href="http://gnuwin32.sourceforge.net/packages/gettext.htm" />
    /// </summary>
    public class PostBuildTask
    {
        private readonly string gettextExePath;
        private readonly string path;

        public PostBuildTask(string gettextExePath, string path)
        {
            this.gettextExePath = gettextExePath;
            this.path = path;
        }

        ///<summary>
        /// Runs GNU xgettext to extract a messages template file
        ///</summary>
        ///<param name="fileTypeAllowed"></param>
        ///<param name="gettextOptions"> </param>
        ///<param name="msgmerge"> </param>
        public void Execute(List<string> fileTypeAllowed, string gettextOptions = null, string msgmerge = null)
        {
            var manifest = BuildProjectFileManifest(fileTypeAllowed);

            CreateMessageTemplate(manifest, gettextOptions);

            MergeTemplateWithExistingLocales(msgmerge);

            File.Delete(manifest);
        }

        private void MergeTemplateWithExistingLocales(string options)
        {
            var locales = Directory.GetDirectories(string.Format("{0}\\locale\\", path));
            var template = string.Format("{0}\\locale\\messages.pot", path);

            foreach (var messages in locales.Select(locale => string.Format("{0}\\messages.po", locale)))
            {
                if(File.Exists(messages))
                {
                    // http://www.gnu.org/s/hello/manual/gettext/msgmerge-Invocation.html
                    var args = string.Format("{2} -U \"{0}\" \"{1}\"", messages, template, options);
                    RunWithOutput(string.Format("{0}\\msgmerge.exe", gettextExePath), args);
                }
                else
                {
                    File.Copy(template, messages);
                }
            }
        }

        private void CreateMessageTemplate(string manifest, string options)
        {
            // http://www.gnu.org/s/hello/manual/gettext/xgettext-Invocation.html
            var args = string.Format("{2} -LC# -k_ --omit-header --from-code=UTF-8 -o\"{0}\\locale\\messages.pot\" -f\"{1}\"", path, manifest, options);
            RunWithOutput(string.Format("{0}\\xgettext.exe", gettextExePath), args);
        }

        private static void RunWithOutput(string filename, string args)
        {
            var info = new ProcessStartInfo(filename, args)
            {
                UseShellExecute = false,
                ErrorDialog = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            Console.WriteLine("{0} {1}", info.FileName, info.Arguments);
            var process = Process.Start(info);
            while (!process.StandardError.EndOfStream)
            {
                var line = process.StandardError.ReadLine();
                if (line == null)
                {
                    continue;
                }
                Console.WriteLine(line);
            }
        }

        private string BuildProjectFileManifest(List<string> fileTypeAllowed)
        {
            var files = new List<string>();
            fileTypeAllowed.ForEach(fileType => files.AddRange(Directory.GetFiles(path, string.Format("*.{0}", fileType), SearchOption.AllDirectories)));

            var temp = Path.GetTempFileName();
            using(var sw = File.CreateText(temp))
            {
                foreach(var file in files)
                {			
                    sw.WriteLine(file);
                }
            }
            return temp;
        }

    }
}