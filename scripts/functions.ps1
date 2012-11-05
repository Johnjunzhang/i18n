Function Import-Config ($configFile) {
	$config = @{}
	Import-CSV $configFile -Delimiter '=' -header 'key','value' | 
		? {$_.key} | 
			% { $config[$_.key.trim()] = $_.value.trim()}
    return $config
}

Function Get-Files ($path, $config) {
	$include = [string[]]($config.sourceFileType -Split ',' | % { "*.{0}" -f $_.trim() })
	$exclude = [string[]]($config.excludeFiles -Split ',' | % { "{0}*" -f $_.trim() })
	$files = Get-ChildItem $path -Recurse -Include $include -Exclude $exclude | 
	   	% { 
    		Resolve-Path $_.FullName -Relative
    	}
    return $files -Join " "
}

Function Get-Directory($path) {
	return Get-ChildItem $path -Recurse | ? { $_.PsIsContainer}
}