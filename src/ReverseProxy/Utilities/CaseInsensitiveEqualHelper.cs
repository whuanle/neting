// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Yarp.ReverseProxy.Utilities;

internal static class CaseInsensitiveEqualHelper
{
    internal static bool Equals(IReadOnlyList<string>? list1, IReadOnlyList<string>? list2)
    {
        return CollectionEqualityHelper.Equals(list1, list2, StringComparer.OrdinalIgnoreCase);
    }

    internal static int GetHashCode(IReadOnlyList<string>? values)
    {
        return CollectionEqualityHelper.GetHashCode(values, StringComparer.OrdinalIgnoreCase);
    }
}
