using System;
using System.IO;
using System.Security;
using System.Security.Permissions;

namespace Gsof.Extensions
{
    public static class DirectoryInfoExtension
    {
        public static bool HasWriteable(this DirectoryInfo p_dirInfo)
        {
            var dirInfo = p_dirInfo;
            if (dirInfo == null)
            {
                return false;
            }

            PermissionSet permissionSet = new PermissionSet(PermissionState.None);
            var ps = new FileIOPermission(FileIOPermissionAccess.Write, dirInfo.FullName);

            permissionSet.AddPermission(ps);

            return permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet);
        }
    }
}
