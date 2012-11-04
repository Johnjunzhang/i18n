Function Extract-TranslatableText($getTextPath, $projectPath, $include, $template){
    Get-ChildItem $projectPath -Recurse -Include $include | 
    	% { 
    		$sourceFiles += $_.FullName + " "
    	}

    $output = (Get-Item $template).FullName
    if( $sourceFiles -ne $null  -and $sourceFiles -ne ""){
	    iex "$getTextPath\xgettext.exe -LC# -k_ --omit-header --from-code=UTF-8 -o$output $sourceFiles" 
	    Write-Host "Extract-TranslatableText From $projectPath folder includes $include." -f cyan
	}
    
}

Function Merge-ExistingPoFile($getTextPath, $localePath, $template) {
	Get-ChildItem $localePath -Recurse |
	    ? { $_.PsIsContainer} |  
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
	Write-Host "Merge-ExistingPoFile From template to existing PO files under $localePath folder." -f cyan
}

Function Import-Config ($configFile) {
	$config = @{}
	Import-CSV $configFile -Delimiter '=' -header 'key','value' | 
		? {$_.key} | 
			% { $config[$_.key.trim()] = $_.value.trim()}
    return $config
}