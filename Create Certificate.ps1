$mkcertVersion = 'v1.4.3';
$mkcertDir = Get-Location | Join-Path -ChildPath 'mkcert';
$outFile = Join-Path $mkcertDir -ChildPath 'mkcert.exe';

New-Item -ItemType Directory -Path $mkcertDir -ErrorAction SilentlyContinue;

if (-not(Test-Path $outFile -PathType Leaf))
{
  Invoke-WebRequest -Uri "https://github.com/FiloSottile/mkcert/releases/download/$mkcertVersion/mkcert-$mkcertVersion-windows-amd64.exe" -OutFile $outFile;
}

$installMkcert = $outFile + ' -install';
Invoke-Expression $installMkcert;

$certFile = Join-Path $mkcertDir -ChildPath 'localhost.pfx';
$generateCert = $outFile + " -pkcs12 -p12-file $certFile localhost host.docker.internal 127.0.0.1";
Invoke-Expression $generateCert;

$getCAROOT = $outFile + ' -CAROOT';
$rootCert = Invoke-Expression $getCAROOT | Join-Path -ChildPath 'rootCA.pem';
Copy-Item $rootCert -Destination $mkcertDir;
