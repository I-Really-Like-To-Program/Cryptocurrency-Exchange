using System;
using CurrencyClass;
using WalletProgram;

namespace LedgerProgram
{
    public class Ledger
    {
        protected double _amount;
        protected Wallet _wallet;
        protected Currency _currency;
        public DateTime _time;
        public Ledger(Currency currency, double amount, Wallet wallet)
        {
            _amount = amount;
            _wallet = wallet;
            _currency = currency;
        }

        public virtual void Execute()
        {
        }
    }
}