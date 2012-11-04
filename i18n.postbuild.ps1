param(
    $projectPath =".\test"
)

trap{
    write-host "Error found: $_" -f red    
    Exit 1
}

$error.clear()

$root = $MyInvocation.MyCommand.Path | Split-Path -parent
. functions.ps1

$config = Import-Config $root\config.ini
$getTextPath = $root + "\" + $config.gettextPath

if( -not (Test-Path $getTextPath)) {
		throw "Could not find gettext tool."
}

$localePath = $projectPath + "\locale"
$template = $projectPath + "\locale\messages.pot"
if ( -not (Test-Path $localePath) ) {
		New-Item $localePath -ItemType Directory
}
if ( -not (Test-Path $template) ) {
	New-Item $template -ItemType File
}

$include = [string[]]($config.sourceFileType -Split ',' | % { "*.{0}" -f $_.trim() })

Extract-TranslatableText $getTextPath $projectPath $include $template 
Merge-ExistingPoFile $getTextPath $localePath $template 

