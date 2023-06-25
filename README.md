# NATS - Plain Text File Scanner

NATS is a basic plain text file scanner tool that allows you to search for keywords within text files. It provides various search options and can be used in both command-line and GUI versions available for Windows, Mac, and Linux.

## License Information

- NATS: [MIT License](https://github.com/liukonen/nats/blob/master/LICENSE), © 2020 [Luke Liukonen](https://github.com/liukonen)
- System.Data.OLEDB: [MIT License](https://licenses.nuget.org/MIT), © Microsoft Corp - Used for Windows Index Scan
- System.Data.Sqllite.Core: Public Domain, © D. Richard Hipp - Used for NATS Index Backend Database
- Farmhash.Sharp: [MIT License](https://github.com/nickbabcock/Farmhash.Sharp/blob/master/LICENSE.txt), © 2015 Nick Babcock and © 2014 Google, Inc. - Used for generating hash values for Indexes
- Material.IO Apache License: [Apache License 2.0](https://www.apache.org/licenses/LICENSE-2.0.html) - Icon for Mac and Windows provided by material.io

## Usage (command line)
```bash
NATS -P path -K keyword [Search Type] [options]
```

## Required Fields

- `-K [keyword]`: Keyword text to search for
- `-P [path]`: Directory path to search through

## Search Types

| Param | Meaning |
|-------|---------|
| -T[X] | **Threading:** Not setting the value indicates a single-thread search. -T implements multithreaded scanning, where X is the number of threads. If not set, a single thread is used. If [X] is empty, it defaults to 4 threads. |
| -I    | **Index:** Generates or refreshes a NATS SQLite DB based on the given directory. It searches the index for matching keywords in files and scans those files for matching phrases. |
| -L    | **Limited:** Performs the same search as Index but does not generate or refresh the index. |
| -B    | **Build:** Indexes a directory only. A keyword needs to be passed but is not used in the indexing process. The indexing defaults to a hardcoded set of file types that are whitelisted, blacklisted, and utilize "Smart Search" to scan the files. Note: There is a chance that indexing may not always pick up text files. |
| -W    | **Windows Index:** Uses the Windows built-in indexing engine to find files. Indexing must be turned on and set in the folder you want to search through for this option to work. |

## Options

| Param    | Meaning |
|----------|---------|
| -S       | **Smart Search:** Uses the byte order mark at the start of the file to assist in filtering out non-text files. This process is not 100% accurate but is used for index-based searches. |
| -R       | **RAM:** Loads files under 10MB into RAM for comparison. This theoretically reduces IO spikes on the system process during non-indexed searches. Does not work with the -L command. |
| -O[path] | **Output:** Outputs the command to the specified path file. |
| -D [ext] | **Disapproved Extensions:** Filters out file extensions not to scan. Multiple extensions should be delimited with the "|" symbol. NATS has a default blacklist of [7z, bmp, db, db-journal, dll, doc, docx, exe, jpg, m4v, mov, mp3, mp4, pdb, pdf, png, tmp, xls, xlsx, zip], and using this option will override the defaults. |
| -A [ext] | **Approved Extensions:** The reverse of the disapproved extension, which is disabled if -A is used. Only searches files with the specified extension. |
| -M       | **MultiLine:** By default, NATS indicates if the file contains the selected keyword. Adding the M keyword during single or multithreaded scan will also indicate the lines in the file where the keyword appears (Note: this search takes longer as it needs to read through the entire file for keywords). |
| -H       | **Help:** Ignores all other inputs and displays the help file. |

## Other

Design patterns or technologies explored:
- Singleton
- Factory Pattern
- Parallel Library
- SQLite DB
- String Hashing
- Cocoa (Mac OS) Development
- XCode
- GTK#
- .NET Standard
- Cross-Platform Development

