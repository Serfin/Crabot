$Crabot_process = Get-Process -Name Crabot

if ($Crabot_process -ne $null && $Crabot_process.Id -ne $null) {
	Write-Output "Stopping process 'Crabot' with PID"
	Write-Output $Crabot_process.Id
	Stop-Process $Crabot_process.Id
}