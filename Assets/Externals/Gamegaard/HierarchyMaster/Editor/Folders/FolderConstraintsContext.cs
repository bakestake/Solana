namespace Gamegaard.HierarchyMaster.Editor
{
    public static class FolderConstraintsContext
    {
        private static bool _isInBuildProcess;

        public static bool SuppressPrefabAlerts => _isInBuildProcess || HierarchyMasterSettings.Instance.IgnoreAllFolderPrefabAlerts;

        public static void SetBuildSuppression(bool value)
        {
            _isInBuildProcess = value;
        }
    }
}