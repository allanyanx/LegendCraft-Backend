$body = @{
    FirstName = "Admin"
    LastName = "Principal"
    Email = "admin@legendcraft.com"
    Password = "AdminPassword123!"
} | ConvertTo-Json

try {
    Invoke-RestMethod -Uri "http://localhost:5104/api/Auth/register" -Method Post -Body $body -ContentType "application/json"
} catch {
    Write-Output "Register failed: $($_.Exception.Message)"
}

try {
    Invoke-RestMethod -Uri "http://localhost:5104/api/Auth/make-admin/admin@legendcraft.com" -Method Post
} catch {
    Write-Output "Make Admin failed: $($_.Exception.Message)"
}
