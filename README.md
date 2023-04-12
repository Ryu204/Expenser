# Expenser

Expenser is a simple command-line tool that helps you keep track of your balance fluctuations over time. The goal of this program is to make it easy to record income and expenses, and see how your balance changes over days, weeks, or months.

# Features
* Record income and expenses with custom descriptions.
* View a list of all recorded income and expenses.
* See a table of your whole balance history or a specific wallet over time.
* Simple command-line interface.
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
``` sub <amount of money> ``` | Take away some money from spare budget
``` delete <wallet name>```| Permanently delete a wallet
``` log ``` | Show your expenses stats
``` log <wallet name>``` | Show stats of a specific wallet
``` log <date1> <date2>``` | Show only stats of the time between these dates (dd/mm/yy)

## Flag

Flag are used when you want to review your expense more carefully. Available flags:

Flag | Usage
-----|-----
```--short``` | More concise output
```--day```| Limit output in today
```--week```|Limit output in the last week
```--month```|Limit output in the last month
```--year```|Limit output in the last year

**Note:** You wont be able to call this type of command:
```
log <wallet Name> --month
```

# Install
There are a few ways to install the program.
## 1. Download executable

You can be lazy and grab the ```PreBuiltRelease``` folder. 
## 2. Use the batch file

### Requirements
```
- git
- dotnet 6.0 or higher
- Windows 10 or higher
```
1. Download ```install.bat```.
2. Place it inside the folder you want to install the project.
3. Run the batch script. The folder you need is	```Expenser\Release```.

**Note:** The folder <u>**must**</u> only consist of ```install.bat```.

## 3. Build the project

Download the latest version of ```-master``` branch and build with **VSCode** or **Visual Studio**.
In the first run there will be a notice looking like this:
```
Unable to load users list. Recovering from local folder...
0 account(s) were added to new users list.
```
It means the data folder ```bin\Release\net6.0\Users``` has not been created yet, and will fix that. Just ignore the message and we are good to go.

# License
Expenser is released under the Apache 2.0 License. See LICENSE for details.