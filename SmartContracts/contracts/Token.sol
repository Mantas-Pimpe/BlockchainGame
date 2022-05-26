// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import "@openzeppelin/contracts/token/ERC721/ERC721.sol";

contract Token is ERC721 {
    constructor() ERC721("token", "TOKEN") {}
    mapping(uint256 => Attr) public attributes;

    struct Attr {
        string name;
        string uri;
    }

    uint private _tokenId = 0;

    event tokenCreated(
        uint tokenId,
        address owner,
        string name,
        string uri
    );

    function createToken(string memory _name, string memory _uri) external returns (uint) {
        _mint(msg.sender, _tokenId);
        attributes[_tokenId] = Attr(_name, _uri);

        emit tokenCreated(_tokenId, msg.sender, _name, _uri);
        _tokenId++;
        return _tokenId-1;
    }

    function getTokenAttributes(uint256 tokenId) public view returns (string memory name, string memory uri) {
        name = attributes[tokenId].name;
        uri = attributes[tokenId].uri;
    }    

}