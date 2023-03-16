# Overview

This Unity plugin makes it straightforward to integrate with the NEAR blockchain. The following functionality is supported:

- Wallet Sign In
- Get account details
- Contract method calls 

## Limitations

At this time, only WebGL builds are supported.
In the Unity editor calls are 'faked', deploy to WebGL to see the action!

## Installation / Build Example

Build Setting -> Platform = WebGL

Player Settings -> Player -> Resolution and Presentation -> WebGL Template = NEARWS

Build

## Details

Both the more minimal near-api-js and the richer wallet-selector apis are supported. The NFTScene and ContractScene use the near-api-js api and the WSScene uses the wallet-selector api. The advantage of the wallet-selector api is that it gives a choice of multiple different wallets. 2 html templates are offered, NEARJS only supports the near-api-js but NEARWS supports both. If in doubt use NEARWS but NEARJS is more minimal and will be faster to load if you don't need the multiple wallet support.

## Links
https://docs.near.org/tools/near-api-js/quick-reference

## Support

https://discord.gg/rogues



## License

See the [LICENSE](LICENSE.md) file for license rights and limitations (MIT).