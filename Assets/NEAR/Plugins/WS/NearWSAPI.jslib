mergeInto(LibraryManager.library, {

  WS_StartUp: async function (contractId, network = "testnet") {
    let contractIdStr = UTF8ToString(contractId);
    let networkStr = UTF8ToString(network);
    const nearWallet = new Wallet({ createAccessKeyFor: contractIdStr, network: networkStr });
    isSignedIn = await nearWallet.startUp();
    window.nearWallet = nearWallet;
    SendMessage('NearCallbacks', 'IsSignedIn', isSignedIn ? 'true' : 'false');
  },

  WS_SignIn: async function () {
    await window.nearWallet.signIn();
  },

  WS_SignOut: async function () {
    await window.nearWallet.signOut();
  },
  WS_IsSignedIn: async function () {
    var signedIn = window.nearWallet.walletSelector.isSignedIn();
    SendMessage('NearCallbacks', 'IsSignedIn', signedIn ? 'true' : 'false');
  },
  WS_GetAccountId: async function () {
    var accountId = window.nearWallet.accountId;
    SendMessage('NearCallbacks', 'GetAccountId', accountId);
  },

  WS_ViewMethod: async function (contractId, method, args) {
    let contractIdStr = UTF8ToString(contractId);
    let methodStr = UTF8ToString(method);
    let argsStr = UTF8ToString(args);
    let argsObj = {};
    try {
      argsObj = JSON.parse(argsStr);
    } catch (error) {
      console.log("ViewMethod: ", error);
    }
    let messages = await window.nearWallet.viewMethod({ contractId: contractIdStr, method: methodStr, args: argsObj });
    var json = JSON.stringify(messages);
    SendMessage('NearCallbacks', 'ViewMethod', json);
  },

  WS_CallMethod: async function (contractId, method, args) {
    let contractIdStr = UTF8ToString(contractId);
    let methodStr = UTF8ToString(method);
    let argsStr = UTF8ToString(args);
    let argsObj = {};
    try {
      argsObj = JSON.parse(argsStr);
    } catch (error) {
      console.log("CallMethod: ", error);
    }
    let messages = await window.nearWallet.callMethod({ contractId: contractIdStr, method: methodStr, args: argsObj });
    var json = JSON.stringify(messages);
    SendMessage('NearCallbacks', 'CallMethod', json);
  }

});