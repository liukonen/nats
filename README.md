# nats
Not Another Text Scanner

 NATS is a basic plain text file scanner.
 - Usage: NATS -P <path> -K <keyword> [options]

Keyword Options
 Keywords can be any group of words, including spaces. The | character will be
 used as a seperator of keywords unless the -| option is used

 Paths Options
 You can also pass in more then one path location to scan by using the | 
 seperator. This is NOT disabled with the -| option

| Option| Desc|
| ------------- |:-------------:|
| -K [keyword]<req> |Keyword text to search for|
| -P [path] <req>   ||
| -S                |Scans all Subdirectories of the Paths Table, as well as its root|
|  -T[X]            | (Search Type) Only -T or -I can be used at a time. Not setting the value indicates a single thread search. -T Implements Multithreading Scanning, with X being the number of Threads. If not set, single thread is used. If [X] is empty, it will be set to 4 threads.|
| -I                |(Search Type) Only -T or -I can be used at a time. Not setting the valueindicates a single thread search. Type I uses Windows build in Indexing|to find files. Indexing must be turned on in order for this to work.|
| -M                |Implements "Smart Search" which uses the byte order mark at the start of the file to assist in filtering out non text files. This process is not 100% accurate.|
| -R                |Loads files under 10MB into Ram for Comparision. This was implemented because I noticed NVME drives would spike some machines, which appeared to be related to the ReadLines. Does Not work the the -L command yet.|
| -O[path]          |Will output the command to the specified path file|
| -B [ext]          |Implements a Blacklist of file extentions not to scan. extentions will be deliminated with the | symbol. NAPS has a default blacklist of [7z,bmp,doc,docx,jpg,m4v,mov,mp3,mp4,pdf,png,tmp,xls,xlsx] and implementing this will override the default|
| -W [ext]          |The reverse of the Blacklist extention. Blacklist is disabled if Whitelist is filled.This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.|
| -L                |By default NATS will only indicate if the file contains the selected keyword. Adding the L keyword will indicate What lines in the file the keyword appears as well (Note: this search will take longer as it needsto read through the entire file for keywords)                            |
|-H                 |Ignores all other Inputs and displays this help file                     |   
