# Note App

This simple console application allows you to create and edit notes in your preferred text editor. It also automatically syncs your notes with a Git repository.

## Requirements

- [.NET Core 3.1](https://dotnet.microsoft.com/download) or later
- [Git](https://git-scm.com/downloads)
- SSH keys set up for Git (follow [these instructions](https://help.github.com/en/github/authenticating-to-github/generating-a-new-ssh-key-and-adding-it-to-the-ssh-agent) if needed)

## Installation

You can download the latest version of the program from the [Releases page](https://github.com/yourusername/noteapp/releases).

After downloading, you can optionally move the program to a directory in your PATH to make it easier to run from anywhere.

## Usage

Run the program by calling `notes` with the desired filename as the argument:

```bash
notes note.txt
```
If the note file does not exist, the program will create it. The program will then open the file in your default editor.

You can change the default editor by providing the --editor option followed by the command to launch your desired editor:

```
notes --editor "code"
```

The program will attempt to pull any changes from the Git repository before opening the file, and push any changes after closing the file.

If the origin remote or the main branch are not set in the Git repository, the program will prompt you to enter the remote repository's URL and will automatically set up the main branch for you.

## Supported Editors
The program supports any text editor that can be launched from the command line, including but not limited to:

nano
vim
nvim
notepad
gedit
Visual Studio Code (code)

The default editor is nvim.