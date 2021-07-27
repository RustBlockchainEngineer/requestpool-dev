using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Foundation.Core
{
    public class CustomBundleOrderer: IBundleOrderer  
    {
        private bool _OrderFiles = false;
        public CustomBundleOrderer(bool orderFiles)
        {
            _OrderFiles = orderFiles;
        }
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            if (_OrderFiles)
                return files.Where(f => !f.VirtualFile.Name.ToLower().StartsWith("x")).OrderBy(f => f.VirtualFile.Name);
            else
                return files.Where(f => !f.VirtualFile.Name.ToLower().StartsWith("x"));

        }
    }
}