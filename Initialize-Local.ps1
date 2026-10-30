$ErrorActionPreference = 'Stop'
$taskEnv = Join-Path $PSScriptRoot '.env'
if (-not (Test-Path -LiteralPath $taskEnv)) {
    $taskRandom = [Security.Cryptography.RandomNumberGenerator]::Create()
    $taskDbBytes = New-Object byte[] 32
    $taskJwtBytes = New-Object byte[] 48
    $taskRandom.GetBytes($taskDbBytes)
    $taskRandom.GetBytes($taskJwtBytes)
    $taskRandom.Dispose()
    $taskDbSecret = [BitConverter]::ToString($taskDbBytes).Replace('-', '')
    $taskJwtSecret = [BitConverter]::ToString($taskJwtBytes).Replace('-', '')
    @("POSTGRES_DB=cete", "POSTGRES_USER=cete", "POSTGRES_PASSWORD=$taskDbSecret", "JWT_KEY=$taskJwtSecret", "CORS_ORIGINS=http://localhost:5173,http://127.0.0.1:5173") | Set-Content -LiteralPath $taskEnv -Encoding utf8
}
Write-Output 'Local environment initialized. Secrets remain in .env.'
