using Huddle01.Helper;
using System;
using System.Collections.Generic;
using UnityEngine;
using ProtoPermissions = Permissions;

namespace Huddle01.Core 
{
    public class Permissions
    {
        public string Role { get; set; }

        public static CustomValueNotifier<Dictionary<string, object>> permissionsValueNotifier =
            new CustomValueNotifier<Dictionary<string, object>>(
            new Dictionary<string, object>
            {
            { "admin", false },
            { "canConsume", true },
            { "canProduce", true },
            { "canProduceSources", new Dictionary<string, bool>
                {
                    { "cam", true },
                    { "mic", true },
                    { "screen", true }
                }
            },
            { "canSendData", true },
            { "canRecvData", true },
            { "canUpdateMetadata", true },
            { "role", null }
            });

        public Dictionary<string, object> Acl
        {
            get { return permissionsValueNotifier.Value; }
        }

        public void UpdatePermissions(ProtoPermissions updatedPermissions)
        {
           permissionsValueNotifier.Value = new Dictionary<string, object>
            {
                { "admin", updatedPermissions?.Admin ?? null },
                { "canConsume", updatedPermissions?.CanConsume ?? null },
                { "canProduce", updatedPermissions?.CanProduce ?? null },
                { "canProduceSources", updatedPermissions?.CanProduceSources ?? null },
                { "canSendData", updatedPermissions?.CanSendData ?? null },
                { "canRecvData", updatedPermissions?.CanRecvData ?? null },
                { "canUpdateMetadata", updatedPermissions?.CanUpdateMetadata ?? null },
            };
        }

        public Permissions() { }

        public void Reset()
        {
            permissionsValueNotifier.Value.Clear();
        }

        public bool CheckPermission(PermissionType? permissionTypeCheck,ProduceSources? produceSourcesCheck)
        {
            var permissions = permissionsValueNotifier.Value;
            // Check if permission type is allowed
            bool isPermissionTypePermitted = false;
            if (permissionTypeCheck != null && permissions.TryGetValue(permissionTypeCheck.ToString(), out object permissionTypeValue))
            {
                isPermissionTypePermitted = (bool)permissionTypeValue;
            }

            // Check if produce sources are allowed
            bool isProduceSourcesPermitted = false;
            if (produceSourcesCheck != null && permissions.TryGetValue("canProduceSources", out object produceSourcesDict))
            {
                if (produceSourcesDict is Dictionary<string, bool> produceSourcesMap)
                {
                    produceSourcesMap.TryGetValue(produceSourcesCheck.ToString(), out isProduceSourcesPermitted);
                }
            }

            // If either permission check passes, return true
            if (isPermissionTypePermitted || isProduceSourcesPermitted)
            {
                return true;
            }

            // Log error if neither permission is granted
            if (permissionTypeCheck != null)
            {
                Debug.LogError($"{permissionTypeCheck} access required.");
            }
            if (produceSourcesCheck != null)
            {
                Debug.LogError($"{produceSourcesCheck} access required.");
            }

            return false;
        }

    }

    public enum ProduceSources
    {
        cam,
        mic,
        screen
    }

    public enum PermissionType
    {
        admin,
        canConsume,
        canProduce,
        canSendData,
        canRecvData,
        canUpdateMetadata
    }
}