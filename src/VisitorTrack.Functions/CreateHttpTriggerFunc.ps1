param(
    [String]
    $Name = $(Throw "Function name is required."),

    [String]
    $Method = $(Throw "HTTP method is required (GET, PUT, POST, DELETE).")
)
try {
    Write-Output "Creating HTTP trigger function $Name - $Method..."

    New-Item ".\$Name" -Type Directory
    New-Item ".\$Name\function.json" -Type File

    (Get-Content .\template.json).replace('[METHOD]', $Method.ToLower()).replace('[NAME]', $Name) | Set-Content ".\$Name\function.json"
    
    $exitCode = 0
}
catch {
    Write-Error "*** $_"

    Write-Output "Process aborted."

    $exitCode = 1
}
finally {
    Exit $exitCode
}