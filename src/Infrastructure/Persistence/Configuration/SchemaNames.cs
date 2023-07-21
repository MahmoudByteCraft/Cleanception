﻿namespace Cleanception.Infrastructure.Persistence.Configuration;

internal static class SchemaNames
{
    // TODO: figure out how to capitalize these only for Oracle
    public static string Auditing = nameof(Auditing); // "AUDITING";
    public static string Identity = nameof(Identity); // "IDENTITY";
    public static string MultiTenancy = nameof(MultiTenancy); // "MULTITENANCY";
}