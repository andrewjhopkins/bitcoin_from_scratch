# Bitcoin From Scratch
My attempt at a simplified C# implementation of the original Bitcoin protocol.

In progress.

### Wallets
- [x] Generate valid Bitcoin addresses
- [x] Save generated address keys to file
- [x] Ability to upload wallet from file

### Blockchain
- [x] Proof of Work
- [x] Mine genesis block
- [ ] Persist blockchain

## Building the Project
- Build the source code. From the directory containing the .csproj
```
dotnet build
```
The .exe will be contained in the bin folder. By default under bin/Debug/net6.0.
## Generate a Bitcoin Address
- Run the command create-wallet on the exe
```
$ ./bitcoin_from_scratch.exe create-wallet
Bitcoin address: 13HvVsfdTPGqWZiD3u67VcYohyYPzpP68k
Saved to file: /wallets/13HvVsfdTPGqWZiD3u67VcYohyYPzpP68k.dat
```
Check it out on https://www.blockchain.com/explorer
