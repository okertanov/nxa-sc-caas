using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
using System.ComponentModel;
using System.Numerics;
using Neo;

namespace NFT {
    [DisplayName("{{DisplayName}}")]
    [ManifestExtra("Author", "{{Author}}")]
    [ManifestExtra("Email", "{{Email}}")]
    [ManifestExtra("Email", "{{Email}}")]
    [ManifestExtra("Description", "{{Description}}")]
    partial class NFTContract : SmartContract {
        [DisplayName("MintToken")]
        public static event Action<UInt160, ByteString, byte[]> MintTokenNotify;

        [DisplayName("BurnToken")]
        public static event Action<UInt160, ByteString, BigInteger> BurnTokenNotify;

        [DisplayName("Transfer")]
        public static event Action<UInt160, UInt160, BigInteger, ByteString> TransferNotify;

        [InitialValue("{{Owner}}", ContractParameterType.Hash160)]
        private static readonly UInt160 owner = default;

        private static readonly UInt160 superAdmin = "{{ContractAdminAddress}}".ToScriptHash();

        private const int TOKEN_DECIMALS = {{TokenDecimals}};
        private const int FACTOR = {{TokenFactor}};

        public static string Symbol() {
            return "{{TokenSymbol}}";
        }
    }
}
