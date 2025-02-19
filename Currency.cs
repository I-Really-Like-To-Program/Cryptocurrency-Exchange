using System;

namespace CurrencyClass
{
    public class Currency
    {
        protected string _name;
        protected string _symbol;
        protected double _price;
        protected double _market_cap;
        public Currency(string name, string symbol, double price, double marketcap)
        {
            _name = name;
            _symbol = symbol;
            _price = price;
            _market_cap = marketcap;
        }

        public string Get_Name()
        {
            return _name;
        }

        public string Get_Symbol()
        {
            return _symbol;
        }

        public double Get_Price_USD()
        {
            return _price;
        }

        public double Get_Market_Cap()
        {
            return _market_cap;
        }
    }
}