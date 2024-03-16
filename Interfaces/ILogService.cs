// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace BulkThumbnailCreator.Interfaces;

public interface ILogService
{
    Task LogError(string message);

    Task LogWarning(string message);

    Task LogInformation(string message);

    Task LogException(string message);

    event Action<string> LogEntryAdded;
}
