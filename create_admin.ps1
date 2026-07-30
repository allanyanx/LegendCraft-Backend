param (
    [string]$FirstName = "Josue",
    [string]$Password = "0806",
    [string]$Email = "josue@legendcraft.com"
)

$body = @{
    FirstName = $FirstName
    LastName = "Admin"
    Email = $Email
    Password = $Password
} | ConvertTo-Json

try {
    Invoke-RestMethod -Uri "http://localhost:5104/api/Auth/register" -Method Post -Body $body -ContentType "application/json"
} catch {
    Write-Output "Register failed: $($_.Exception.Message)"
}

try {
    Invoke-RestMethod -Uri "http://localhost:5104/api/Auth/make-admin/$Email" -Method Post
} catch {
    Write-Output "Make Admin failed: $($_.Exception.Message)"
}
