using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VendingMachine.Services.Contracts;
using VendingMachine.Services.Model;

namespace VendingMachine.Services.Services
{
    public class IOProcessor : IIOProcessor
    {
        private static readonly string _expectedPoundsInputPattern = "^\\s*£\\s*([0-9]+)\\s*$";
        private static readonly string _expectedPenceInputPattern = "^\\s*([0-9]+)\\s*p\\s*$";
        private static readonly string _exceptedSlotInputPattern = "^\\s*slot\\s*([0-9]+)\\s*";
        private static readonly string _replaceGroupPattern = "$1";

        private static readonly string _changeFormat = "Change = {0}";
        private static readonly string _itemFormat = "Item = {0}";
        private static readonly string _clientTotalFormat = "Total amount = £{0}.{1}";
        private static readonly string _poundFormat = "£{0}";
        private static readonly string _pennyFormat = "{0}p";
        private static readonly string _storageContnetFormat = "Id: {0}, Item: {1}, Quantity: {2}, Price: £{3}";
        private static readonly string _invalidInputFormat = "Input is neither a coin or a item pick. Can't be processed! {0}";
        private static readonly string _invalidItemFormat = "Invalid item! Item with id {0} doesn't exist!";
        private static readonly string _invalidItem = "Not enough quantity of the choosen product! Slot is empty!";
        private static readonly string _notEnoughChange = "There is not enough change in the machine to return!";
        private static readonly string _notEnoughCoientAmountFormat = "Entered amount is not enough to cover item cost {0}!";
        private static readonly string _storageContentFormat = $"Current storage content: {Environment.NewLine}{{0}}";
        private static readonly string _acceptableCoinsFormat = $"Currently accepting coins: {Environment.NewLine}{{0}}";

        private readonly IConsoleFacade _consoleFacade;
        private readonly Dictionary<string, int> _inputToItemId;
        private readonly Dictionary<string, int> _inputToCoinWorth;
        private readonly Dictionary<int, string> _coinWorthToOutput;

        public IOProcessor(IConsoleFacade consoleFacade)
        {
            _consoleFacade = consoleFacade;

            _inputToItemId = new Dictionary<string, int>();
            _inputToCoinWorth = new Dictionary<string, int>();
            _coinWorthToOutput = new Dictionary<int, string>();
        }

        public bool ParseInputToItemId(string inputItem, out int itemId)
        {
            return ParseSlotInput(inputItem, out itemId);
        }

        public bool ParseInputToPence(string inputCoins, out int coinWorthInPence)
        {
            return ParseCoinsInput(inputCoins, out coinWorthInPence);
        }

        public void WriteChange(List<int> changeCoins)
        {
            var coinsOutput = string.Join(", ", changeCoins.OrderBy(coin => coin).Select(coin => CoinWorthToOutput(coin)));
            _consoleFacade.WriteLine(string.Format(_changeFormat, coinsOutput));
        }

        public void WriteClientTotalAmount(int clientAmount)
        {
            int pounds = clientAmount / Constants.PoundToPensMultiplier;
            int pence = clientAmount % Constants.PoundToPensMultiplier;
            _consoleFacade.WriteLine(string.Format(_clientTotalFormat, pounds, pence));
        }

        public void WriteStorageContent(Dictionary<VendingItem, int> storageContnent)
        {
            var storageContentSerialized =
                storageContnent.Select(x => string.Format(_storageContnetFormat, x.Key.Id, x.Key.Name, x.Value, x.Key.CostInDecimal)).ToList();

            var output = string.Format(_storageContentFormat, string.Join(Environment.NewLine, storageContentSerialized));

            _consoleFacade.WriteLine(output);
        }

        public void WriteInvalidCointWorth(int coinWorth)
        {
            _consoleFacade.WriteLine("");
        }

        public void WriteCannotAcceptClientCoin(int coinWorth)
        {
            _consoleFacade.WriteLine("");
        }

        public void WriteInvalidInput(string input)
        {
            _consoleFacade.WriteLine(string.Format(_invalidInputFormat, input));
        }

