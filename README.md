# KnowMySystem
A tool showing information about your system's hardware and software in one place. Useful for both end users and tech support who are trying to get more information about their/their customers' systems.

If you are a company and want to use this for your tech support you are welcome to as long as you follow the GNU GPL-3 licence terms.

Previously known as KnowYourSystem. Rebranded because a repo called "KnowYourSystem" already exists.

## Installation
Installation is as simple as downloading KnowMySystem.exe and running it.

## Features
* Hardware Specifications
  * CPU
  * GPU
  * RAM
  * Motherboard
  * Storage on Windows drive
  * CPU Architecture
  * BIOS Mode
  * Secure Boot
  * TPM
 * Operating System information
   * OS name
   * OS edition
   * OS minor version (Windows 10 and up only)
   * OS build
   * OS branch
   * Insider status
   * Insider branch
 * Startup Items
   * Enable/disable startup items (Windows 8 and up only)
   * Open startup file location
   * Open startup (Registry) entry

## Requirements
* .NET Framework 4.8 or later
* Windows 7 and up
* Admin privileges (needed to check TPM status and apply changes to startup items)

## Known issues
* When enabling/disabling using the context menu, nothing happens when you re-enable/disable using the context menu again.
   * **Workaround:** The entire Startup Items list will refresh when you enable/disable using the context menu.
   * Fix status: Unable to fix due to how WPF manages context menu events.

![image](https://user-images.githubusercontent.com/63195743/157604163-ebfe2eae-3f85-4922-bb17-1d3d137a38ec.png)
![image](https://user-images.githubusercontent.com/63195743/157605051-43ff17d5-e543-44a4-83da-ea8224ded011.png)
![image](https://user-images.githubusercontent.com/63195743/157604229-76ae62f3-655a-4a73-adba-0b20fa3bc9a1.png)
![image](https://user-images.githubusercontent.com/63195743/157604249-d61d04b8-ebd2-4d15-be3e-842605d7587f.png)
![image](https://user-images.githubusercontent.com/63195743/157604267-671f60a8-20c6-4f14-b504-ee318c1dc612.png)
![image](https://user-images.githubusercontent.com/63195743/157604915-92db0cd4-92a5-49d1-a42b-dc410dab59d0.png)
