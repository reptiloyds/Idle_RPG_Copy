using System;
using System.Collections.Generic;
using System.Globalization;
using PleasantlyGames.RPG.Runtime.Core.Products.Model.Periodic;
using PleasantlyGames.RPG.Runtime.Core.Products.Save;
using PleasantlyGames.RPG.Runtime.Core.Products.Type;
using PleasantlyGames.RPG.Runtime.TimeUtilities.Model;
using R3;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.Products.Model
{
    public class PeriodicProducts : IDisposable
    {
        private readonly ProductDataContainer _data;
        [Inject] private TimeService _timeService;

        private readonly CompositeDisposable _disposable = new();
        private readonly Dictionary<PeriodicType, ReadOnlyReactiveProperty<float>> _refreshes = new();
        private readonly Dictionary<PeriodicType, IReadOnlyDictionary<int, IReadOnlyList<Product>>> _dictionary = new();
        private readonly Dictionary<PeriodicType, int> _order = new();
        private readonly Dictionary<PeriodicType, int> _maxOrder = new();
        private readonly CultureInfo _culture = new("ru-RU"); 

        public IReadOnlyDictionary<PeriodicType, ReadOnlyReactiveProperty<float>> Refreshes => _refreshes;
        public IReadOnlyDictionary<PeriodicType, IReadOnlyDictionary<int, IReadOnlyList<Product>>> Dictionary => _dictionary;
        
        public event Action<PeriodicType> OnRefreshed; 

        public PeriodicProducts(IReadOnlyList<Product> products, ProductDataContainer data)
        {
            _data = data;
            var tempDictionary = new Dictionary<PeriodicType, Dictionary<int, List<Product>>>();
            foreach (var product in products)
            {
                if (product.Placement != ProductPlacement.Periodic) continue;
                var periodicData = product.PeriodicData;
                if (!tempDictionary.TryGetValue(periodicData.Type, out var subDictionary))
                {
                    subDictionary = new Dictionary<int, List<Product>>();
                    tempDictionary.Add(periodicData.Type, subDictionary);
                }

                if (!subDictionary.TryGetValue(periodicData.Order, out var list))
                {
                    list = new List<Product> { product };
                    subDictionary.Add(periodicData.Order, list);
                    if (_maxOrder.ContainsKey(periodicData.Type))
                        _maxOrder[periodicData.Type] = Math.Max(_maxOrder[periodicData.Type], periodicData.Order);
                    else
                        _maxOrder.Add(periodicData.Type, periodicData.Order);
                }
                else
                    list.Add(product);
            }

            foreach (var kvp in tempDictionary)
            {
                var subDic = new Dictionary<int, IReadOnlyList<Product>>();
                _dictionary.Add(kvp.Key, subDic);
                foreach (var subKvp in kvp.Value)
                    subDic.Add(subKvp.Key, subKvp.Value);
            }
        }

        public void Initialize()
        {
            FillRefreshes();
            UpdateOrders();
        }

        public IReadOnlyList<Product> GetActualProducts(PeriodicType type) => 
            _dictionary[type][_order[type]];

        private void FillRefreshes()
        {
            _refreshes.Add(PeriodicType.Monthly, _timeService.TimeToEndMonth);
            _timeService.TimeToEndMonth
                .Where(value => value <= 0)
                .Subscribe(_ => UpdateOrder(PeriodicType.Monthly, _timeService.Now()))
                .AddTo(_disposable);
            _refreshes.Add(PeriodicType.Weekly, _timeService.TimeToEndWeek);
            _refreshes.Add(PeriodicType.Resources, _timeService.TimeToEndWeek);
            _timeService.TimeToEndWeek
                .Where(value => value <= 0)
                .Subscribe(_ =>
                {
                    var now = _timeService.Now();
                    UpdateOrder(PeriodicType.Weekly, now);
                    UpdateOrder(PeriodicType.Resources, now);
                })
                .AddTo(_disposable);
            _refreshes.Add(PeriodicType.Daily, _timeService.TimeToEndDay);
            _timeService.TimeToEndDay
                .Where(value => value <= 0)
                .Subscribe(_ => UpdateOrder(PeriodicType.Daily, _timeService.Now()))
                .AddTo(_disposable);
        }

        private void UpdateOrders()
        {
            var now = _timeService.Now();
            foreach (var (type, _) in _maxOrder) 
                UpdateOrder(type, now);
        }

        private void UpdateOrder(PeriodicType type, DateTime now)
        {
            var maxOrder = _maxOrder[type];
            int guessOrder = 0;
            switch (type)
            {
                case PeriodicType.Monthly:
                    guessOrder = now.Month;
                    break;
                case PeriodicType.Weekly:
                case PeriodicType.Resources:
                    guessOrder = _culture.Calendar.GetWeekOfYear(
                        now,
                        _culture.DateTimeFormat.CalendarWeekRule,
                        _culture.DateTimeFormat.FirstDayOfWeek
                    );
                    break;
                case PeriodicType.Daily:
                    guessOrder = now.DayOfYear;
                    break;
            }

            _order[type] = (guessOrder - 1) % maxOrder + 1;
            var previousOrder = _data.GuessOrders[type];
            if (previousOrder >= 0 && guessOrder > previousOrder)
            {
                foreach (var product in GetActualProducts(type)) 
                    product.Refresh();
                OnRefreshed?.Invoke(type);
            }
            _data.GuessOrders[type] = guessOrder;
        }

        public void Dispose() => 
            _disposable.Dispose();
    }
}