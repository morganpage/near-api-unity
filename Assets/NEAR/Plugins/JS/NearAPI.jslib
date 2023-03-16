mergeInto(LibraryManager.library, {

  RequestSignIn: async function (contractId, network = "testnet") {
    let contractIdStr = UTF8ToString(contractId);
    let networkStr = UTF8ToString(network);
    console.log("RequestSignIn", contractIdStr, networkStr);
    const { connect, WalletConnection, getConfig } = window.nearApi;
    const nearConnection = await connect(getConfig(networkStr));
    const walletConnection = new WalletConnection(nearConnection);
    walletConnection.requestSignIn(contractIdStr);
  },

  SignOut: async function (network = "testnet") {
    let networkStr = UTF8ToString(network);
    const { connect, WalletConnection, getConfig } = window.nearApi;
    const nearConnection = await connect(getConfig(networkStr));
    const walletConnection = new WalletConnection(nearConnection);
    walletConnection.signOut();
  },

  IsSignedIn: async function (network = "testnet") {
    let networkStr = UTF8ToString(network);
    const { connect, WalletConnection, getConfig } = window.nearApi;
    const nearConnection = await connect(getConfig(networkStr));
    const walletConnection = new WalletConnection(nearConnection);
    var signedIn = walletConnection.isSignedIn();
    SendMessage('NearCallbacks', 'IsSignedIn', signedIn ? 'true' : 'false');
  },

  NftTokensForOwner: async function (accountId, contractId, network = "testnet") {
    let networkStr = UTF8ToString(network);
    let accountIdStr = UTF8ToString(accountId);
    let contractIdStr = UTF8ToString(contractId);
    const { connect, Contract, getConfig } = window.nearApi;
    const nearConnection = await connect(getConfig(networkStr));
    const account = await nearConnection.account(accountIdStr);
    const contract = await new Contract(account, contractIdStr, {
      viewMethods: ["nft_total_supply", "nft_supply_for_owner", "nft_tokens_for_owner"],
      sender: account.accountId,
    });
    const tokens = await contract.nft_tokens_for_owner({ account_id: account.accountId });
    var json = JSON.stringify(tokens);
    SendMessage('NearCallbacks', 'NftTokensForOwner', json);
  },

  GetAccountId: async function (network = "testnet") {
    let networkStr = UTF8ToString(network);
    const { connect, WalletConnection, getConfig } = window.nearApi;
    const nearConnection = await connect(getConfig(networkStr));
    const walletConnection = new WalletConnection(nearConnection);
    var accountId = walletConnection.getAccountId();
    SendMessage('NearCallbacks', 'GetAccountId', accountId);
  },

  ContractMethod: async function (accountId, contractId, methodName, args, changeMethod = false, network = "testnet") {
    let networkStr = UTF8ToString(network);
    let accountIdStr = UTF8ToString(accountId);
    let contractIdStr = UTF8ToString(contractId);
    let methodNameStr = UTF8ToString(methodName);
    let argsStr = UTF8ToString(args);
    console.log("ViewMethod: ", argsStr);
    let argsObj = {};
    try {
      argsObj = JSON.parse(argsStr);
    } catch (error) {
      console.log("ViewMethod: ", error);
    }

    const { connect, Contract, getConfig } = window.nearApi;
    const nearConnection = await connect(getConfig(networkStr));
    const account = await nearConnection.account(accountIdStr);
    const contract = await new Contract(account, contractIdStr, {
      viewMethods: changeMethod ? [] : [methodNameStr],
      changeMethods: changeMethod ? [methodNameStr] : [],
      sender: account.accountId,
    });

    let fnContract = contract[methodNameStr];
    const result = await fnContract(argsObj);
    var json = JSON.stringify(result);
    SendMessage('NearCallbacks', 'ContractMethod', json);
  },


});