function VeloTransfer.SetEnvVariable 
{
    param (
        [Parameter(Mandatory=$true)][string]$VariableName,
        [Parameter(Mandatory=$true)][string]$Value,
        [Parameter(Mandatory=$true)][boolean]$OverrideValues
    )
    
    $oldValue = [System.Environment]::GetEnvironmentVariable($VariableName)
    $oldValueNotSet = [string]::IsNullOrEmpty($oldValue) 
    
    if ($oldValueNotSet)
    {
        Write-Host ("Setting value for '{0}'" -f $VariableName)
        Write-Host ("               to '{0}'" -f $Value)
        [System.Environment]::SetEnvironmentVariable($VariableName, $Value)
    }
    elseif ($OverrideValues -and $oldValue -ne $Value)
    {
        Write-Host ("Overriding value for '{0}'" -f $VariableName)
        Write-Host ("                from '{0}'" -f $oldValue)
        Write-Host ("                  to '{0}'" -f $Value)
        [System.Environment]::SetEnvironmentVariable($VariableName, $Value)    
    }
    else
    {
        Write-Host ("Leave the value for '{0}'" -f $VariableName)
        Write-Host ("              as is '{0}'" -f $oldValue)
    }
}

function VeloTransfer.PrepareEnvironment
{
    if ($NULL -eq (Get-Command "dotnet-ef" -ErrorAction SilentlyContinue)) 
    {
        dotnet tool install --global dotnet-ef
    }

    if ($NULL -eq (Get-InstalledModule -Name SqlServer -ErrorAction SilentlyContinue)) 
    {
        Install-Module -Name SqlServer -Force
    }
}


$overrideValues = $True
VeloTransfer.PrepareEnvironment
VeloTransfer.SetEnvVariable -VariableName VeloTransfer.DbConfiguration.ConnectionStringHistory  -Value "Server=127.0.0.1;Database=Migration_VeloTransferHistory;User Id=VeloTransfer;Password=DpTo7mNHVtTYg;" -OverrideValues $overrideValues
VeloTransfer.SetEnvVariable -VariableName VeloTransfer.DbConfiguration.ConnectionStringData     -Value "Server=127.0.0.1;Database=Migration_VeloTransferData;User Id=VeloTransfer;Password=DpTo7mNHVtTYg;"     -OverrideValues $overrideValues
VeloTransfer.SetEnvVariable -VariableName VeloTransfer.ConnectionStringData                     -Value "Server=127.0.0.1;Database=VeloTransferData;User Id=VeloTransfer;Password=DpTo7mNHVtTYg;;"               -OverrideValues $overrideValues
VeloTransfer.SetEnvVariable -VariableName VeloTransfer.ConnectionStringHistory                  -Value "Server=127.0.0.1;Database=VeloTransferHistory;User Id=VeloTransfer;Password=DpTo7mNHVtTYg;;"            -OverrideValues $overrideValues



