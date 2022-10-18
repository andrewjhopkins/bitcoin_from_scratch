# Bitcoin From Scratch
My attempt at a simplified C# implementation of the Bitcoin protocol.
No p2p network, and not completely true to the original implementation.
Definitley not bug free or production ready.

## Building the Project
Build the source code. From the directory containing the .csproj
```
dotnet build
```
The .exe will be contained in the bin folder. By default under bin/Debug/net6.0.

Use --help to see which commands are available. 
## Generate a Bitcoin Address
Run the command create-wallet on the .exe
```
$ ./bitcoin_from_scratch.exe create-wallet
Bitcoin address: 1PUPzb4QnMPib93V9YHfQtEmu586nLcUGq
Saved to file: /wallets/1PUPzb4QnMPib93V9YHfQtEmu586nLcUGq.dat
```
Check it out on https://www.blockchain.com/explorer

## Create a Blockchain
Run the command create-blockchain <address>
The address parameter is the bitcoin address which recieves the genesis block mining reward
```
$ ./bitcoin_from_scratch.exe create-blockchain 1PUPzb4QnMPib93V9YHfQtEmu586nLcUGq
Blockchain does not exist
Mining genesis block...
2e98077ac50a0000e5d1ab13a08e762babb97b086c2352407c556be6a99da8e4233449
Blockchain created!
```
## Get Balance
Run the command get-balance <address>
```
$ ./bitcoin_from_scratch.exe get-balance 1PUPzb4QnMPib93V9YHfQtEmu586nLcUGq
Balance: 10
```

## Send Bitcoin
Run the command send <from address> <to address> <amount> 
Include -r flag in the command to give the from address a mining reward!
```
$ ./bitcoin_from_scratch.exe send 1PUPzb4QnMPib93V9YHfQtEmu586nLcUGq 19zJdGoTN6iJsywZXABcWdfbfrCCSJuVa7 3
2e08cfb994320000fcff47b13661af693a7b0bc83089a65765646428d9112b0c254177
3 bitcoin sent from address: 1PUPzb4QnMPib93V9YHfQtEmu586nLcUGq to address: 19zJdGoTN6iJsywZXABcWdfbfrCCSJuVa7
```
