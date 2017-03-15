/*
 * 
In assignment 2 you have setup a batch processing system, in which you were able to parse a folder and its subfolders and use a filter to 
look for certain txt files and do processing on them. Here we are going to do similar processing, but this time we are going to build 
projects in each folder we visit. You can also use the batchprocessing project I gave you in class as a starting point, but you should 
already have your own version coming from assignment 2.

There is attached a zip file to this assignment. I have also put the zip file on doc sharing. It contains a folder named threadSamples 
containing 7 vc++ projects. Our goal is to modify our batch file system, naming it myBatchBuild,  to visit each project and build it 
using msbuild, the same way we did for a vc++ project in the class. 
All these projects can already be loaded using the solution file which is in this zip folder. You can try and build them using Visual 
Studio in both Debug adn Release configurations. But now we want to do the building using myBatchBuild app. You can use the msbuild script 
that are available in each project, as your starting point but you will have to modify those msbuild scripts based on what is needed bellow.

--Your batch file should provide command line options so we can provide configuration option, i.e. debug vs release. 
If any of the msbuild projects missing any of the configuration setting you should modify those project files manually and add them.
--We should have 3 targets for each project: Build, Clean, Rebuild, very similar to the way we did in class. Rebuild should run targets Clean and Build.
--When we run Target Clean, it should delete  Debug and Release Folder that are generated when we build the project.  
For this step you need to use the following command >rmdir dirname /S /Q. Here dirname is the name of the folder you want to delete.

--You must add a post build event to each project, so that if the configuration is a release build, 
it copies the exe file to subfolder under the main folder, named bin. This means after running command >myBatchBuild release we should have 
folder threadSamples\bin containing the exe files for all 7 projects. 
*/


using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using BatchFramework;


namespace MyBuildApp
{
    class Program
    {

        static Process process = new Process();
        static string buildmode = "Release", target = "Build";
        static long sTotalTextFilesLength = 0;
        static void testNotify(string msg)
        {
            Console.WriteLine(msg);
        }
        static void AccumulateLength(IFileAccessLogic lo, FileInfo fi)
        {
            sTotalTextFilesLength += fi.Length;
        }

        static void Main(string[] args)
        {
            process.StartInfo.FileName = "C:/Windows/Microsoft.NET/Framework64/v4.0.30319/msbuild.exe";
            
            if (args.Length < 1 || args[0] == "--help" || args[0] == "/help" || args[0] == "--help")
            {
                Console.Write(args[0]);
                Console.WriteLine("\n" +
                    "\nUsage: MyBuildApp -{buildmode} -{target} folderPath\n\nExample: MyBuildApp -Release -Build H:\\threadSamples\n");
                Console.ReadKey();
                return;
            }
            //process.StartInfo.FileName = "msbuild";
            if(args.Length>2)
            {
                if (args[1] == "-Release" || args[2] == "-Release")
                {
                    buildmode = "Release";
                }
                if(args[1]=="-Debug" || args[2] == "-Debug")
                {
                    buildmode = "Debug";
                }
                if(args[1]=="-Rebuild" || args[2]=="-Rebuild")
                {
                    target = "Rebuild";
                }
                if(args[1]=="-Clean" || args[2]=="-Clean")
                {
                    target = "Clean";
                }
            }
            
            //process.StartInfo.Arguments = "somefile.txt";
            //string directoryPath = args[0];
            string directoryPath = args[args.Length-1];// ChooseDirectory();
            FileAccessLogic logic = new FileAccessLogic();
            logic.Recursive = true;
            logic.FilePattern = "*.vcxproj";
            logic.onProcess += new FileAccessProcessEventHandler(OnProcessSimpleList);
            logic.onNotify += new FileAccessNotifyEventHandler(OnNotify);

            Console.WriteLine("");
            Console.WriteLine("Processing file or folder " + directoryPath);
            Console.WriteLine("Press any key to start:");
            Console.ReadKey();
            Console.WriteLine("************************************");
            logic.Execute(directoryPath);
            Console.WriteLine("Build Complete");
            Console.WriteLine("************************************");
            Console.WriteLine("");
            Console.ReadKey();

        }

        private static void OnProcessSimpleList(object sender,
            ProcessEventArgs e)
        {
            if (e.Logic.Cancelled)
                return;
            //Process.Start("notepad somefile.txt");
            //Console.Write(e.FileInfo.DirectoryName);
            process.StartInfo.WorkingDirectory = e.FileInfo.DirectoryName;
            process.StartInfo.Arguments = "/p:Configuration=" + buildmode + " /target:" + target + " " + e.FileInfo.Name;
            Console.WriteLine("{0}ing Project {1}!!!",target, e.FileInfo.FullName);
            Console.WriteLine("before");
            process.Start();
            if(target == "Clean")   
            {
                Console.WriteLine("after");
                ProcessStartInfo Info = new ProcessStartInfo();
                Info.Arguments = "/C rmdir /S /Q " + e.FileInfo.DirectoryName + "\\" + buildmode;
                Info.WindowStyle = ProcessWindowStyle.Hidden;
                Info.CreateNoWindow = true;
                Info.FileName = "cmd.exe";
                Process.Start(Info);

                //process.StartInfo.WorkingDirectory = "H:/";
                //process.StartInfo.Arguments = e.FileInfo.DirectoryName + "\\" + buildmode + " /S /Q";

                //Console.WriteLine(process.StartInfo.Arguments);
                //Console.ReadKey();
                //process.StartInfo.FileName = "rmdir";
                //process.Start();
                //process.StartInfo.FileName = "C:/Windows/Microsoft.NET/Framework64/v4.0.30319/msbuild.exe";
            }
            //e.Logic.Notify(String.Format("Listing \t{0} \t{1} \t{2}", String.Format("{0,-50}", e.FileInfo.FullName), e.FileInfo.Length + " bytes", File.GetLastWriteTime(e.FileInfo.FullName)));
            AccumulateLength(e.Logic, e.FileInfo);
        }
        private static void OnNotify(object sedner, NotifyEventArgs e)
        {
            //Console.WriteLine(e.Message);
            testNotify(e.Message);
        }

        private static string ChooseDirectory()
        {
            bool validChoice = false;
            string directoryChoice = "";

            while (validChoice == false)
            {
                Console.WriteLine("Please specify top level directory in full:");
                directoryChoice = Console.ReadLine().Trim();
                if (Directory.Exists(directoryChoice))
                    validChoice = true;
                else
                {
                    Console.WriteLine("Invalid directory. Try again!");
                    Console.WriteLine("");
                }
            }

            return directoryChoice;
        }
    }
}
