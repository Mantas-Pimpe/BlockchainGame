//import * as ethers from 'ethers';

const express = require('express');
const { BigNumber, ethers, utils } = require('ethers');

const app = express();
const port = 3000;
const blockChainUrl = 'http://localhost:8545';
app.use(express.json());

const MNEMONIC = 'hold raw tape accuse embark wild tortoise length album coffee fantasy elbow'; //set from ganache-cli local blockchain server

const provider = new ethers.providers.JsonRpcProvider(blockChainUrl);

const wallet = ethers.Wallet.fromMnemonic(MNEMONIC).connect(provider);

const marketContractAddress = '0x57deB85632D7B32dCb35e74FBE251A3E7861778F'; //set from deployed market smartcontract
const marketJsonABI = require('./builtContracts/Market.json').abi;
const marketContract = new ethers.Contract(marketContractAddress, marketJsonABI, provider);
const marketWalletContract = new ethers.Contract(marketContractAddress, marketJsonABI, wallet);

const tokenContractAddress = '0x330A3B32ebDD716263CC2BF08c3dE8Db30f58Db1'; //set from deployed token smartcontract
const tokenJsonABI = require('./builtContracts/Token.json').abi;
const tokenContract = new ethers.Contract(tokenContractAddress, tokenJsonABI, provider);
const tokenWalletContract = new ethers.Contract(tokenContractAddress, tokenJsonABI, wallet);

//mock buyer address
const buyerWalletPrivateKey = '0xec6868c77c0076cd3118d811fcf5316c714a269aa78bd1e9f13809583104774b'; //set from ganache-cli local blockchain server one of private keys
const buyerWallet = new ethers.Wallet(buyerWalletPrivateKey, provider);
const buyerMarketContract = new ethers.Contract(marketContractAddress, marketJsonABI, buyerWallet);

//----------------------------------------------------------------
// Listings api

app.post('/listings', async (request, response) => {
	try {
		const approveResponse = await tokenWalletContract.approve(marketContractAddress, request.body.tokenId);
		const tokenListingResponse = await marketWalletContract.createTokenListing(
			tokenContractAddress,
			request.body.tokenId,
			request.body.price
		);
		const eventResponse = await tokenListingResponse.wait();
		const event = eventResponse.events.find((event) => event.event === 'tokenListed');
		result = {
			listingId: parseInt(event.args.listingId._hex, 16),
			seller: event.args.seller,
			token: event.args.token,
			tokenId: parseInt(event.args.tokenId._hex, 16),
			price: parseInt(event.args.price._hex, 16)
		};
		response.send(result);
	} catch (e) {
		response.send(e);
		console.log('ERROR: ' + e);
	}
});

app.post('/listings/cancel/:id', async (request, response) => {
	try {
		const cancelListingsResponse = await marketWalletContract.cancelTokenListing(request.params.id);
		const eventResponse = await cancelListingsResponse.wait();
		const event = eventResponse.events.find((event) => event.event === 'tokenListingCancelled');
		result = {
			listingId: parseInt(event.args.listingId._hex, 16),
			seller: event.args.seller,
			token: event.args.token,
			tokenId: parseInt(event.args.tokenId._hex, 16),
			price: parseInt(event.args.price._hex, 16)
		};
		response.send(result);
	} catch (e) {
		response.send(e);
		console.log('ERROR: ' + e);
	}
});

app.post('/listings/buy/:listingId', async (request, response) => {
	try {
		listing = await getListingById(request.params.listingId);
		const options = { value: listing.price };

		const buyTokenResponse = await buyerMarketContract.buyToken(request.params.listingId, options); //mock connection to contract to simulate diffrent address
		const eventResponse = await buyTokenResponse.wait();
		const event = eventResponse.events.find((event) => event.event === 'tokenSold');
		result = {
			listingId: parseInt(event.args.listingId._hex, 16),
			seller: event.args.seller,
			buyer: event.args.buyer,
			token: event.args.token,
			tokenId: parseInt(event.args.tokenId._hex, 16),
			price: parseInt(event.args.price._hex, 16)
		};
		response.send(result);
	} catch (e) {
		response.send(e);
		console.log('ERROR: ' + e);
	}
});

app.get('/listings', async (request, response) => {
	try {
		const listings = await marketContract.getAllListings();
		tokenListings = [];
		for (let i = 0; i < listings.length; i++) {
			listing = listings[i];
			token = await tokenContract.getTokenAttributes(BigNumber.from(listing.tokenId._hex));
			if (token.name !== '' && token.uri !== '') {
				tokenListing = {
					listingId: i,
					tokenId: parseInt(listing.tokenId._hex, 16),
					seller: listing.seller,
					token: listing.token,
					status: getListingStatus(listing.status),
					price: parseInt(listing.price._hex, 16),
					name: token.name,
					uri: token.uri
				};
				tokenListings.push(tokenListing);
			}
		}
		response.send(tokenListings);
	} catch (e) {
		response.send(e);
		console.log('ERROR: ' + e);
	}
});

app.get('/listings/:id', async (request, response) => {
	try {
		response.send(await getListingById(request.params.id));
	} catch (e) {
		response.send(e);
		console.log('ERROR: ' + e);
	}
});

//----------------------------------------------------------------
// Tokens api

app.post('/tokens', async (request, response) => {
	try {
		const createTokenResponse = await tokenWalletContract.createToken(request.body.name, request.body.uri);
		const eventResponse = await createTokenResponse.wait();
		const event = eventResponse.events.find((event) => event.event === 'tokenCreated');

		result = {
			tokenId: parseInt(event.args.tokenId._hex, 16),
			owner: event.args.owner,
			name: event.args.name,
			uri: event.args.uri
		};
		response.send(result);
	} catch (e) {
		response.send(e);
		console.log('ERROR: ' + e);
	}
});

app.get('/tokens/:id', async (request, response) => {
	try {
		const value = await tokenContract.getTokenAttributes(BigNumber.from(request.params.id)); //[ 'test', 'test', name: 'test', uri: 'test' ] displayed dublicates for listing
		result = { name: value.name, uri: value.uri }; // remove abundant variables
		response.send(result);
	} catch (e) {
		response.send(e);
		console.log('ERROR: ' + e);
	}
});

async function getListingById(id) {
	const listingResponse = await marketContract.getListing(BigNumber.from(id));
	//console.log(listingResponse);
	tokenAttributes = await tokenContract.getTokenAttributes(BigNumber.from(listingResponse.tokenId._hex));
	//console.log(tokenAttributes);
	return {
		listingId: id,
		tokenId: parseInt(listingResponse.tokenId._hex, 16),
		seller: listingResponse.seller,
		token: listingResponse.token,
		status: getListingStatus(listingResponse.status),
		price: parseInt(listingResponse.price._hex, 16),
		name: tokenAttributes.name,
		uri: tokenAttributes.uri
	};
}

function getListingStatus(status) {
	switch (status) {
		case 0:
			return 'Active';
		case 1:
			return 'Sold';
		case 2:
			return 'Cancelled';
		default:
			return 'ERROR';
	}
}

app.listen(port);
