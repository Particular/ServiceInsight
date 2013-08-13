﻿using System.ComponentModel;

namespace Particular.ServiceInsight.Desktop.Models
{
    public enum MessageStatus
    {
        [Description("Failed")]
        Failed = 1,
        
        [Description("Repeated Failures")]
        RepeatedFailures = 2,

        [Description("Successful")]
        Successful = 3,

        [Description("Retry Requested")]
        RetryIssued = 4
    }
}