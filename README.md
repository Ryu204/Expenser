# Expenser

Expenser is a simple command-line tool that helps you keep track of your balance fluctuations over time. The goal of this program is to make it easy to record income and expenses, and see how your balance changes over days, weeks, or months.

# Features
* Record income and expenses with custom descriptions.
* View a list of all recorded income and expenses.
* See a table of your whole balance history or a specific wallet over time.
* Simple command-line interface.

# License
Expenser is released under the Apache 2.0 License. See LICENSE for details.

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