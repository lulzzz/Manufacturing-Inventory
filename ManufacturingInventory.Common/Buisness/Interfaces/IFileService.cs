using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManufacturingInventory.Common.Buisness.Interfaces {

    public interface IFileService {
        string SaveFile(string name,string startAddress);
        void DeleteFile(string name, string fileReference);
        void RenameFile(string name, string newName, string fileReference); 
    }
}
