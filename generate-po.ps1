param($baseDir, [string[]]$filters, [string[]]$excludeFolders)

trap{
    write-host "Error found: $_" -f red    
    Exit 1
}

$error.clear()

$root = $MyInvocation.MyCommand.Path | Split-Path -parent

if( -not (Test-Path $baseDir)) {
    throw "Could not find path $baseDir."
}

$getTextPath = $root + "\tools\gettext-0.14.4"
if( -not (Test-Path $getTextPath)) {
    throw "Could not find gettext tool."
}

$localePath = $baseDir + "\locale"
$template = $baseDir + "\locale\messages.pot"

if ( -not (Test-Path $localePath) ) {
    New-Item $localePath -ItemType Directory
}
if ( -not (Test-Path $template) ) {
    New-Item $template -ItemType File
}

Function Get-Files {
    $files = Get-ChildItem $baseDir -Recurse -Include $filters | ? {
        -not (Test-IsExcluded $_.DirectoryName)
    } | % { 
        Resolve-Path $_.FullName -Relative
    }

    return $files -Join " "
}

Function Test-IsExcluded($path){
    if( -not $path ){
        return $true
    }
    foreach($excludeFolder in $excludeFolders){
        if ($path.StartsWith($excludeFolder, [System.StringComparison]::InvariantCultureIgnoreCase)) {
            return $true
        }
    }
    $false
}

Function Get-Directory($path) {
    return Get-ChildItem $path -Recurse | ? { $_.PsIsContainer}
}

$inputFiles = Get-Files

if( $inputFiles){       
    iex "$getTextPath\xgettext.exe -LC# -k_ --omit-header --from-code=UTF-8 -o$template $inputFiles"
}

Get-Directory $localePath |  % {
    $dir = $_.FullName
    $target = "$dir\messages.po"
    if(Test-Path $target) {
        & "$getTextPath\msgmerge.exe" -U $target $template
    } else {
        & "$getTextPath\msginit.exe" -i $template -l $_.BaseName '--no-translator' -o $target
    }
}