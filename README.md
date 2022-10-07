# Bitcoin From Scratch
My attempt at a C# implementation of the original Bitcoin protocol.

### Wallets
- [x] Generate valid Bitcoin addresses
- [ ] Save generated address keys to file
- [ ] Ability to upload wallet from file

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
bitcoin address: 18EKiVTeMe8iiEuE6z22nHdiGpRTMhJx3Z
```
Check it out on https://www.blockchain.com/explorer