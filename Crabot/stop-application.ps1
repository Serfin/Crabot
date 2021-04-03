$Crabot_process = Get-Process -Name Crabot

if ($Crabot_process.Id -ne null) {
	Stop-Process $Crabot_process.Id
}