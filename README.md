# WordFinderApp

## Overview

**WordFinderApp** efficiently searches for words in a 64x64 character matrix using a large word stream. Words can be found horizontally or vertically, returning the top 10 most frequent ones.

## Key Features

- **Optimized performance** using parallel processing and efficient resource management.
- **Trie data structure** for fast word lookup.
- **ConcurrentDictionary** for safe, concurrent word counting.
- **Async processing** with cancellation support.

## How to Run

1. Download the appropriate executable from the `AppBundles` folder:
   - **Windows**: `WordFinderApp.exe`
   - **Linux**: `./WordFinderApp_linux`
   - **macOS**: `./WordFinderApp_macos`

2. Choose input method:
   - **Default data**.
   - **Load from files**: `matrix.txt` and `wordstream.txt`.
   - **Interactive input**.

## Performance

The application handles large word streams with high performance using parallelism, concurrency, and efficient algorithms.
