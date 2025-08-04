using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Gamegaard.Commons.Editor.Utils
{
    public static class PackageImporter
    {
        public static bool HasPackageInProject(string packageID)
        {
            ListRequest request = Client.List();
            while (!request.IsCompleted) { }

            if (request.Status == StatusCode.Success)
            {
                foreach (PackageInfo package in request.Result)
                {
                    if (package.name == packageID)
                    {
                        return true;
                    }
                }
            }
            else if (request.Status >= StatusCode.Failure)
            {
                Debug.LogError("Could not get list of packages from Package Manager.");
            }
            return false;
        }

        public static void InstallPackage(string packageID)
        {
            AddRequest request = Client.Add(packageID);
            while (!request.IsCompleted) { }

            if (request.Status == StatusCode.Success)
            {
                Debug.Log($"{packageID} imported successfully!");
            }
            else if (request.Status >= StatusCode.Failure)
            {
                Debug.LogError($"{packageID} import failed!");
            }
        }
    }
}