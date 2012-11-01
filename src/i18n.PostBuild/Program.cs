using System;
using System.Linq;

namespace i18n.PostBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("This post build task requires passing in the $(ProjectDirectory) path");
                return;
            }

            var projectPath = args[0];
            projectPath = projectPath.Trim(new[] {'\"'});

            var gettextExePath = args[1].Trim(new[] { '\"' });

            string gettext = null;
            string msgmerge = null;

            for (int i = 2; i < args.Length; i++)
            {
                if (args[i].StartsWith("gettext:", StringComparison.InvariantCultureIgnoreCase))
                    gettext = args[i].Substring(8);

                if (args[i].StartsWith("msgmerge:", StringComparison.InvariantCultureIgnoreCase))
                    msgmerge = args[i].Substring(9);
            }

            var fileTypeAllowed = System.Configuration.ConfigurationManager.AppSettings["fileType"].Split(',').ToList();

            new PostBuildTask(gettextExePath, projectPath).Execute(fileTypeAllowed, gettext, msgmerge);
        }
    }
}
