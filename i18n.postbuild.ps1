param(
    $projectPath =".\test"
)

trap{
    write-host "Error found: $_" -f red    
    Exit 1
}

$error.clear()
$root = $MyInvocation.MyCommand.Path | Split-Path -parent
. $root\scripts\functions.ps1
$config = Import-Config $root\scripts\config.ini

if( -not (Test-Path $projectPath)) {
	throw "Could not find project path $projectPath."
}

$getTextPath = $root + "\" + $config.gettextPath
if( -not (Test-Path $getTextPath)) {
	throw "Could not find gettext tool."
}


$projectPath = Resolve-Path "$projectPath" -Relative
$localePath = $projectPath + "\locale"
$template = $projectPath + "\locale\messages.pot"

if ( -not (Test-Path $localePath) ) {
	New-Item $localePath -ItemType Directory
}
if ( -not (Test-Path $template) ) {
	New-Item $template -ItemType File
}

$inputFiles = Get-Files $projectPath $config
if( $inputFiles){    	
    iex "$getTextPath\xgettext.exe -LC# -k_ --omit-header --from-code=UTF-8 -o$template $inputFiles"
}

Get-Directory $localePath |  
% {
	$dir = $_.FullName
	$result = Test-Path "$dir\messages.po"
	if( -not $result) {
		Copy-Item $template "$dir\messages.po"
	} 
	else {
		iex "$getTextPath\msgmerge.exe -U $dir\messages.po $template"		
	}
	
}