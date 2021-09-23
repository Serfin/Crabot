# Attributes

[Command("blackjack", 1)] - marks command to be available for bot to find 
	- eg. user now can use ?blackjakc command with exactyl 1 argument, under 
	  the attribute there is a basic argument count validation

[CommandUsage("?blackjack <bet-amount>")] - marks command as one for which user can find help 
	- eg. ?help blackjack - returns ?blackjack <bet-amount>

Each command must implement ICommandHandler which under the hood executes ValidateCommandAsync()

There are internal commands whether there's an error with processing command or bot cannot find command with specified command name