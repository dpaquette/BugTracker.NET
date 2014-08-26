$startingDate = Get-Date
echo (Get-Date -Format "yyyy-MM-dd")
$counter = 0
Get-ChildItem "Blog *md" | ForEach-Object { 
	
	$content = Get-Content $_
	$blogName = $_.Name -replace "Blog \d+ - ","" -replace "\.md",""
	
	$header = "---`n"
	$header += "layout: post`n"
	$header += "title:  ""$($blogName)""`n"
	$header += "date:   $($startingDate.AddDays($counter).ToString("yyyy-MM-dd"))`n"
	$header += "categories: asp.net`n"
	$header += "author: David Paquette and Simon Timms`n"
	$header += "---`n"
	del "blog/_posts/$($startingDate.ToString("yyyy-MM-dd"))-$($blogName).md"
	Add-Content -path "blog/_posts/$($startingDate.ToString("yyyy-MM-dd"))-$($blogName).md" -value $header
	Add-Content -path "blog/_posts/$($startingDate.ToString("yyyy-MM-dd"))-$($blogName).md" -value $content
	#cp $_.Name "blog/_posts/$($startingDate.ToString("yyyy-MM-dd"))-$($_.Name)" 
	$counter += 1
}
