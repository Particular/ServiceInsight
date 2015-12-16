$productName = "ServiceInsight";

$url = "https://github.com/Particular/$productName/releases/download/$env:chocolateyPackageVersion/$productName-$env:chocolateyPackageVersion.exe"

Install-ChocolateyPackage $productName 'exe' '/quiet' $url