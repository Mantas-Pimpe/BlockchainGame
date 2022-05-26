// SPDX-License-Identifier: MIT
pragma solidity >=0.4.22 <0.9.0;

import "@openzeppelin/contracts/token/ERC721/IERC721.sol";

contract Market {

    enum ListingStatus{
        Active,
        Sold,
        Cancelled
    }

    struct Listing {
        ListingStatus status;
        address seller;
        address token;
        uint tokenId;
        uint price; // 1 ETH = 1 * 10^18
    }

    event tokenListed(
        uint listingId,
        address seller,
        address token,
        uint tokenId,
        uint price
    );

    event tokenSold(
        uint listingId,
        address seller,
        address buyer,
        address token,
        uint tokenId,
        uint price
    );

    event tokenListingCancelled(
        uint listingId,
        address seller,
        address token,
        uint tokenId,
        uint price
    );

    uint private _listingId = 0; //listing counter
    mapping(uint => Listing) private _listings;

    function getAllListings() public view returns (Listing[] memory) {
        Listing[] memory listings = new Listing[](_listingId+1);
        for(uint i=0; i < _listingId; i++){
            listings[i] = getListing(i);
        }
        return listings;
    }

    function getListing(uint listingId) public view returns (Listing memory) {
        return _listings[listingId];
    }

    function createTokenListing(address token, uint tokenId, uint price) external {
        IERC721(token).transferFrom(msg.sender, address(this), tokenId);

        Listing memory listing = Listing(ListingStatus.Active, msg.sender, token, tokenId, price);

        _listings[_listingId] = listing;

        emit tokenListed(_listingId, msg.sender, token, tokenId, price);
        _listingId++;
    }

    function buyToken(uint listingId) external payable { 
        Listing storage listing = _listings[listingId];

        require(listing.status == ListingStatus.Active, "Listing not active");
        require(listing.seller != msg.sender, "Lister cannot be the buyer");
        require(listing.price <= msg.value, "Insufficient payment");

        listing.status = ListingStatus.Sold;

        IERC721(listing.token).transferFrom(address(this), msg.sender, listing.tokenId);
        payable(listing.seller).transfer(listing.price);

        emit tokenSold(listingId, listing.seller , msg.sender, listing.token, listing.tokenId, listing.price);
    }

    function cancelTokenListing(uint listingId) public {
        Listing storage listing = _listings[listingId];

        require(listing.status == ListingStatus.Active, "Listing not active");
        require(listing.seller == msg.sender, "Only lister can cancel listing");

        listing.status = ListingStatus.Cancelled;
        IERC721(listing.token).transferFrom(address(this), msg.sender, listing.tokenId);

        emit tokenListingCancelled(listingId, msg.sender, listing.token, listing.tokenId, listing.price);
    }
}