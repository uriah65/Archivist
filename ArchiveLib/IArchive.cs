using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveLib
{
    public interface IArchive
    {
        T WrapAction<T>(Func<T> action);

        FileInfo GetFileInfo(string archiveFilePath);

        void CopyFileTo(string sourceFilePath, string destinationFilePath, bool allowOverwrite = true);

        void CopyFileFrom(string sourceFilePath, string destinationFilePath, bool allowOverwrite = true);

        void CopyFileWithin(string sourceFilePath, string destinationFilePath, bool allowOverwrite = true, bool deleteSource = false);

        void DeleteFile(string archiveFile, bool exceptionIfNotFound = false);

        void InsureDirectory(string archiveDirectory);       
    }
}
