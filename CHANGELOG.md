## 0.1.0 

### Current known bugs:
* **(B4)** whole selected line is colord not just the text
* **(B5)** IndexError in half the code

##### (2022-11-14)

###### New code
* worked on Add_data method

###### Bugs
* **(B5)** Identified: options later in the line couse an IndexError. Thats becouse I passed line as an index where it should be a line - (Dispalyable)father.line 

##### (2022-11-11)

* wrote half of the add or modify function

##### (2022-11-07)

###### New features
* reintroduced the colors dictionary for easier use of colors

##### (2022.11.06)

###### Bugs
* **(B3)** Fixed: pause every line
* **(B1)** Updated: Identified the cause of the bug: last entry is not acessable
* **(B4)** Identified: selected line colors the whole line not just the word as it's supposed to


##### (2022.11.05)

###### New features
* ConsolColorExtension is now Displayable
* Started to set up the new Main menu
* Deleted old code
* Made types configurable runtime (so you can break the program)

###### Known bugs
* **(B3)** pauses program every line (intended should be fixed in next version)
* **(B2)** console not cleared
* **(B1)** last entry is not accessable

##### (2022.11.03)

#### New features
* uploaded the whole project to github repo
* Updated the Classes class
* Added CHANGELOG.md
* Moved the TODO list from Program.cs the end of the CHANGELOG.md

## Todo list for myself
**In progress**
* Add_data pass father and inxed as parameters (so i can insert above or below)
* make data types configurable runtime instead of compile time


**Priority 1:**
* second "display" on the right side of the screen
* create a way to add data
    * list every class key first
    * **Prioritiy 2:** list every not null class key in the first group
                  then list every null class key in the bottom
* create a way to veiw data
    * Alphabetically
    * Displayable bool in_line_open
* create a way to save and load data
    * save when modiffied
    * load at start
    * unload data

**Priority 2:**
* secundary sorting options for data
* generate questions automaticly
* Adding colors and menus
* make the connections of a graph be the data_logic
* same data different weight for data_logic for different subjects

**Priority 3:**
* create customisable controlls
* rewrite the whole thing with switch statements instead of if statements
* make the program unbreakable again but withouth losing functionality
* always on top, limit time you can use your PC
* sounds for the controlls
* multiple languange packs
* way to update the project after release
* server for peaple to upload and download projects
* classes class: fileds can only take: (string, int, displayable as classes x or list&lt;T&gt; where T is any of the above) as parameters.
* check that consolecolor.reset() is used everywhere instead of CCE.colors["default"]