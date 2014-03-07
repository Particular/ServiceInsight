For testing
cinst ServiceInsight.install  -source "C:\Code\ServiceInsight\nugets;http://chocolatey.org/api/v2" -force -pre
cinst ServiceInsight.install  -source "C:\Code\ServiceInsight\nugets" -force -pre

cuninst ServiceInsight.install 

Here is an actual release https://github.com/Particular/ServiceInsight/releases/download/1.0.0-Beta5/Particular.ServiceInsight-1.0.0-Beta5.exe