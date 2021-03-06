﻿using System;

namespace Adorable.Auditing
{
    /// <summary>
    /// Used to disable auditing for a single method or
    /// all methods of a class or interface.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DisableAuditingAttribute : Attribute
    {

    }
}