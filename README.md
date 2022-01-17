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
* .NET Framework 4.7.2 or later
* Windows 7 and up
* Admin privileges (needed to check TPM status and apply changes to startup items)

## Known issues
* When enabling/disabling using the context menu, nothing happens when you re-enable/disable using the context menu again.
   * **Workaround:** The entire Startup Items list will refresh when you enable/disable using the context menu.
   * Fix status: Unable to fix due to how WPF manages context menu events.
