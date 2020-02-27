# NATS is a basic plain text file scanner.
>  **Usage:** NATS -P path -K keyword [Search Type] [options]


## License Information

- NATS  - [MIT](https://github.com/liukonen/nats/blob/master/LICENSE), Copyright 2020 [Luke Liukonen](https://github.com/liukonen)
- [System.Data.OLEDB](https://github.com/dotnet/corefx) - [MIT](https://licenses.nuget.org/MIT), Copyright Microsoft Corp - For use in Windows Index Scan
- [System.Data.Sqllite.Core](https://system.data.sqlite.org/index.html/doc/trunk/www/index.wiki) [-Public Domain](https://www.sqlite.org/copyright.html), D. Richard Hipp - Used for NATS Index Backend Database 
- [Farmhash.Sharp](https://nickbabcock.github.io/Farmhash.Sharp/) - [MIT](https://github.com/nickbabcock/Farmhash.Sharp/blob/master/LICENSE.txt),  Copyright (c) 2015 Nick Babcock - Copyright (c) 2014 Google, Inc. For use in generating hash values for Indexes

 ## Required Fields
 -K [keyword]    Keyword text to search for
 -P [path]       Dirrectory path to search through
                

##  Search Types

| Param | Meaning |
|-------|---------|
| -T[X] | **Threading:** Not setting the value indicates a single thread search. -T Implements Multithreading Scanning, with X being the number of Threads. If not set, single thread is used. If  [X] is empty, it will be set to 4 threads.|
| -I    | **Index:** Will generate or refresh NATS Sqlite DB based on the directory given. It will then search said index for any matching keywords in files, and scan that file for a match to  the phrase supplied.|
| -L    | **Limited:** Performs the same search as Index, but will not generate or refresh its index.|
| -B    | **Build:** Indexes on a directory only. A keyword needs to be passed but is not used in the action of indexing. Indexing also defaults to a hard coded set of file types that are whitelisted, blacklisted, and used "Smart Search" in order to scan the files. There is a chance Indexing will not always pick up on text files.|
| -W    | **Windows Index:** uses Windows build in Indexing engine to find files. Indexing must be turned on, and set in the folder you want to search through in order for this to work.|

## Options
| Param    | Meaning |
|----------|---------|
| -S       | **Smart Search:** uses the byte order mark at the start of the file to assist in filtering out non text files. This process is not 100% accurate, however is set for index based search.|
| -R       | **RAM:** Loads files under 10MB into Ram for comparison. This in theory should reduce IO spikes on the System process on non indexed searches. Does Not work the the -L command.|
| -O[path] | **Output:** Will output the command to the specified path file|
| -D [ext] | **Disapproved Extensions:** Implements filtering of file extensions not to scan. Multiple extensions will be deliminated with the | symbol. NAPS has a default blacklist of [7z|bmp|db|db-journal|dll|doc|docx|exe|jpg|m4v|mov|mp3|mp4|pdb|pdf|png|tmp|xls|xlsx|zip] and invoking this will override the default|
| -A [ext] | **Approved Extensions:** The reverse of the disapproved extension, which is disabled if -A is sent. Will only search files with the particular extension.|
| -M       | **MultiLine:** By default NATS will only indicate if the file contains the selected keyword. Adding the M keyword on Single or Multi thread scan will indicate What lines in the file the keyword appears as well (Note: this search will take longer as it needs to read through the entire file for keywords)|
| -H       | **Help:** Ignores all other Inputs and displays this help file|


## Other
|Design patterns or technologies explored|  
|--|
| - Singleton  |
| - Factory Pattern|
| - Parallel Library|
| - Sqlite db |
| - String Hashing |
