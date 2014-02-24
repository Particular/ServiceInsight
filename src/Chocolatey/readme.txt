For testing
cinst ServiceInsight.install  -source "C:\Code\ServiceInsight\src\ChocolateyBuild;http://chocolatey.org/api/v2" -force -pre
cinst ServiceInsight.install  -source "C:\Code\ServiceInsight\nugets" -force -pre

cuninst ServiceInsight.install 

Here is an actual release https://github.com/Particular/ServicePulse/releases/download/1.0.0-Beta3/Particular.ServicePulse-1.0.0-Beta3.exe