## ServiceInsight

Essential insights for distributed solutions

Verifying that large distributed solutions function correctly is no easy
task, especially with many asynchronous and loosely-coupled
processes.
ServiceInsight gives you visibility across queues, processes, and
machines showing messages whose processing has failed (and for what reason) as well as their relation to other messages.


http://docs.particular.net/Serviceinsight/getting-started-overview

### How to build

You can build ServiceInsight using Visual Studio 2015 or later.

#### Prerequisites

If using Visual Studio 2017, ensure that you have installed:

- Workloads
  - .NET desktop development
- Individual components
  - Blend for Visual Studio SDK for .NET

You can check whether the prerequisites have been installed by running the Visual Studio Installer and clicking "Modify".

### Troubleshooting

The following compilation error in Visual Studio 2017 indicates that *Blend for Visual Studio SDK for .NET* has not been installed. See ["prerequisites"](#prerequisites).

> Could not resolve this reference. Could not locate the assembly "System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL". Check to make sure the assembly exists on disk. If this reference is required by your code, you may get compilation errors.
