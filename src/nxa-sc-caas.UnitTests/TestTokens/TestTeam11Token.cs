using System;
using System.Numerics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Examples
{
    public abstract class NXATokenContract : SmartContract.Framework.SmartContract
    {
        protected const byte Prefix_TotalSupply = 0x00;
        protected const byte Prefix_Balance = 0x01;

        [Safe]
        [DisplayName("Name")]
        public abstract string TokenName();

        [Safe]
        public abstract string Symbol();

        [Safe]
        public abstract byte Decimals();

        [Safe]
        public static BigInteger TotalSupply() => (BigInteger)Storage.Get(Storage.CurrentContext, new byte[] { Prefix_TotalSupply });

        [Safe]
        public static BigInteger BalanceOf(UInt160 owner)
        {
            if (owner is null || !owner.IsValid)
                throw new Exception("The argument \"owner\" is invalid.");
            StorageMap balanceMap = new(Storage.CurrentContext, Prefix_Balance);
            return (BigInteger)balanceMap[owner];
        }

        protected static void UpdateTotalSupply(BigInteger increment)
        {
            StorageContext context = Storage.CurrentContext;
            byte[] key = new byte[] { Prefix_TotalSupply };
            BigInteger totalSupply = (BigInteger)Storage.Get(context, key);
            totalSupply += increment;
            Storage.Put(context, key, totalSupply);
        }

        protected static bool UpdateBalance(UInt160 owner, BigInteger increment)
        {
            StorageMap balanceMap = new(Storage.CurrentContext, Prefix_Balance);
            BigInteger balance = (BigInteger)balanceMap[owner];
            balance += increment;
            if (balance < 0) return false;
            if (balance.IsZero)
                balanceMap.Delete(owner);
            else
                balanceMap.Put(owner, balance);
            return true;
        }
    }

    //[SupportedStandards("NEP-17")]
    [ContractPermission("*", "onNEP17Payment")]
    public abstract class NXANep17Token : NXATokenContract
    {
        public delegate void OnTransferDelegate(UInt160 from, UInt160 to, BigInteger amount);

        [DisplayName("Transfer")]
        public static event OnTransferDelegate OnTransfer;

        public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data)
        {
            if (from is null || !from.IsValid)
                throw new Exception("The argument \"from\" is invalid.");
            if (to is null || !to.IsValid)
                throw new Exception("The argument \"to\" is invalid.");
            if (amount < 0)
                throw new Exception("The amount must be a positive number.");
            if (!Runtime.CheckWitness(from)) return false;
            if (amount != 0)
            {
                if (!UpdateBalance(from, -amount))
                    return false;
                UpdateBalance(to, +amount);
            }
            PostTransfer(from, to, amount, data);
            return true;
        }

        protected static void MintImpl(UInt160 account, BigInteger amount)
        {
            if (amount.Sign < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (amount.IsZero) return;
            UpdateBalance(account, +amount);
            UpdateTotalSupply(+amount);
            PostTransfer(null, account, amount, null);
        }

        protected static void BurnImpl(UInt160 account, BigInteger amount)
        {
            if (amount.Sign < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (amount.IsZero) return;
            if (!UpdateBalance(account, -amount))
                throw new InvalidOperationException();
            UpdateTotalSupply(-amount);
            PostTransfer(account, null, amount, null);
        }

        protected static void PostTransfer(UInt160 from, UInt160 to, BigInteger amount, object data)
        {
            OnTransfer(from, to, amount);
            if (to is not null && ContractManagement.GetContract(to) is not null)
                Contract.Call(to, "onNEP17Payment", CallFlags.All, from, amount, data);
        }
    }

    [ManifestExtra("Author", "Team11")]
    [ManifestExtra("Email", "okertanov@gmail.org")]
    [ManifestExtra("Description", "Team11 Token with CaaS")]
    //[SupportedStandards("NEP-17")]
    [ContractPermission("*", "onNEP17Payment")]
    public partial class Team11Token : NXANep17Token
    {
        [InitialValue("NZJsKhsKzi9ipzjC57zU53EVMC97zqPDKG", ContractParameterType.Hash160)]
        private static readonly UInt160 owner = default;
        // Prefix_TotalSupply = 0x00; Prefix_Balance = 0x01;
        private const byte Prefix_Contract = 0x02;
        public static readonly StorageMap ContractMap = new StorageMap(Storage.CurrentContext, Prefix_Contract);
        private static readonly byte[] ownerKey = "owner".ToByteArray();
        private static bool IsOwner() => Runtime.CheckWitness(GetOwner());
        private static int InitialCoins => 1_000_000;

        [DisplayName("Name")]
        public override string TokenName() => "Team11 Token";

        public override string Symbol() => "T11";

        public override byte Decimals() => 2;


        public static void _deploy(object data, bool update)
        {
            if (update) return;
            ContractMap.Put(ownerKey, owner);
            //Team11Token.Mint(owner, Team11Token.InitialCoins);
        }
        
        public static UInt160 GetOwner()
        {
            return (UInt160)ContractMap.Get(ownerKey);
        }

        public static new void Mint(UInt160 account, BigInteger amount)
        {
            if (!IsOwner()) throw new InvalidOperationException("No Authorization!");
            NXANep17Token.MintImpl(account, amount);
        }

        public static new void Burn(UInt160 account, BigInteger amount)
        {
            if (!IsOwner()) throw new InvalidOperationException("No Authorization!");
            NXANep17Token.BurnImpl(account, amount);
        }

        public static bool Update(ByteString nefFile, string manifest)
        {
            if (!IsOwner()) throw new InvalidOperationException("No Authorization!");
            ContractManagement.Update(nefFile, manifest, null);
            return true;
        }

        public static bool Destroy()
        {
            if (!IsOwner()) throw new InvalidOperationException("No Authorization!");
            ContractManagement.Destroy();
            return true;
        }
    }
}

/*
block: 172030/172030  connected: 1  unconnected: 2 

nxa> deploy /nxa-node-data/t11.bin /nxa-node-data/t11.manifest
Contract hash: 0x9072b3814fc2de5b4e122f73703ff313317d4ed6
Gas consumed: 10.0535103
Network fee: 0.0462352
Total fee: 10.0997455 GAS
Relay tx? (no|yes): yes
Signed and relayed transaction with hash:
0xca393bd207ff355017631ec16a8a9cb16e02ea7d34ebbfa10d1874b5a143f34e

balanceof 0x9072b3814fc2de5b4e122f73703ff313317d4ed6 NZJsKhsKzi9ipzjC57zU53EVMC97zqPDKG

invoke 0x9072b3814fc2de5b4e122f73703ff313317d4ed6 mint NZJsKhsKzi9ipzjC57zU53EVMC97zqPDKG 1000

invoke 0x9072b3814fc2de5b4e122f73703ff313317d4ed6 symbol

convert NZJsKhsKzi9ipzjC57zU53EVMC97zqPDKG

invoke 0x9072b3814fc2de5b4e122f73703ff313317d4ed6 balanceOf [{"type":"ByteArray","value":"ku5O5IVJRg4eLYfZSxhnZdU47U0="}]

invoke 0x9072b3814fc2de5b4e122f73703ff313317d4ed6 mint [{"type":"ByteArray","value":"ku5O5IVJRg4eLYfZSxhnZdU47U0="},{"type":"Integer","value":"100000"}]

*/