using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using RandomizerMod.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreEndings
{
    //Looks Menu's back on the menu, boys!
    internal class Menu
    {
        internal MenuPage MoreEndingsPage;

        internal SmallButton MenuActivationButton;

        internal static Menu Instance { get; private set; }

        ToggleButton EnableToggle1;
        NumericEntryField<uint> TotalCount;
        NumericEntryField<uint> RequiredCount;

        public static void Hook()
        {
            RandomizerMenuAPI.AddMenuPage((landingPage) => Instance = new(landingPage), ReturnMainMenuButton);
        }

        public static bool ReturnMainMenuButton(MenuPage landingPage, out SmallButton button)
        {
            button = Instance.MenuActivationButton;
            return true;
        }

        private void UpdateEnable(bool value)
        {
            DreamerShardRando.saveSettings.enabled = value;
        }

        private void UpdateTotal(uint value)
        {
            if (value == 0) value = 1;
            DreamerShardRando.saveSettings.totalShardCount = value;
        }

        private void UpdateRequired(uint value)
        {
            if (value == 0) value = 1;
            DreamerShardRando.saveSettings.shardsRequired = value;
        }

        public void UpdateSettings()
        {
            EnableToggle1.SetValue(DreamerShardRando.saveSettings.enabled);
            TotalCount.SetValue(DreamerShardRando.saveSettings.totalShardCount);
            RequiredCount.SetValue(DreamerShardRando.saveSettings.shardsRequired);
        }


        private Menu(MenuPage landingPage)
        {
            MoreEndingsPage = new MenuPage(DreamerShardRando.ModName, landingPage);

            landingPage.BeforeShow += () => MenuActivationButton.Text.color = DreamerShardRando.saveSettings.enabled ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;

            VerticalItemPanel layout = new(MoreEndingsPage, new(0, 300), 150, true);

            EnableToggle1 = new(MoreEndingsPage, "Enable " + DreamerShardRando.ModName);
            EnableToggle1.ValueChanged += UpdateEnable;

            TotalCount = new(MoreEndingsPage, "Total number of shards");
            TotalCount.ValueChanged += UpdateTotal;
            TotalCount.SetValue(20);

            RequiredCount = new(MoreEndingsPage, "Shards required to open Black Egg");
            RequiredCount.ValueChanged += UpdateRequired;
            RequiredCount.SetValue(10);

            layout.Add(new MenuLabel(MoreEndingsPage, DreamerShardRando.ModName, MenuLabel.Style.Title));
            layout.Add(new MenuLabel(MoreEndingsPage, "Splits the three Dreamers of Hallownest into an amount of Dreamer shards of your choice", MenuLabel.Style.Body));

            layout.Add(EnableToggle1);
            layout.Add(TotalCount);
            layout.Add(RequiredCount);

            /*layout.Add(new MenuLabel(MoreEndingsPage, "If you're using more than 5 total shards, it is recommended to use the 'Full Flexible' option added by RandoPlus", MenuLabel.Style.Body));
            layout.Add(new MenuLabel(MoreEndingsPage, "Otherwise the shops can become quite crammed", MenuLabel.Style.Body));*/

            MenuActivationButton = new(landingPage, DreamerShardRando.ModName);
            MenuActivationButton.AddHideAndShowEvent(landingPage, MoreEndingsPage);
        }
    }
}
