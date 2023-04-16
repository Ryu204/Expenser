# User manual
 
 ## Basis

The structure of a command is:
 ```
 <action> <parameter1> <parameter2> ... <flag1> <flag2> ...
 ```
* ```<action>``` is a string with only letter and case-insensitive 
* ```<flag>``` is in the form ```--flagName``` 

* ```<parameter>``` is any data passed by user 
 
 * Inappropriate ```<flag>``` will be ignored
 
 * Unsuitable ```<parameter>``` will break the command

 ## Command

The basic interface consists of following commands:

Command | Usage
-------|-------
``` signup <username> ``` | Sign up for first time use
``` delete <username> ``` | Permanently delete your account
``` login <username> ``` | Log into your account
``` new <wallet name> ``` | Create a new wallet
``` add <amount of money> <wallet name> ``` | Add some money to a specific wallet
``` add <amount of money> ``` | Add some money to spare budget
``` sub <amount of money> <wallet name> ``` | Take away some money from a specific wallet
``` sub <amount of money>``` | Take away some money from spare budget
``` delete <wallet name>```| Permanently delete a wallet
``` log ``` | Show your expenses stats
``` log <wallet name>``` | Show stats of a specific wallet
``` log <date1> <date2>``` | Show only stats of the time between these dates (dd/mm/yy)

## Flag

Flag are most used when you want to review your expense more carefully. Available flags:

Flag | Usage
-----|-----
```--short``` | More concise output
```--day```| Limit output in today
```--week```|Limit output in the last week
```--month```|Limit output in the last month
```--year```|Limit output in the last year

Moreover, if you want to add an amount of money from spare money (the ```_Other``` wallet) to another wallet, use the flag ```--transfer```. For example:

```
add 1000 Eating --transfer
// take 1000 spare money and add to "Eating"
subtract 2000 Dancingclass --transfer
// give up 2000 money from Dancingclass to spare money
```

**Note:** You wont be able to call this type of command:
```
log <wallet Name> --month
```