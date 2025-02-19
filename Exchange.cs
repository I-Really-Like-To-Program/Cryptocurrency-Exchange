using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using LedgerProgram;
using WalletProgram;
using CurrenciesClass;
using CurrencyClass;
using BuyProgram;
using SellClass;
using System.Linq.Expressions;

namespace ExchangeClass
{
    class Exchange
    {
        private List<Wallet> _wallets = new List<Wallet>();

        public Exchange()
        {
        }

        public bool Login_To_Wallet(string id, string password)
        {
            return Get_Wallet(id).Login(password);
        }

        private Wallet Get_Wallet(string id)
        {
            for (int i = 0; i < _wallets.Count; i++)
            {
                if (_wallets[i].Get_ID() == id) 
                {
                    return _wallets[i];
                }
            }
            return null;
        }
    }
}