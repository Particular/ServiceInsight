Profiler for NServiceBus
========================

Essential developer insights for distributed solutions using NServiceBus

Verifying that large distributed solutions function correctly is no easy task, especially with many asynchronous and loosely-coupled processes.

NSB Profiler gives visibility across queues, processes, and machines showing messages whose processing has failed (and for what reason) as well as to which other messages they were related. In addition to the ability to send messages on-the-fly to any process, and see the effect those messages had across the system, the built-in audit log provides developers a ready-made reproduction of any bugs uncovered that they can then replay automatically on their machines.

NSB Profiler is currently in beta, offering a subset of the full functionality of the final product.

![Screenshot](http://i.imgur.com/CvZWVnP.jpg)

Featured Highlights
-------------------

* Message Queue Manager
    + Manage and monitor local and remote MSMQ queues
    + Connect to, create, purge, delete queues
    + Resend error message using “Return to Source”
<br/><br/>
* Endpoint Explorer
    + Central location to view and track multiple endpoints, across multiple machines
<br/><br/>
* End-to-End Message Flow visualization 
    + Visualization of complex processes 
    + Supports multiple endpoints, messages and branching logic
    + Context aware indicators for performance, SLA, Error, retry and dependencies
<br/><br/>
* Deep-insights into message properties
    + Full auditing of message details (including all headers and message body)
    + Performance information & statistics
    + Saga (workflow) details
    + Retry and timeout information and behavior 
    + Detailed Error and full stacktrace data
    + Gateway information and behavior
<br/><br/>
* Integrated full-text search
