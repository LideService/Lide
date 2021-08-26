using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VendingMachine.Services.Contracts;
using VendingMachine.Services.Model;

namespace VendingMachine.Services.Services
{
    public class IoProcessor : IIoProcessor
    {
        private const string ExpectedPoundsInputPattern = "^\\s*£\\s*([0-9]+)\\s*$";
        private const string ExpectedPenceInputPattern = "^\\s*([0-9]+)\\s*p\\s*$";
        private const string ExceptedSlotInputPattern = "^\\s*slot\\s*([0-9]+)\\s*";
        private const string ReplaceGroupPattern = "$1";

        private const string ChangeFormat = "Change = {0}";
        private const string ItemFormat = "Item = {0}";
        private const string ClientTotalFormat = "Total amount = £{0}.{1}";
        private const string PoundFormat = "£{0}";
        private const string PennyFormat = "{0}p";
        private const string StorageContentFormat = "Id: {0}, Item: {1}, Quantity: {2}, Price: £{3}";
        private const string InvalidInputFormat = "Input is neither a coin or a item pick. Can't be processed! {0}";
        private const string InvalidItemFormat = "Invalid item! Item with id {0} doesn't exist!";
        private const string InvalidItem = "Not enough quantity of the choosen product! Slot is empty!";
        private const string NotEnoughChange = "There is not enough change in the machine to return!";
        private const string NotEnoughCoinsAmountFormat = "Entered amount is not enough to cover item cost {0}!";
        private static readonly string _storageContentFormat = $"Current storage content: {Environment.NewLine}{{0}}";
        private static readonly string _acceptableCoinsFormat = $"Currently accepting coins: {Environment.NewLine}{{0}}";

        private readonly IConsoleFacade _consoleFacade;
        private readonly Dictionary<string, int> _inputToItemId;
        private readonly Dictionary<string, int> _inputToCoinWorth;
        private readonly Dictionary<int, string> _coinWorthToOutput;

        public IoProcessor(IConsoleFacade consoleFacade)
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

        public void WriteChange(IEnumerable<int> changeCoins)
        {
            var coinsOutput = string.Join(", ", changeCoins.OrderBy(coin => coin).Select(CoinWorthToOutput));
            _consoleFacade.WriteLine(string.Format(ChangeFormat, coinsOutput));
        }

        public void WriteClientTotalAmount(int clientAmount)
        {
            var pounds = clientAmount / Constants.PoundToPensMultiplier;
            var pence = clientAmount % Constants.PoundToPensMultiplier;
            _consoleFacade.WriteLine(string.Format(ClientTotalFormat, pounds, pence));
        }

        public void WriteStorageContent(Dictionary<VendingItem, int> storageContent)
        {
            var storageContentSerialized =
                storageContent.Select(x => string.Format(StorageContentFormat, x.Key.Id, x.Key.Name, x.Value, x.Key.CostInDecimal)).ToList();

            var output = string.Format(_storageContentFormat, string.Join(Environment.NewLine, storageContentSerialized));

            _consoleFacade.WriteLine(output);
        }

        public void WriteInvalidCoinsWorth(int coinWorth)
        {
            _consoleFacade.WriteLine("");
        }

        public void WriteCannotAcceptClientCoin(int coinWorth)
        {
            _consoleFacade.WriteLine("");
        }

        public void WriteInvalidInput(string input)
        {
            _consoleFacade.WriteLine(string.Format(InvalidInputFormat, input));
        }

        public void WriteInvalidItem(int itemId)
        {
            _consoleFacade.WriteLine(string.Format(InvalidItemFormat, itemId));
        }

        public void WriteNotEnoughQuantity(int itemId)
        {
            _consoleFacade.WriteLine(InvalidItem);
        }

        public void WriteNotEnoughChange()
        {
            _consoleFacade.WriteLine(NotEnoughChange);
        }

        public void WriteNotEnoughClientAmount(int costInPence)
        {
            var itemCost = string.Format(PoundFormat, costInPence / (decimal)Constants.PoundToPensMultiplier);
            _consoleFacade.WriteLine(string.Format(NotEnoughCoinsAmountFormat, itemCost));
        }

        public void WriteAcceptableCoins(IEnumerable<int> acceptableCoins)
        {
            var coinsOutput = string.Join(", ", acceptableCoins.OrderBy(coin => coin).Select(CoinWorthToOutput));
            _consoleFacade.WriteLine(string.Format(_acceptableCoinsFormat, coinsOutput));
        }

        public void WriteItemOutput(VendingItem vendingItem)
        {
            _consoleFacade.WriteLine(string.Format(ItemFormat, vendingItem.Name));
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

            var isPounds = IsInputPounds(inputCoins);
            var isPence = IsInputPence(inputCoins);
            if (!isPounds && !isPence)
            {
                parsedAmountInPence = 0;
                return false;
            }

            if (!GetInputNumber(inputCoins, out var parsedAmount))
            {
                parsedAmountInPence = 0;
                return false;
            }

            parsedAmountInPence = isPounds ? parsedAmount * Constants.PoundToPensMultiplier : parsedAmount;
            _inputToCoinWorth[inputCoins] = parsedAmountInPence;
            return true;
        }

        private string CoinWorthToOutput(int coinWorth)
        {
            if (_coinWorthToOutput.ContainsKey(coinWorth))
            {
                return _coinWorthToOutput[coinWorth];
            }

            var coinOutput = coinWorth > Constants.PoundToPensMultiplier ?
                string.Format(PoundFormat, coinWorth / Constants.PoundToPensMultiplier) :
                string.Format(PennyFormat, coinWorth);

            _coinWorthToOutput[coinWorth] = coinOutput;
            return coinOutput;
        }

        private static bool GetInputNumber(string input, out int parsedNumber)
        {
            var amountPart = string.Empty;
            amountPart = IsInputPounds(input) ? Regex.Replace(input, ExpectedPoundsInputPattern, ReplaceGroupPattern, RegexOptions.IgnoreCase) : amountPart;
            amountPart = IsInputPence(input) ? Regex.Replace(input, ExpectedPenceInputPattern, ReplaceGroupPattern, RegexOptions.IgnoreCase) : amountPart;
            amountPart = IsInputItemId(input) ? Regex.Replace(input, ExceptedSlotInputPattern, ReplaceGroupPattern, RegexOptions.IgnoreCase) : amountPart;
            return int.TryParse(amountPart, out parsedNumber);
        }

        private static bool IsInputPounds(string input)
        {
            return Regex.Match(input, ExpectedPoundsInputPattern, RegexOptions.IgnoreCase).Success;
        }

        private static bool IsInputPence(string input)
        {
            return Regex.Match(input, ExpectedPenceInputPattern, RegexOptions.IgnoreCase).Success;
        }

        private static bool IsInputItemId(string input)
        {
            return Regex.Match(input, ExceptedSlotInputPattern, RegexOptions.IgnoreCase).Success;
        }
    }
}