        public void WriteInvalidItem(int itemId)
        {
            _consoleFacade.WriteLine(string.Format(_invalidItemFormat, itemId));
        }

        public void WriteNotEnoughQuantiy(int itemId)
        {
            _consoleFacade.WriteLine(_invalidItem);
        }

        public void WriteNotEnoughChange()
        {
            _consoleFacade.WriteLine(_notEnoughChange);
        }

        public void WriteNotEnoughClientAmount(int costInPence)
        {
            var itemCost = string.Format(_poundFormat, costInPence / (decimal)Constants.PoundToPensMultiplier);
            _consoleFacade.WriteLine(string.Format(_notEnoughCoientAmountFormat, itemCost));
        }

        public void WriteAcceptableCoins(List<int> acceptableCoins)
        {
            var coinsOutput = string.Join(", ", acceptableCoins.OrderBy(coin => coin).Select(coin => CoinWorthToOutput(coin)));
            _consoleFacade.WriteLine(string.Format(_acceptableCoinsFormat, coinsOutput));
        }

        public void WriteItemOutput(VendingItem vendingItem)
        {
            _consoleFacade.WriteLine(string.Format(_itemFormat, vendingItem.Name));
        }

        private bool ParseSlotInput(string inputItem, out int itemId)
        {
            if (_inputToItemId.ContainsKey(inputItem))
            {
                itemId = _inputToItemId[inputItem];
                return true;
            }

            if (!IsInputItemId(inputItem))
            {
                itemId = 0;
                return false;
            }

            if (!GetInputNumber(inputItem, out itemId))
            {
                itemId = 0;
                return false;
            }

            _inputToItemId[inputItem] = itemId;
            return true;
        }

        private bool ParseCoinsInput(string inputCoins, out int parsedAmountInPence)
        {
            if (_inputToCoinWorth.ContainsKey(inputCoins))
            {
                parsedAmountInPence = _inputToCoinWorth[inputCoins];
                return true;
            }

            if (!IsInputPounds(inputCoins) && !IsInputPence(inputCoins))
            {
                parsedAmountInPence = 0;
                return false;
            }

            if (!GetInputNumber(inputCoins, out int parsedAmount))
            {
                parsedAmountInPence = 0;
                return false;
            }

            parsedAmountInPence = IsInputPounds(inputCoins) ? parsedAmount * Constants.PoundToPensMultiplier : parsedAmount;
            return true;
        }

        private bool IsInputPounds(string input)
        {
            return Regex.Match(input, _expectedPoundsInputPattern, RegexOptions.IgnoreCase).Success;
        }

        private bool IsInputPence(string input)
        {
            return Regex.Match(input, _expectedPenceInputPattern, RegexOptions.IgnoreCase).Success;
        }

        private bool IsInputItemId(string input)
        {
            return Regex.Match(input, _exceptedSlotInputPattern, RegexOptions.IgnoreCase).Success;
        }

        private bool GetInputNumber(string input, out int parsedNumber)
        {
            string amountPart = string.Empty;
            amountPart = IsInputPounds(input) ? Regex.Replace(input, _expectedPoundsInputPattern, _replaceGroupPattern, RegexOptions.IgnoreCase) : amountPart;
            amountPart = IsInputPence(input) ? Regex.Replace(input, _expectedPenceInputPattern, _replaceGroupPattern, RegexOptions.IgnoreCase) : amountPart;
            amountPart = IsInputItemId(input) ? Regex.Replace(input, _exceptedSlotInputPattern, _replaceGroupPattern, RegexOptions.IgnoreCase) : amountPart;
            return int.TryParse(amountPart, out parsedNumber);

        }

        private string CoinWorthToOutput(int coinWorth)
        {
            if (_coinWorthToOutput.ContainsKey(coinWorth))
            {
                return _coinWorthToOutput[coinWorth];
            }

            string coinOutput = coinWorth > Constants.PoundToPensMultiplier ?
                string.Format(_poundFormat, coinWorth / Constants.PoundToPensMultiplier) :
                string.Format(_pennyFormat, coinWorth);

            _coinWorthToOutput[coinWorth] = coinOutput;
            return coinOutput;
        }
    }
}
