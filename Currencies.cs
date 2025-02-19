using System;
using CurrencyClass;
using Newtonsoft.Json.Linq;

namespace CurrenciesClass
{
    public class Currencies
    {
        protected List<Currency> _currencies;
        public Currencies(JArray json)
        {
            _currencies = Get_Currencies(json);
        }

        public List<Currency> Get_Currencies(JArray json)
        {
            List<Currency> append_to_currencies = new List<Currency>();
            foreach (var coin in json)
            {
                string name = coin["name"].ToString();
                string symbol = coin["symbol"].ToString().ToUpper();
                double price = Convert.ToDouble(coin["current_price"]);
                double marketCap = Convert.ToDouble(coin["market_cap"]);
                var currency = new Currency(name, symbol, price, marketCap);
                append_to_currencies.Add(currency);
            }
            return append_to_currencies;
        }

        public Currency Find_Currency(string name)
        {
            for (int i = 0; i < _currencies.Count; i++)
            {
                if (_currencies[i].Get_Name() == name || _currencies[i].Get_Symbol() == name)
                {
                    return _currencies[i];
                }
            }
            return null;
        }

        public void Print_Currencies()
        {
            for (int i = 0; i < _currencies.Count; i++)
            {
                Console.WriteLine($"Name: {_currencies[i].Get_Name()} | Price: {_currencies[i].Get_Price_USD()}");
            }
        }
    }
}