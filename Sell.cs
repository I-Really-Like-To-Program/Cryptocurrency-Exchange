using System;
using CurrencyClass;
using LedgerProgram;
using WalletProgram;


namespace SellClass
{
    public class Sell : Ledger
    {
        protected double _amount;
        protected Wallet _wallet;
        protected Currency _currency;
        protected bool _executed = false;
        public Sell(Currency currency, double amount, Wallet wallet) : base(currency, amount, wallet)
        {
            _currency = currency;
            _amount = amount;
            _wallet = wallet;
        }
        public override void Execute()
        {
            _time = DateTime.Now;
            if (_executed)
            {
                throw new InvalidOperationException("Cannot execute a transaction which has already been executed!");
            }
            _executed = _wallet.Sell_Currency(_currency, _amount);
            //Console.WriteLine("");
        }
    }
}
