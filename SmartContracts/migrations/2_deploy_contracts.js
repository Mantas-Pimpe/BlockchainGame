const Token = artifacts.require('Token');
const Market = artifacts.require('Market');

module.exports = function(deployer) {
	deployer.deploy(Token);
	deployer.deploy(Market);
};

//npx truffle compile
//npx truffle migrate
//npx ganache-cli
//npx truffle migrate --reset
//npx truffle console

//const t = await Token.deployed()
//t.createToken('test', 'test')
//t.approve(m.address, 1)
//const m = await Market.deployed()
//m.createTokenListing(t.address, 1, 1000)
//m.getListing(0)
