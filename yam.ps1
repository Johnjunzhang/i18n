param(
    [string]$command = 'help', 
    [string[]] $files, 
    [string] $runtimeProfile = '',
    [string[]] $ends, 
    [switch] $reverse
)
Push-Location
Set-Location ($MyInvocation.MyCommand.Path | Split-Path -Parent)
try{
    & ".\build\tools\ps-gets\yam.0.0.7\tools\yam.ps1" $command $files $runtimeProfile $ends $reverse
}
finally{
    Pop-Location
}
