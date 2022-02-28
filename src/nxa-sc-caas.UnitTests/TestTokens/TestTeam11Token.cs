using System;
using System.Numerics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Attributes;

namespace Neo.SmartContract.Examples
{
    public abstract class NXATokenContract : SmartContract.Framework.SmartContract
    {
        protected const byte Prefix_TotalSupply = 0x00;
        protected const byte Prefix_Balance = 0x01;
        protected const byte Prefix_Contract = 0x02;

        [Safe]
        [DisplayName("name")]
        public abstract string TokenName();

        [Safe]
        [DisplayName("symbol")]
        public abstract string Symbol();

        [Safe]
        [DisplayName("decimals")]
        public abstract byte Decimals();

        [Safe]
        [DisplayName("totalSupply")]
        public static BigInteger TotalSupply()
        {
            return (BigInteger)Storage.Get(Storage.CurrentContext, new byte[] { Prefix_TotalSupply });
        }

        [Safe]
        [DisplayName("balanceOf")]
        public static BigInteger BalanceOf(UInt160 owner)
        {
            if (owner is null || !owner.IsValid)
            {
                throw new Exception("BalanceOf: The argument 'owner' is invalid.");
            }
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
            if (balance < 0)
            {
                return false;
            }

            if (balance.IsZero)
            {
                balanceMap.Delete(owner);
            }
            else
            {
                balanceMap.Put(owner, balance);
            }

            return true;
        }
    }

    [SupportedStandards("NEP-17")]
    [ContractPermission("*", "onNEP17Payment")]
    public abstract class NXANep17Token : NXATokenContract
    {
        public delegate void OnTransferDelegate(UInt160 from, UInt160 to, BigInteger amount);

        [DisplayName("Transfer")]
        public static event OnTransferDelegate OnTransfer;

        public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount, object data)
        {
            if (from is null || !from.IsValid)
            {
                throw new Exception("Transfer: The argument 'from' is invalid.");
            }

            if (to is null || !to.IsValid)
            {
                throw new Exception("Transfer: The argument 'to' is invalid.");
            }

            if (amount < 0)
            {
                throw new Exception("Transfer: The amount must be a positive number.");
            }

            if (!Runtime.CheckWitness(from))
            {
                return false;
            }

            if (amount != 0)
            {
                if (!UpdateBalance(from, -amount))
                {
                    return false;
                }

                UpdateBalance(to, +amount);
            }

            PostTransfer(from, to, amount, data);

            return true;
        }

        protected static void MintImpl(UInt160 account, BigInteger amount)
        {
            if (amount.Sign < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            if (amount.IsZero)
            {
                return;
            }

            UpdateBalance(account, +amount);
            UpdateTotalSupply(+amount);

            PostTransfer(null, account, amount, null);
        }

        protected static void BurnImpl(UInt160 account, BigInteger amount)
        {
            if (amount.Sign < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            if (amount.IsZero)
            {
                return;
            }

            if (!UpdateBalance(account, -amount))
            {
                throw new InvalidOperationException();
            }

            UpdateTotalSupply(-amount);

            PostTransfer(account, null, amount, null);
        }

        protected static void PostTransfer(UInt160 from, UInt160 to, BigInteger amount, object data)
        {
            OnTransfer(from, to, amount);

            if (to is not null && ContractManagement.GetContract(to) is not null)
            {
                Contract.Call(to, "onNEP17Payment", CallFlags.All, from, amount, data);
            }
        }
    }

    [ManifestExtra("Author", "Team11")]
    [ManifestExtra("Email", "info@team11.it")]
    [ManifestExtra("Description", "Team11 Token with CaaS")]
    [ManifestExtra("Version", "1.3")]
    [SupportedStandards("NEP-17")]
    [ContractPermission("*", "onNEP17Payment")]
    public sealed class Team11Token : NXANep17Token
    {
        //
        // WARNING: Replace it with your own address.
        //
        [InitialValue("NZJsKhsKzi9ipzjC57zU53EVMC97zqPDKG", ContractParameterType.Hash160)]
        private static readonly new UInt160 owner = default;

        private static readonly byte[] ownerKey = "owner".ToByteArray();       

        private static readonly StorageMap ContractMap = new StorageMap(Storage.CurrentContext, Prefix_Contract);
        
        //
        // Initial Coins w/o 'decimals' e.g 1M is 1M coins,
        // 100M when for example 'decimals' is '2'.
        //
        private static int InitialCoins => 100_000_000;

        [Safe]
        [DisplayName("name")]
        public override string TokenName() => "Team11 Token";

        [Safe]
        [DisplayName("symbol")]
        public override string Symbol() => "T11";

        [Safe]
        [DisplayName("decimals")]
        public override byte Decimals() => 2;

        [InitialValue("64", Neo.SmartContract.ContractParameterType.ByteArray)] 
        private static readonly BigInteger ConvertDecimal;

        [Safe]
        public static string[] SupportedStandards() => new string[] { "NEP-17" };

        //
        // It will be executed during deploy.
        //
        public static void _deploy(object data, bool update)
        {
            if (update) {
                return;
            }

            ContractMap.Put(ownerKey, owner);
            // Impl w/o checking Witness since there is no TX context here.
            NXANep17Token.MintImpl(GetOwner(), Team11Token.InitialCoins);
        }

        [Safe]
        public static bool IsOwner()
        {
            return Runtime.CheckWitness(GetOwner());
        }

        [Safe]
        public static UInt160 GetOwner()
        {
           return (UInt160)ContractMap.Get(ownerKey);
        }

        public static UInt160 SetOwner(UInt160 account)
        {
            var oldOwner = (UInt160)ContractMap.Get(ownerKey);
            CheckOwner(account);
            ContractMap.Put(ownerKey, owner);
            return oldOwner;
        }

        public static void CheckOwner(UInt160 account)
        {
            var isZero = account.IsZero ? "Zero" : "Nonzero";
            var isValid = account.IsValid ? "Valid" : "Invalid";
            var address = StdLib.Base64Encode(account);
            if (!IsOwner())
            {
                throw new InvalidOperationException($"No Authorization for account: {isZero}, {isValid}, {address}");
            }
        }

        [DisplayName("mint")]
        public static new void Mint(UInt160 account, BigInteger amount)
        {
            CheckOwner(account);
            amount = amount / ConvertDecimal;
            NXANep17Token.MintImpl(account, amount);
        }

        [DisplayName("burn")]
        public static new void Burn(UInt160 account, BigInteger amount)
        {
            CheckOwner(account);

            NXANep17Token.BurnImpl(account, amount);
        }

         [DisplayName("update")]
        public static bool Update(ByteString nefFile, string manifest)
        {
            CheckOwner(UInt160.Zero);
            
            ContractManagement.Update(nefFile, manifest, null);

            return true;
        }

        [DisplayName("destroy")]
        public static bool Destroy()
        {
            CheckOwner(UInt160.Zero);

            ContractManagement.Destroy();

            return true;
        }
    }
}
