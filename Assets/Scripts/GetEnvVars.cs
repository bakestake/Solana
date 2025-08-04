using CandyCoded.env;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bakeland
{
    public class GetEnvVars : MonoBehaviour
    {
        public static string Get(string key, bool throwIfMissing = true)
        {
            if (env.TryParseEnvironmentVariable(key, out string value) && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            if (throwIfMissing)
            {
                throw new Exception($"Failed to load required environment variable: {key}");
            }

            return null;
        }

    }
}
