using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace Gsof.Extensions
{
    public static class FileInfoExtension
    {
        public static bool HasWriteable(this DirectoryInfo p_fileInfo)
        {
            var fileInfo = p_fileInfo;
            if (fileInfo == null)
            {
                return false;
            }

            PermissionSet permissionSet = new PermissionSet(PermissionState.None);
            FileIOPermission writePermission = new FileIOPermission(FileIOPermissionAccess.Write, fileInfo.);

            permissionSet.AddPermission(writePermission);

            if (permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet))
            {
                // You have write permissions
            }
            else
            {
                // You don't have write permissions
            }
        }
    }
}
