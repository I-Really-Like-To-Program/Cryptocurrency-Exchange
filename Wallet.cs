using System;
using CurrenciesClass;
using CurrencyClass;
using LedgerProgram;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using EncryptorClass;

namespace WalletProgram
{
    public class Wallet
    {
        private string _id;
        protected string _password;
        private byte[] _key;
        private byte[] _iv;
        protected double _balance_usd = 0;
        protected Dictionary<string, double> _user_assets = new Dictionary<string, double>();
        protected List<Ledger> _ledgers = new List<Ledger>();
        public Wallet(string id, string password, Currencies currencies, JArray info)
        {
            _id = id;

            // Allocate memory for _key and _iv
            _key = new byte[32]; // AES 256-bit key (32 bytes)
            _iv = new byte[16];  // AES IV (128-bit, 16 bytes)

            // Generate random key and IV
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(_key);
                rng.GetBytes(_iv);
            }
            var encryptor = new Encryptor(_key, _iv);
            _password = encryptor.Encrypt(password);
            _user_assets = Set_User_Assets(currencies, info);
        }

        public Dictionary<string, double> Set_User_Assets(Currencies currencies, JArray info)
        {
            var currenciesList = currencies.Get_Currencies(info);
            foreach (var currency in currenciesList)
            {
                _user_assets.Add(currency.Get_Name(), 0);  // Add the currency's name as a key with 0 as initial value
            }
            return _user_assets;
        }

        public bool Login(string password)
        {
            var encryptor = new Encryptor(_key, _iv);
            if (_password == encryptor.Encrypt(password))
            {
                return true;
            }
            return false;
        }

        public void Add_Money(double amount)
        {
            if (amount > 0)
            {
                _balance_usd += amount;
            }
            //Add Exceptions, change conditional statement to amount < 0 etc.
        }

        public void Print_Assets()
        {
            Console.WriteLine($"Balance (USD): {_balance_usd}");
            for (int i = 0; i < _user_assets.Count; i++)
            {
                if (_user_assets.ElementAt(i).Value != 0)
                {
                    Console.WriteLine($"Currency: {_user_assets.ElementAt(i).Key} | Amount: {_user_assets.ElementAt(i).Value}");
                }
            }
        }

        public double Account_Balance()
        {
            return _balance_usd;
        }

        public string Get_ID()
        {
            return _id;
        }

        public double Get_Balance(string currency)
        {
            return _user_assets[currency];
        }

        public void Add_Ledger(Ledger ledger)
        {
            _ledgers.Add(ledger);
        }

        public void Execute_Ledger(Ledger ledger)
        {
            ledger.Execute();
        }

        public bool Buy_Currency(Currency currency, double amount)
        {
            if (amount > _balance_usd)
            {
                throw new InvalidOperationException("Cost exceeds your account balance!");
            }
            double _asset_amount = amount / currency.Get_Price_USD();
            _balance_usd -= amount;
            _user_assets[currency.Get_Name()] += _asset_amount;
            return true;
        }

        public bool Sell_Currency(Currency currency, double amount)
        {
            if (amount < 0)
            {
                throw new InvalidOperationException("Cannot sell a negative amount of an asset");
            }
            if (_user_assets[currency.Get_Name()] < amount)
            {
                throw new InvalidOperationException("Trying to sell more than you own of this asset!");
            }
            _balance_usd += currency.Get_Price_USD() * amount;
            _user_assets[currency.Get_Name()] -= amount;
            return false;
        }
    }
}