# BlockchainGame

Blokų grandinės paleidimas:
  npx ganache-cli //paleidžiama lokali blokų grandinė
  npx truffle migrate --reset //sukompiliuojami išmanieji kontraktai ir sudiegiami į lokalią blokų grandinę

REST API:
  Nustatyti šiuos kintamuosius gautus iš ganache-cli blokų grandinės konsolės:
    MNEMONIC
    buyerWalletPrivateKey
  Nustatyti šiuos kintamuosius gautus iš truffle išmaniųjų kontraktų diegimo konsolės:
    tokenContractAddress
    marketContractAddress
  Paleisti REST API su komanda npm start
  
Unity žaidimas:
  Paleisti žaidimą su unity įrankiu v.2021.3.3f1
