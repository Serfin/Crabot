# TODO

[DONE] - Improve logging on critical parts, closing long running tasks 
[DONE] - Add external logging
[DONE] - Resuming session


https://crabot.scm.azurewebsites.net/api/vfs/SystemDrive/local/Temp/jobs/continuous/deployedJob/

[] - Add parameters handling / maybe with attributes? [CommandAttributes("t", "d")]
[] - Adding commands with attributes [Command("ping")]



```


	-> Register Commands marked with attribute

	[Command("ping")]
	PingCommand

	-> Register CommandHandler for specific command

	PingCommandHandler : ICommandHandler<PingCommand>
	fn async Handle()

	-> After received and validated message pass it to proper handler

```