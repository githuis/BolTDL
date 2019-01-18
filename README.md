# BolTDL
Bolhuis' ToDo Lists: A tiny organization program with vim-like keybinds.
Tested working on Windows 10 and Archlinux (x64).


![](boltdl.gif)

## Build
There are two ways to build. The first way is to open the solution with Visual Studio or something alike and build in release mode from there.

The other is with xbuild:
```bash
git clone https://github.com/githuis/BolTDL.git
cd BolTDL/BolTDL
xbuild
```
## Instructions

Lower case input / don't hold shift, as the application doesn't seem to like that.

| Key 	| Action                   	| Extra                                              	|
|-----	|--------------------------	|----------------------------------------------------	|
| O   	| Create a new task        	|                                                    	|
| J   	| Select next task         	|                                                    	|
| K   	| Select previous task     	|                                                    	|
| DD  	| Delete selected task     	| Press D twice                                      	|
| DT 	| Delete current list       | Press D then T                                        |
| L   	| View task                	|                                                    	|
| C   	| Change task               | Rewrite a task, same as 'DDO'                     	|
| H   	| Exit (when viewing task) 	|                                                    	|
| N   	| Create a new tab      	|                                                    	|
| W   	| Save list to webserver.     	|  Requires settings set. Run program once and check json file. |
| T   	| Select next tab       	|                                                    	|
| R   	| Rename current tab     	|                                                    	|
| Q   	| Quit                     	| Can also be used in place of H when viewing a task 	|


## Authors notes
BolTDL uses the MIT licence, so feel free to use this software for whatever, as long as it complies with it.

Bitcoin, if you like it/manage to sell a version of it, somehow.
```bash
1GAN5GpfmhBzih5pUSvJgeXd3RzF7hD9CS
```
