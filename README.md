
# Customer Preference Centre

This is a command line tool which takes the list of customer preferences via
stdin, and the start date and duration as parameters.

The customer preferences input file should consist of lines with the customer
name and frequency separated by a semi-colon (`;`). For an example, see
[example-input.txt](CustomerPreferencesApp/example-input.txt)

## How to run

After cloning this repo and changing into the directory, run the following:

```
cd CustomerPreferencesApp
cat example-input.txt | dotnet run Program.fs 2018-04-01 90
```

To test it, run the following:

```
cd CustomerPreferences.Test
dotnet test
```

These have been tested on a Linux machine running the Zsh shell. It *should*
also work on a Mac but may need to change slightly for Windows.
