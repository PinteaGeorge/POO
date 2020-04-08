using System;
using System.IO;
namespace SIzeOfFileOrFolder
{
    public class SizeOfFileOrFolder
    {
        string path;
        private static long size;
        public SizeOfFileOrFolder(string path)
        {
            size = 0;
            this.path = path;

        }
        public static long FileSize(string path)
        {
            var FS = new FileInfo(path);
            return FS.Length;
        }
        public  long DirectorySize(string path)
        {
            
            
            string[] files = Directory.GetFiles(path);
            foreach (string fileName in files)
            {
                size += FileSize(fileName);
            }


            
            string[] subdirectoryEntries = Directory.GetDirectories(path);
            foreach (string subdirectory in subdirectoryEntries)
            {
                size += DirectorySize(subdirectory);
            }

            return size;
        }
        public void TotalSize()
        {
            Console.WriteLine("Total size of File/Folder is: {0} bytes.",size);
        }
    }
}