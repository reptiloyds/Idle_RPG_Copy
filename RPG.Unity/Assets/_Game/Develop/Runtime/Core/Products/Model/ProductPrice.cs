using System;
using PleasantlyGames.RPG.Runtime.Core.Const;
using PleasantlyGames.RPG.Runtime.Core.Products.Save;
using PleasantlyGames.RPG.Runtime.Core.Products.Sheet;
using PleasantlyGames.RPG.Runtime.Core.Products.Type;
using PleasantlyGames.RPG.Runtime.InAppPurchase.Contract;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using R3;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Model
{
    public class ProductPrice
    {
        [Inject] private TimeService _time;
        [Inject] private IInAppProvider _provider;

        private readonly ProductRow _config;
        private readonly ProductData _data;
        private string _formattedFree;
        private ReactiveProperty<string> _value;
        private readonly ReactiveProperty<float> _cooldown = new();
        private readonly ReactiveProperty<bool> _isFree = new();
        private readonly SerialDisposable _cooldownDisposable = new();

        public ReadOnlyReactiveProperty<float> Cooldown => _cooldown;
        public ReadOnlyReactiveProperty<string> Value => _value;
        public ReadOnlyReactiveProperty<bool> IsFree => _isFree; 

        public ProductPrice(ProductRow config, ProductData data)
        {
            _config = config;
            _data = data;
        }

        [Inject]
        private void Construct(ITranslator translator)
        {
            _formattedFree = translator.Translate(TranslationConst.PurchaseFree);
            _value = new ReactiveProperty<string>(GetPrice());

            switch (_config.PurchaseCooldown)
            {
                case PurchaseCooldownType.None:
                    break;
                case PurchaseCooldownType.NextDay:
                    if (_time.DaysFrom(_data.LastPurchase) == 0) 
                        StartCooldownToMidnight();
                    break;
            }

            _isFree.Value = IsPriceFree();
        }

        public void AfterPurchase()
        {
            switch (_config.PriceType)
            {
                case ProductPriceType.Paid:
                case ProductPriceType.Free:
                    break;
                case ProductPriceType.FreeOnce:
                    if (_data.Purchases == 1)
                        _value.Value = GetPrice();
                    break;
            }

            switch (_config.PurchaseCooldown)
            {
                case PurchaseCooldownType.None:
                    break;
                case PurchaseCooldownType.NextDay:
                    StartCooldownToMidnight();
                    break;
            }
            
            _isFree.Value = IsPriceFree();
        }

        private bool IsPriceFree()
        {
            switch (_config.PriceType)
            {
                case ProductPriceType.Paid:
                    return false;
                case ProductPriceType.FreeOnce:
                    return _data.Purchases < 1;
                case ProductPriceType.Free:
                    return true;
            }

            return false;
        }

        private string GetPrice()
        {
            string result = null;
            switch (_config.PriceType)
            {
                case ProductPriceType.Paid:
                    result = GetActualPrice();
                    break;
                case ProductPriceType.FreeOnce:
                    result = _data.Purchases > 0 ? GetActualPrice() : _formattedFree;
                    break;
                case ProductPriceType.Free:
                    result = _formattedFree;
                    break;
            }

            return result;
        }

        private string GetActualPrice() =>
            $"{_provider.GetPrice(_config.Id)}";

        private void StartCooldownToMidnight()
        {
            _cooldown.Value = _time.TimeToEndDay.CurrentValue;
            _time.LaunchGlobalTimer(_cooldownDisposable, _cooldown, TimeSpan.FromSeconds(1));
        }
    }
}