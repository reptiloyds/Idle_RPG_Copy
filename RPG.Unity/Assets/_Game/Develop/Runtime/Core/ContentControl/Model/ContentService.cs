using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PleasantlyGames.RPG.Runtime.AssetManagement;
using PleasantlyGames.RPG.Runtime.Core.Balance.Json;
using PleasantlyGames.RPG.Runtime.Core.Balance.Model;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Contract;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Variant.Branch;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Variant.Companion;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Variant.Dungeon;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Variant.Lootbox;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Variant.PopupWindow;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Variant.Product;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Variant.Skill;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Model.Variant.Stuff;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Save;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Sheet;
using PleasantlyGames.RPG.Runtime.Core.ContentControl.Type;
using PleasantlyGames.RPG.Runtime.Localization.Contracts;
using VContainer;

namespace PleasantlyGames.RPG.Runtime.Core.ContentControl.Model
{
    public class ContentService : IDisposable
    {
        private ContentDataContainer _data;
        private readonly List<Content> _contents = new();
        
        private readonly Dictionary<string, Content> _stuffDictionary = new();
        private readonly Dictionary<int, Content> _companionDictionary = new();
        private readonly Dictionary<int, Content> _skillDictionary = new();
        private readonly Dictionary<string, Content> _branchDictionary = new();
        private readonly Dictionary<string, Content> _lootboxDictionary = new();
        private readonly Dictionary<string, Content> _subModeDictionary = new();
        private readonly Dictionary<string, Content> _popupDictionary = new();
        private readonly Dictionary<string, Content> _productDictionary = new();

        [Inject] private BalanceContainer _balance;
        [Inject] private ITranslator _translator;
        [Inject] private ISpriteProvider _spriteProvider;
        [Inject] private IObjectResolver _resolver;

        //TODO piece of shit, rework it!
        public bool IsInitialized { get; private set; }
        public event Action OnInitialized;

        public event Action<Content> OnContentUnlocked; 

        [Preserve]
        public ContentService() { }

        public void Setup(ContentDataContainer data) => 
            _data = data;

        public void Initialize()
        {
            CreateModels();
            SetupDependencies();
            foreach (var content in _contents) 
                content.Initialize();
            IsInitialized = true;
            OnInitialized?.Invoke();
        }

        public void Dispose()
        {
            foreach (var content in _contents)
            {
                content.Dispose();
                content.OnUnlocked -= OnUnlocked;
            }
        }

        public List<Content> GetManualLockedContent()
        {
            var result = new List<Content>(_contents.Count);
            result.AddRange(_contents.Where(content => content.IsManualUnlock && !content.IsUnlocked));
            return result;
        }
        
        private void OnUnlocked(Contract.IUnlockable unlockable) => 
            OnContentUnlocked?.Invoke((Content)unlockable);

        private void CreateModels()
        {
            var contentSheet = _balance.Get<ContentSheet>();
            foreach (var contentData in _data.List)
            {
                if(contentSheet.TryGetValue(contentData.Id, out var config))
                    CreateModel(contentData, config);
            }
        }

        private void CreateModel(ContentData data, ContentSheet.Row config)
        {
            var content = new Content(data, config, _translator, _spriteProvider);
            switch (config.ContentType)
            {
                case ContentType.StuffSlot:
                    var stuffData = JsonConvert.DeserializeObject<StuffContentData>(config.ContentDataJSON);
                    _stuffDictionary.Add(stuffData.SlotId, content);
                    break;
                case ContentType.CompanionSlot:
                    var companionData = JsonConvert.DeserializeObject<CompanionContentData>(config.ContentDataJSON);
                    _companionDictionary.Add(companionData.SlotId, content);
                    break;
                case ContentType.SkillSlot:
                    var skillData = JsonConvertLog.DeserializeObject<SkillContentData>(config.ContentDataJSON);
                    _skillDictionary.Add(skillData.SlotId, content);
                    break;
                case ContentType.Branch:
                    var branchData = JsonConvert.DeserializeObject<BranchContentData>(config.ContentDataJSON);
                    _branchDictionary.Add(branchData.BranchId, content);
                    break;
                case ContentType.Lootbox:
                    var lootboxData = JsonConvert.DeserializeObject<LootboxContentData>(config.ContentDataJSON);
                    _lootboxDictionary.Add(lootboxData.LootboxId, content);
                    break;
                case ContentType.SubMode:
                    var subModeData = JsonConvert.DeserializeObject<SubModeData>(config.ContentDataJSON);
                    _subModeDictionary.Add(subModeData.Id, content);
                    break;
                case ContentType.PopupWindow:
                    var popupData = JsonConvert.DeserializeObject<PopupWindowContentData>(config.ContentDataJSON);
                    _popupDictionary.Add(popupData.Id, content);
                    break;
                case ContentType.Product:
                    var productData = JsonConvert.DeserializeObject<ProductContentData>(config.ContentDataJSON);
                    _productDictionary.Add(productData.Id, content);
                    break;
            }
            
            _resolver.Inject(content);
            content.OnUnlocked += OnUnlocked;
            _contents.Add(content);
        }

        private void SetupDependencies()
        {
            foreach (var content in _contents)
            {
                var dependency = _contents.FirstOrDefault(item => item.Id == content.Dependency);
                if(dependency != null)
                    content.SetupDependency(dependency);
            }
        }

        public bool IsUnlocked(string id)
        {
            foreach (var content in _contents)
            {
                if(!string.Equals(content.Id, id)) continue;
                return content.IsUnlocked;
            }

            return false;
        }

        public Content GetById(string id)
        {
            foreach (var content in _contents)
                if(string.Equals(id, content.Id))
                    return content;

            return null;
        }

        public IUnlockable GetStuff(string slotId) => 
            _stuffDictionary.GetValueOrDefault(slotId);

        public IUnlockable GetCompanion(int companionId) => 
            _companionDictionary.GetValueOrDefault(companionId);

        public IUnlockable GetSkill(int skillId) => 
            _skillDictionary.GetValueOrDefault(skillId);
        
        public IUnlockable GetBranch(string branchId) => 
            _branchDictionary.GetValueOrDefault(branchId);

        public IUnlockable GetLootbox(string lootboxId) =>
            _lootboxDictionary.GetValueOrDefault(lootboxId);

        public IUnlockable GetSubMode(string subModeId) => 
            _subModeDictionary.GetValueOrDefault(subModeId);

        public IUnlockable GetPopupWindow(string id) => 
            _popupDictionary.GetValueOrDefault(id);
        
        public IUnlockable GetProduct(string id) => 
            _productDictionary.GetValueOrDefault(id);

        public void Unlock(List<ContentType> contentTypes)
        {
            foreach (var contentType in contentTypes) 
                Unlock(contentType);
        }

        public void Unlock(ContentType contentType)
        {
            foreach (var content in _contents)
            {
                if(contentType != content.Type) continue;
                if (!content.IsUnlocked)
                    content.Unlock();
            }
        }
    }
}