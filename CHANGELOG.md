## 0.1.0 

##### (2022.11.06)

###### Bugs
* (B3) Fixed: pause every line
* (B1) Updated: Identified the cause of the bug: last entry is not acessable
* (B4) Identified: selected line colors the whole line not just the word as it's supposed to


##### (2022.11.05)

###### New features
* ConsolColorExtension is now Displayable
* Started to set up the new Main menu
* Deleted old code
* Made types configurable runtime (so you can break the program)

###### Known bugs
* (B3) pauses program every line (intended should be fixed in next version)
* (B2) console not cleared
* (B1) last entry is not accessable

##### (2022.11.03)

#### New features
* uploaded the whole project to github repo
* Updated the Classes class
* Added CHANGELOG.md
* Moved the TODO list from Program.cs the end of the CHANGELOG.md

## Todo list for myself
**Priority 1:**
* make data types configurable runtime instead of compile time
* create a way to add data
    * list every class key first
    * **Prioritiy 2:** list every not null class key in the first group
                  then list every null class key in the bottom
* create a way to veiw data
    * Alphabetically
    * on the right as a secund "display"
* create a way to save and load data
    * save when modiffied
    * load at start
    * unload data

**Priority 2:**
* generate questions automaticly
* same data different weight for data_logic for different subjects

**Priority 3:**
* always on top, limit time you can use your PC
* sounds for the controlls
* multiple languange packs
* way to update the project after release
* server for peaple to upload and download projects
* classes class: fileds can only take: (string, int, displayable as classes x or list&lt;T&gt; where T is any of the above) as parameters.