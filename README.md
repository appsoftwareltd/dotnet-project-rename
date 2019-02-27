# Visual Studio Project Rename

## Visual Studio project renaming (within files, file names and directory names)

This console application is designed for finding and replacing text throughout Visual Studio projects, including inside project and data files, along with file and folder names, including solution files. By recursively renaming at the root of the project, you can avoid breaking links to project files and references to other projects in the solution. 

You may want to do this if you are reusing an existing project as a base for a new project, but need to replace old names.

If you have a project with the name `Foo.Bar`, which is used in namespaces, project names and file and folder names, and you want to change it to `My.New.Project`, then run the program and enter the path, the find text (Foo.Bar) and the replace text (My.New.Project) when prompted.


![Example](https://i.imgur.com/ihBY1aL.png)

You can also run from the command line by passing arguments instead:

- -p : Directory path
- -f : Find text
- -r : Replace text

Run with --help for help text.

All instances of the find text will be replaced, giving you a completely renamed project throught the file structure.

An attempt is made to avoid corrupting files managed by version control and visual studio (so `.git`, `.svn` and `.vs` directories are ignored). This list is not exhaustive though so ideally you will have the solution code in a directory other than where these files sit.
