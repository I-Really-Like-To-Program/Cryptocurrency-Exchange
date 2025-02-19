using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using LedgerProgram;
using WalletProgram;
using CurrenciesClass;
using CurrencyClass;
using ExchangeClass;
using BuyProgram;
using SellClass;
using System.Linq.Expressions;

class Program
{
    static async Task<JArray> Get_Info()
    {
        string baseUrl = "https://api.coingecko.com/api/v3/";
        string vsCurrency = "usd";
        int perPage = 100; // Number of coins per page
        int page = 1;
        string endpoint = $"coins/markets?vs_currency={vsCurrency}&order=market_cap_desc&per_page={perPage}&page={page}";

        using HttpClient client = new HttpClient { BaseAddress = new Uri(baseUrl) };
        client.DefaultRequestHeaders.Add("User-Agent", "CSharpApp");

        HttpResponseMessage response = await client.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        return JArray.Parse(responseBody);
    }

    static Wallet Create_Wallet(Currencies currencies, JArray info)
    {
        Console.WriteLine("Enter wallet id: ");
        string id = Console.ReadLine();
        Console.WriteLine("Enter password for wallet: ");
        string password = Console.ReadLine();
        var wallet = new Wallet(id, password, currencies, info);
        return wallet;
    }

    static void Buy_Crypto(Wallet wallet, Currencies currencies)
    {
        Console.WriteLine("Enter currency name: ");
        string name = Console.ReadLine();
        var currency = currencies.Find_Currency(name);
        if (currency == null)
        {
            throw new InvalidOperationException("Currency not found!");
        }
        double amount = 0;
        Console.WriteLine("Enter amount (USD): ");
        try
        {
            amount = Convert.ToDouble(Console.ReadLine());
        }
        catch (Exception ex)
        {
            Console.WriteLine("Couldn't convert to double");
        }
        if (amount < 0)
        {
            throw new InvalidOperationException("Cannot buy a negative amount of an asset!");
        }
        var ledger = new Buy(currency, amount, wallet);
        wallet.Add_Ledger(ledger);
        wallet.Execute_Ledger(ledger);
    }

    static void Sell_Crypto(Wallet wallet, Currencies currencies)
    {
        Console.WriteLine("Enter currency name: ");
        string name = Console.ReadLine();
        var currency = currencies.Find_Currency(name);
        if (currency == null)
        {
            throw new InvalidOperationException("Currency not found!");
        }
        double amount = 0;
        Console.WriteLine("Enter amount: ");
        try
        {
            amount = Convert.ToDouble(Console.ReadLine());
        }
        catch (Exception ex)
        {
            Console.WriteLine("Couldn't convert to double");
        }
        if (amount < 0)
        {
            throw new InvalidOperationException("Cannot sell a negative amount of an asset!");
        }
        var ledger = new Sell(currency, amount, wallet);
        wallet.Add_Ledger(ledger);
        wallet.Execute_Ledger(ledger);
    }

    static void Wallet_Add_Money(Wallet wallet)
    {
        if (wallet == null)
        {
            throw new InvalidOperationException("Trying to add money to an account that doesn't exist!");
        }
        double amount = 0;
        Console.WriteLine("Enter amount to add (USD): ");
        try
        {
            amount = Convert.ToDouble(Console.ReadLine());
        }
        catch (Exception ex)
        {
            Console.WriteLine("Invalid Input! Must enter a number!");
            throw new InvalidOperationException();
        }
        wallet.Add_Money(amount);
    }

    static Wallet Find_Wallet(List<Wallet> wallets)
    {
        Console.WriteLine("Enter wallet id: ");
        string id = Console.ReadLine();
        Console.WriteLine("Enter wallet password: ");
        string password = Console.ReadLine();
        for (int i = 0; i < wallets.Count; i++)
        {
            if (wallets[i].Get_ID() == id && wallets[i].Login(password))
            {
                return wallets[i];
            }
        }
        return null;
    }

    static async Task Main(string[] args)
    {
        JArray info = await Get_Info();
        var currencies = new Currencies(info);
        bool loop_while = true;
        List<Wallet> wallets = new List<Wallet>();
        var exchange = new Exchange();
        while (loop_while)
        {
            int user_input = 0;
            Console.WriteLine("(1) - Create Account: ");
            Console.WriteLine("(2) - Add Money: ");
            Console.WriteLine("(3) - Print Currencies: ");
            Console.WriteLine("(4) - Buy Currency: ");
            Console.WriteLine("(5) - Sell Currency: ");
            Console.WriteLine("(6) - Print Account Details: ");
            Console.WriteLine("(7) - Quit: ");
            try
            {
                user_input = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid Input! Must be an integer!");
            }
            try
            {
                switch (user_input)
                {
                    case 0: Console.WriteLine("No option selected!"); break;
                    case 1: wallets.Add(Create_Wallet(currencies, info)); break;
                    case 2: Wallet_Add_Money(Find_Wallet(wallets)); break;
                    case 3: info = await Get_Info(); currencies = new Currencies(info); currencies.Print_Currencies(); break;
                    case 4: info = await Get_Info(); currencies = new Currencies(info); Buy_Crypto(Find_Wallet(wallets), currencies); break;
                    case 5: info = await Get_Info(); currencies = new Currencies(info); Sell_Crypto(Find_Wallet(wallets), currencies); break;
                    case 6: Find_Wallet(wallets).Print_Assets(); break;
                    case 7: loop_while = false; break;
                    default: Console.WriteLine("No option selected!"); break;
                }
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}