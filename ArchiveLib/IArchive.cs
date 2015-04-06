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

        void CopyToArchive(string sourceFilePath, string destinationFilePath, bool allowOverwrite = true);

        void CopyFromArchive(string sourceFilePath, string destinationFilePath, bool allowOverwrite = true);

        void DeleteInArchive(string destinationFilePath, bool exceptionIfNotFound = false);

        //void CopyToArchiveBt();
        //void CopyFromArchiveBt();
        //void CopyWithingArchive();
        //void DeleteInArchive();
        //void InsureArchiveDirectory();

    }
}
