# Universal Profile Avatar SDK Third Person Example

An example Unity project using the Universal Profile Avatar SDK built on top of the Unity Third Person Starter Assets example.

Loads an Universal Profile and it's avatars, then instantiates them and allows you to run around an example scene.

![Unity_2022-08-07_16-10-22](https://user-images.githubusercontent.com/16716633/183292260-b8f01554-df80-4ad1-a2a7-1dc70df8cfa3.png)

The project was created in Unity **2020.3.30f1**, other versions were not tested.

## Getting Started
To get started clone the repo, load the Unity project and open the **SampleScene** located in the `Scenes` folder.

You'll be presented with a scene and a player character. Press `Esc` to open the Universal Profile menu.
In the address field, enter a Universal Profile address or a local path to a [LSP3Profile JSON](https://github.com/lukso-network/LIPs/blob/main/LSPs/LSP-3-UniversalProfile-Metadata.md#lsp3profile) then load it.

Once loaded you'll see a list of available avatars in the dropdown box at the bottom of the menu.
Once an avatar is selected it will begin loading and the loading percentage will be displayed above the character's head.

When the profile loads, the first avatar in the list is selected by default and will start loading automatically.
Currently the avatar list is not filtered by platform, unlike the simple example, so avatars not made for the current platform will likely load with their shaders broken, looking all pink.

## Avatars in the LSP3

An avatar entry in the `Avatars` field of the LSP3 json should look like this, with it's `fileType` field currently being responsible for the platform filtering feature. (Not included here, but included in the [simple example](https://github.com/lukso-network/universalprofile-unity-avatar-sdk-example)).
```json
{
	"hashFunction": "keccak256(bytes)",
	"hash": "0x6b9d7c98a455236845ef44e3f0120ff1d89301753b049d9230cd1f48869a708a",
	"url": "ipfs://QmRgpQnk82SMeTudCmunskDE98t81QiejvqEofNE48vSXc",
	"fileType": "assetbundle/windows"
}
```
