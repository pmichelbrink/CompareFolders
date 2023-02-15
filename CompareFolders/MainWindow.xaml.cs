using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CompareFolders
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            System.IO.DirectoryInfo dir1 = new System.IO.DirectoryInfo(txtFolder1.Text);
            System.IO.DirectoryInfo dir2 = new System.IO.DirectoryInfo(txtFolder2.Text);

            // Take a snapshot of the file system.  
            IEnumerable<System.IO.FileInfo> fileList1 = dir1.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
            IEnumerable<System.IO.FileInfo> fileList2 = dir2.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

            IEnumerable<System.IO.DirectoryInfo> folderList1 = dir1.GetDirectories("*.*", System.IO.SearchOption.AllDirectories);
            IEnumerable<System.IO.DirectoryInfo> folderList2 = dir2.GetDirectories("*.*", System.IO.SearchOption.AllDirectories);

            //A custom file comparer defined below  
            FileCompare myFileCompare = new FileCompare();
            FolderCompare myFolderCompare = new FolderCompare();

            // This query determines whether the two folders contain  
            // identical file lists, based on the custom file comparer  
            // that is defined in the FileCompare class.  
            // The query executes immediately because it returns a bool.  
            bool areIdentical = fileList1.SequenceEqual(fileList2, myFileCompare);

            if (areIdentical == true)
            {
                txtResults.Text += "The two folders are the same" + Environment.NewLine + Environment.NewLine;
            }
            else
            {
                txtResults.Text += "The two folders are not the same" + Environment.NewLine + Environment.NewLine;
            }

            // Find the common files. It produces a sequence and doesn't
            // execute until the foreach statement.  
            //var queryCommonFiles = list1.Intersect(list2, myFileCompare);

            //if (queryCommonFiles.Any())
            //{
            //    Console.WriteLine("The following files are in both folders:");
            //    foreach (var v in queryCommonFiles)
            //    {
            //        Console.WriteLine(v.FullName); //shows which items end up in result list  
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("There are no common files in the two folders.");
            //}

            // Find the set difference between the two folders.  
            // For this example we only check one way.  
            var queryList1Only = (from file in fileList1
                                  select file).Except(fileList2, myFileCompare);

            txtResults.Text += "The following files are in Folder 1 but not Folder 2:" + Environment.NewLine;
            foreach (var v in queryList1Only)
            {
                txtResults.Text += v.FullName + Environment.NewLine;
            }

            var queryList2Only = (from file in fileList2
                                  select file).Except(fileList1, myFileCompare);

            txtResults.Text += Environment.NewLine + "The following files are in Folder 2 but not Folder 1:" + Environment.NewLine;
            foreach (var v in queryList2Only)
            {
                txtResults.Text += v.FullName + Environment.NewLine;
            }

            var queryFolderList1Only = (from folder in folderList1
                                  select folder).Except(folderList2, myFolderCompare);

            txtResults.Text += Environment.NewLine + "The following folders are in Folder 1 but not Folder 2:" + Environment.NewLine;
            foreach (var v in queryFolderList1Only)
            {
                txtResults.Text += v.FullName + Environment.NewLine;
            }

            var queryFolderList2Only = (from folder in folderList2
                                  select folder).Except(folderList1, myFolderCompare);

            txtResults.Text += Environment.NewLine + "The following folders are in Folder 2 but not Folder 1:" + Environment.NewLine;
            foreach (var v in queryFolderList2Only)
            {
                txtResults.Text += v.FullName + Environment.NewLine;
            }

        }
    }

    class FileCompare : System.Collections.Generic.IEqualityComparer<System.IO.FileInfo>
    {
        public FileCompare() { }

        public bool Equals(System.IO.FileInfo f1, System.IO.FileInfo f2)
        {
            return (f1.Name == f2.Name &&
                    f1.Length == f2.Length);
        }

        // Return a hash that reflects the comparison criteria. According to the
        // rules for IEqualityComparer<T>, if Equals is true, then the hash codes must  
        // also be equal. Because equality as defined here is a simple value equality, not  
        // reference identity, it is possible that two or more objects will produce the same  
        // hash code.  
        public int GetHashCode(System.IO.FileInfo fi)
        {
            string s = $"{fi.Name}{fi.Length}";
            return s.GetHashCode();
        }
    }
    class FolderCompare : System.Collections.Generic.IEqualityComparer<System.IO.DirectoryInfo>
    {
        public FolderCompare() { }

        public bool Equals(System.IO.DirectoryInfo f1, System.IO.DirectoryInfo f2)
        {
            return (f1.Name == f2.Name);
        }

        // Return a hash that reflects the comparison criteria. According to the
        // rules for IEqualityComparer<T>, if Equals is true, then the hash codes must  
        // also be equal. Because equality as defined here is a simple value equality, not  
        // reference identity, it is possible that two or more objects will produce the same  
        // hash code.  
        public int GetHashCode(System.IO.DirectoryInfo fi)
        {
            string s = $"{fi.Name}";
            return s.GetHashCode();
        }
    }

}
