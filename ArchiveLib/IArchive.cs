using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveLib
{
    public interface IArchive
    {
        void CopyToArchive();
        void CopyFromArchive();
        void CopyToArchiveBt();
        void CopyFromArchiveBt();
        void CopyWithingArchive();
        void DeleteInArchive();
        void InsureArchiveDirectory();
        void GetFileInfo();
    }
}
